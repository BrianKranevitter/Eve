using Enderlook.Unity.AudioManager;
using Enderlook.Unity.Toolset.Attributes;

using Game.Enemies;
using Game.Utility;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Game.Player.Weapons
{
    public abstract partial class Weapon : MonoBehaviour
    {
        [field: Header("Ammunition")]
        [field: SerializeField, IsProperty, Min(1), Tooltip("Amount of ammunition per magazine.")]
        protected int MaximumMagazineAmmo { get; private set; }

        [SerializeField, Tooltip("Name of the ammunition type used.")]
        private string ammunitionName;

        [SerializeField, Tooltip("The amount of ammo in magazine when picked up.")]
        private int startMagazineAmmo;

        [Header("HUD")]
        [SerializeField, Tooltip("Display amount of magazine ammunition in the weapon.")]
        protected TextMesh ammunitionMagazineDisplay;

        [SerializeField, Tooltip("Display amount of total ammunition in the weapon.")]
        protected TextMesh ammunitionTotalDisplay;

        [Header("Reload")]
        [SerializeField, Tooltip("Name of the reload animation trigger.")]
        private string reloadAnimationTrigger;

        [SerializeField, Tooltip("Sound player when reloading with no ammunition.")]
        private AudioUnit noAmmoSound;

        [Header("Waiting")]
        [SerializeField, Tooltip("Name of the waiting animation trigger.")]
        private string waitAnimationTrigger;

        [SerializeField, ShowIf(nameof(waitAnimationTrigger), typeof(string), "", false), Min(0), Tooltip("Determines each how much idle time the awaiting animation is triggered.")]
        private float waitAnimationTimer = 6;

        [Header("Melee")]
        [SerializeField, Tooltip("Collider which produces the melee hit.")]
        private Collider meleeCollider;

        [SerializeField, ShowIf(nameof(meleeCollider), typeof(Collider), null, false), Tooltip("Name of the melee animation trigger. (The animation must execute Melee() at some point.)")]
        private string meleeAnimationTrigger;

        [SerializeField, ShowIf(nameof(meleeCollider), typeof(Collider), null, false), Tooltip("Amount of damage produced on a melee hit.")]
        private float meleeDamage;

        [Header("Effects")]
        [SerializeField, Tooltip("Point where shoot particles will spawn.")]
        private Transform shootParticlesSpawnPoint;

        [SerializeField, Tooltip("Configuration of the spawed shell.")]
        private ShellConfiguration shellToSpawn;

        [Header("Primary Shoot")]
        [SerializeField, Tooltip("Configuration of the primary shoot.")]
        private ShootTypeConfiguration primaryConfiguration;

        [Header("Secondary Shoot")]
        [SerializeField, Tooltip("Configuration of the secondary shoot.")]
        private ShootTypeConfiguration secondaryConfiguration;

        [Header("Other")]
        [SerializeField, Tooltip("Animation trigger used to switch out from the weapon.")]
        private string switchOutAnimationTrigger;

        [SerializeField, Tooltip("Animation trigger used when the weapon is picked up by first time.")]
        private string pickedUpAnimationTrigger;

        public bool PrimaryCanBeHeldDown => primaryConfiguration.CanBeHeldDown;

        public bool SecondaryCanBeHeldDown => secondaryConfiguration.CanBeHeldDown;

        protected int CurrentMagazineAmmo
        {
            get => currentMagazineAmmo;
            set
            {
                currentMagazineAmmo = value;
                if (ammunitionMagazineDisplay == null)
                    Debug.LogWarning("Missing ammunition magazine display.");
                else
                    ammunitionMagazineDisplay.text = value.ToString();
            }
        }

        protected int CurrentTotalAmmo
        {
            get => ammunition.CurrentAmmunition;
            set
            {
                ammunition.CurrentAmmunition = value;
                if (ammunitionTotalDisplay == null)
                    Debug.LogWarning("Missing ammunition total display.");
                else
                    ammunitionTotalDisplay.text = value.ToString();
            }
        }

        protected Animator Animator { get; private set; }

        private Camera Camera {
            get {
                WeaponManager manager = this.manager;
                if (manager != null)
                {
                    Camera camera = manager.ShootCamera;
                    if (camera != null)
                        return camera;
                }
                return Camera.main;
            }
        }

        private float canShootAt;
        private bool isInShootingAnimation;

        private bool isInWaitingAnimation;
        private float nextWaitingAnimation;

        private bool isInReloadAnimation;

        protected bool IsReady => !isInShootingAnimation && Time.time >= canShootAt;

        private int currentMagazineAmmo;

        private AmmunitionType ammunition;

        private WeaponManager manager;

        private MeleeAttack meleeAttack;

        private void Awake()
        {
            Animator = GetComponent<Animator>();
            Debug.Assert(Animator != null);
            ResetWaitingAnimation();
            if (meleeCollider == null)
                Debug.LogWarning("Missing melee collider.");
            else
            {
                meleeAttack = meleeCollider.gameObject.AddComponent<MeleeAttack>();
                meleeCollider.isTrigger = true;
                meleeCollider.enabled = false;
            }
        }

        public void Initialize(WeaponManager manager)
        {
            this.manager = manager;
            ammunition = manager.GetAmmunitionType(ammunitionName);
            CurrentMagazineAmmo = startMagazineAmmo;
            ForceTotalAmmunitionUIUpdate();
        }

        public void ForceTotalAmmunitionUIUpdate() => CurrentTotalAmmo = CurrentTotalAmmo; // Trigger UI update for total ammo.

        public void PlayAudioOneShoot(AudioUnit audio) => AudioController.PlayOneShoot(audio, transform.position);

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
                Animator.SetTrigger("LowBattery");

            if (PlayerController.IsMoving)
                nextWaitingAnimation = Time.time + waitAnimationTimer;

            if (isInShootingAnimation || isInWaitingAnimation || Time.time < nextWaitingAnimation)
                return;

            if (Try.SetAnimationTrigger(Animator, waitAnimationTrigger, "wait animation trigger"))
                isInWaitingAnimation = true;
        }

        public void TryMeleeHit()
        {
            if (isInShootingAnimation)
                return;
            if (meleeCollider == null)
                return;
            if (!Try.SetAnimationTrigger(Animator, meleeAnimationTrigger, "go to melee"))
                Melee();
        }

        private void Melee()
        {
            if (meleeCollider == null)
                return;

            StartCoroutine(Work());
            IEnumerator Work()
            {
                meleeCollider.enabled = true;
                yield return null;
                meleeCollider.enabled = false;
                foreach (IGrouping<Enemy, IDamagable> enemy in meleeAttack
                    .damagables
                    .GroupBy(e => ((Component)e).gameObject.GetComponentInParent<Enemy>()))
                    foreach (IDamagable damagable in enemy)
                        damagable.TakeDamage(meleeDamage / enemy.Count());
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void FromWait() => ResetWaitingAnimation();

        public void TryReload()
        {
            if (!IsReady)
                return;

            if (CurrentMagazineAmmo == MaximumMagazineAmmo)
                // We already has ammo.
                return;

            if (CurrentTotalAmmo == 0)
            {
                Try.PlayOneShoot(transform, noAmmoSound, "no ammo");
                // We don't have any ammo.
                return;
            }

            if (isInReloadAnimation)
                return;

            isInReloadAnimation = true;

            if (!Try.SetAnimationTrigger(Animator, reloadAnimationTrigger, "reload"))
                FromReload();
        }

        private void FromReload()
        {
            isInReloadAnimation = false;
            ResetWaitingAnimation();
            AfterReload();
        }

        protected virtual void AfterReload()
        {
            int need = MaximumMagazineAmmo - CurrentMagazineAmmo;
            if (need >= CurrentTotalAmmo)
            {
                CurrentMagazineAmmo += CurrentTotalAmmo;
                CurrentTotalAmmo = 0;
            }
            else
            {
                CurrentMagazineAmmo = MaximumMagazineAmmo;
                CurrentTotalAmmo -= need;
            }
        }

        public void TriggerOutAnimation()
        {
            isInShootingAnimation = false;
            isInReloadAnimation = false;
            Try.SetAnimationTrigger(Animator, switchOutAnimationTrigger, "switch out");
        }

        public void FromGetOut() => manager.FinalizeTriggerOutAnimation();

        public void TryPrimaryShoot()
        {
            if (!IsReady)
                // We are still in cooldown.
                return;

            if (CurrentMagazineAmmo > 0)
            {
                isInShootingAnimation = true;

                ToPrimaryShoot();

                shellToSpawn.Spawn();

                if (primaryConfiguration.Shoot(manager, Animator, transform, shootParticlesSpawnPoint, out canShootAt))
                    FromShoot();
            }
            else
                TryReload();
        }

        protected abstract void ToPrimaryShoot();

        public void TrySecondaryShoot()
        {
            if (!IsReady)
                // We are still in cooldown.
                return;

            if (CurrentMagazineAmmo > 0)
            {
                isInShootingAnimation = true;

                ToSecondaryShoot();

                shellToSpawn.Spawn();

                if (secondaryConfiguration.Shoot(manager, Animator, transform, shootParticlesSpawnPoint, out canShootAt))
                    FromShoot();
            }
            else
                TryReload();
        }

        protected abstract void ToSecondaryShoot();

        private void FromShoot()
        {
            isInShootingAnimation = false;
            ResetWaitingAnimation();
            isInReloadAnimation = false; // If the player shoot when it was reloading, this flag can still be set.
            if (CurrentMagazineAmmo == 0)
                TryReload();
        }

        protected Ray GetShootRay() => GetShootCamera().ViewportPointToRay(new Vector3(.5f, .5f));

        protected Transform GetShootPointTransform() => GetShootCamera().transform;

        protected Vector3 GetShootPointPosition() => GetShootCamera().transform.position;

        private Camera GetShootCamera()
        {
            Camera camera = Camera;
#if UNITY_EDITOR
            if (!Application.isPlaying && camera == null)
                camera = Camera.main;
#endif
            return camera;
        }

        private void OnDisable()
        {
            isInShootingAnimation = false;
            isInWaitingAnimation = false;
        }

        private void OnEnable()
        {
            if (ammunition != null)
                ForceTotalAmmunitionUIUpdate();
        }

        protected void ResetWaitingAnimation()
        {
            isInWaitingAnimation = false;
            nextWaitingAnimation = Time.time + waitAnimationTimer;
        }

        public void TriggerPickedUpAnimation() => Try.SetAnimationTrigger(Animator, pickedUpAnimationTrigger, "picked up");

        private sealed class MeleeAttack : MonoBehaviour
        {
            [NonSerialized]
            public List<IDamagable> damagables = new List<IDamagable>();

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
            private void OnTriggerEnter(Collider other)
            {
                IDamagable damagable = other.GetComponentInParent<IDamagable>();
                if (damagable != null)
                    damagables.Add(damagable);
            }
        }
    }
}
