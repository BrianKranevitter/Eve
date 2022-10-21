using Enderlook.Enumerables;
using Enderlook.Unity.Toolset.Attributes;

using Game.Utility;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Game.Player.Weapons
{
    public sealed class WeaponManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField, Tooltip("A list of all the weapon that exists.")]
        private WeaponPack[] allWeapons;

        [SerializeField, Tooltip("Types of ammunitions.")]
        private AmmunitionType[] ammunitions;

        [Header("Setup")]
        [SerializeField, Tooltip("Key used to reload ammunition.")]
        private KeyCode reloadKey;

        [SerializeField, Tooltip("Key used to melee hit.")]
        private KeyCode hitKey;

        [SerializeField, Tooltip("Key used to toggle light.")]
        private KeyCode lightKey;

        [field: SerializeField, IsProperty, Tooltip("Camera where target point is generated.")]
        public Camera ShootCamera { get; private set; }

        [SerializeField, Tooltip("Animator of the camera used for player sight.")]
        private Animator eyeCameraAnimator;

        private static WeaponManager instance;
        public static WeaponManager Instance => instance;

        private int currentWeaponIndex;

        private Lantern[] lanterns;

        private int weaponScroll;

        private Weapon[] weapons;

        private Weapon CurrentWeapon => weapons[currentWeaponIndex];

        private Lantern CurrentLantern {
            get {
                Lantern lantern = lanterns[currentWeaponIndex];
                if (lantern == null)
                    return null;
                return lantern;
            }
        }

        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogError($"{nameof(WeaponManager)} is a singlenton.");
                Destroy(this);
                return;
            }
            OnValidate();

            instance = this;

            weapons = allWeapons.Where(e => e.CanUse).Select(e => e.Weapon).ToArray();

            lanterns = new Lantern[weapons.Length];
            for (int i = 0; i < weapons.Length; i++)
            {
                Weapon weapon = weapons[i];
                weapon.Initialize(this);
                weapon.gameObject.SetActive(false);
                Lantern lantern = weapon.gameObject.GetComponentInChildren<Lantern>();
                if (lantern != null)
                {
                    lantern.Initialize();
                    lanterns[i] = lantern;
                }
            }
            CurrentWeapon.gameObject.SetActive(true);
        }

        private void Update()
        {
            if (PauseMenu.Paused) return;
            
            if (!PlayerBody.IsAlive)
                return;

            if (weapons.Length > 1)
            {
                float scrollWheel = Input.GetAxis("Mouse ScrollWheel");

                if (scrollWheel != 0 && weaponScroll == 0)
                {
                    weaponScroll = scrollWheel > 0 ? 1 : -1;
                    CurrentWeapon.TriggerOutAnimation();
                }

                if (weaponScroll != 0)
                    return;
            }

            Weapon weapon = CurrentWeapon;
            if (weapon.PrimaryCanBeHeldDown ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0))
                weapon.TryPrimaryShoot();
            else if (weapon.SecondaryCanBeHeldDown ? Input.GetMouseButton(1) : Input.GetMouseButtonDown(1))
                weapon.TrySecondaryShoot();
            else if (Input.GetKeyDown(reloadKey))
                weapon.TryReload();
            else if (Input.GetKeyDown(hitKey))
                weapon.TryMeleeHit();

            Lantern lantern = CurrentLantern;
            if (lantern is null)
                return;
            if (Input.GetKeyDown(lightKey))
            {
                if (Lantern.ActiveLight == null)
                    lantern.SetOn();
                else
                    lantern.SetOff();
            }
        }

        public void FinalizeTriggerOutAnimation()
        {
            bool hasLight = Lantern.ActiveLight != null;

            CurrentWeapon.gameObject.SetActive(false);
            CurrentLantern?.SetOffImmediately();

            if (weaponScroll > 0)
            {
                currentWeaponIndex++;
                if (currentWeaponIndex == weapons.Length)
                    currentWeaponIndex = 0;
                Work();
            }
            else if (weaponScroll < 0)
            {
                currentWeaponIndex--;
                if (currentWeaponIndex == -1)
                    currentWeaponIndex = weapons.Length - 1;
                Work();
            }
            else
            {
                Work();
                CurrentWeapon.TriggerPickedUpAnimation();
            }

            weaponScroll = 0;

            void Work()
            {
                CurrentWeapon.gameObject.SetActive(true);
                if (hasLight)
                    CurrentLantern?.SetOnImmediately();
                else
                    CurrentLantern?.SetOffImmediately();
            }
        }

        public AmmunitionType GetAmmunitionType(string name)
        {
            for (int i = 0; i < ammunitions.Length; i++)
            {
                AmmunitionType ammunition = ammunitions[i];
                if (ammunition.Name == name)
                    return ammunition;
            }
            throw new KeyNotFoundException($"Not found ammunition type with name {name}.");
        }

        public void ForceTotalAmmunitionUIUpdate() => CurrentWeapon.ForceTotalAmmunitionUIUpdate();

        public void TrySetAnimationTriggerOnCamera(string triggerName, string metaTriggerName)
            => Try.SetAnimationTrigger(eyeCameraAnimator, triggerName, metaTriggerName, "eye camera");

        public void UnlockWeapon(string weaponName)
        {
            CurrentWeapon.TriggerOutAnimation();

            Weapon newWeapon = null;
            Weapon[] newWeapons = new Weapon[weapons.Length + 1];
            int j = 0;
            for (int i = 0; i < allWeapons.Length; i++)
            {
                ref WeaponPack weapon = ref allWeapons[i];
                if (weapon.CanUse)
                    newWeapons[j++] = weapon.Weapon;
                else if (weapon.Name == weaponName)
                {
                    weapon.CanUse = true;
                    newWeapon = weapon.Weapon;
                    currentWeaponIndex = j;
                    newWeapons[j++] = newWeapon;
                    Lantern lantern = newWeapon.GetComponentInChildren<Lantern>();
                    newWeapon.Initialize(this);
                    if (lantern != null)
                        lantern.Initialize();
                }
            }

            if (j == weapons.Length)
                Debug.LogError($"Weapon with name {weaponName} was not found or it was already unlocked");
            else
            {
                weaponScroll = 0;
                weapons = newWeapons;
                lanterns = new Lantern[newWeapons.Length];
                for (int i = 0; i < newWeapons.Length; i++)
                {
                    Lantern lantern = newWeapons[i].GetComponentInChildren<Lantern>();
                    if (lantern != null)
                        lanterns[i] = lantern;
                }
            }
        }

        private void OnValidate()
        {
            if (allWeapons.Select(e => e.Name).HasDuplicates())
                Debug.LogError("Has weapon with duplicated name.");
            if (allWeapons.Select(e => e.Weapon).HasDuplicates())
                Debug.LogError("Has duplicated weapons.");
        }

        [Serializable]
        private struct WeaponPack
        {
            [field: SerializeField, Tooltip("Associated weapon.")]
            public Weapon Weapon { get; private set; }

            [SerializeField, Tooltip("Whenever the player can use it.")]
            public bool CanUse;

            [field: SerializeField, Tooltip("Name of the weapon. Used to unlock it.")]
            public string Name { get; private set; }
        }
    }
}