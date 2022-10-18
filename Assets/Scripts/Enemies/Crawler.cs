using Enderlook.Unity.AudioManager;

using Game.Player;
using Game.Utility;

using System;
using System.Collections;
using Game.Player.Weapons;
using UnityEngine;

namespace Game.Enemies
{
    public sealed class Crawler : Enemy
    {
        [Header("Sight")]
        [SerializeField, Min(0), Tooltip("Determines at which distance from player the creature start shooting.")]
        private float startShootingRadius = 3;

        [SerializeField, Min(0), Tooltip("Determines at which distance from player the creature stop shooting.")]
        private float stopShootingRadius = 4;

        [SerializeField, Min(0), Tooltip("Determines at which distance from player the creature start charging.")]
        private float startChargeRadius = 2;

        [SerializeField, Min(0), Tooltip("Determines at which distance from player the creature start melee.")]
        private float startMeleeRadius = .75f;

        [SerializeField, Min(0), Tooltip("Determines at which distance from player the creature stop melee.")]
        private float stopMeleeRadius = 1.5f;

        [Header("Melee")]
        [SerializeField, Min(0), Tooltip("Movement speed multiplier when charging to player.")]
        private float chargingSpeedMultiplier = 1;

        [SerializeField, Min(0), Tooltip("Amount of damage produced on melee strike.")]
        private float meleeDamage = 20;

        [SerializeField, Tooltip("Determines from which collider melee damage is done.")]
        private Collider meleePosition;

        [Header("Stun")]
        [SerializeField, Tooltip("Amount of accumulated damage required from weakspot to stun the creature.")]
        private float stunDamageRequired = 20;

        [SerializeField, Tooltip("Time required to lose accumulated damage for stun.")]
        private float stunClearTimer = 5;

        [SerializeField, Tooltip("Duration after an stun where creature doesn't accumulate damage.")]
        private float stunImmunityDuration = 2;

        [Header("Animation Triggers")]
        [SerializeField, Tooltip("The mouth object with animation of acid puke.")]
        private Animator mouthAnimator;

        [SerializeField, Tooltip("Name of the animation trigger for mouth.")]
        private string mouthAnimationTrigger;

        [SerializeField, Tooltip("Name of the animation trigger when stunned (the animation must execute `FromStun()` at the end).")]
        private string stunAnimationTrigger;

        [SerializeField, Tooltip("Name of the animation trigger when is running towards the player to attack him.")]
        private string huntAnimationTrigger;

        [SerializeField, Tooltip("Name of the animation trigger when lose line of sight to player and is running to its last position.")]
        private string chaseAnimationTrigger;

        [SerializeField, Tooltip("Name of the animation trigger when is charging towards player.")]
        private string chargeAnimationTrigger;

        [SerializeField, Tooltip("Name of the animation trigger used to shoot (the animation must execute `Shoot()` event at some point and `FromShoot()` at the end).")]
        private string shootAnimationTrigger;

        [SerializeField, Tooltip("Name of the animation trigger used to melee (the animation must execute `Melee()` event at some point and `FromMelee()` at the end).")]
        private string meleeAnimationTrigger;

        [Header("Sounds")]
        [SerializeField, Tooltip("Sound played on shoot.")]
        private AudioFile shootSound;

        [SerializeField, Tooltip("Sound played on melee.")]
        private AudioFile meleeSound;
        
        private MeleeAttack meleeAttack;

        private bool isInShootingAnimation;
        private bool isInMeleeAnimation;
        private bool isInStunAnimation;

        private float accumulatedDamage;
        private float accumulatedDamageCleanAt;
        private float accumulatedImmunityUntil;

        private new class State : Enemy.State
        {
            public const byte HuntingPlayer = 10;
            public const byte ChasingPlayer = 11;
            public const byte ChargeToPlayer = 12;
            public const byte Shooting = 13;
            public const byte Melee = 14;
            public const byte Stunned = 15;
        }

        protected override void Awake()
        {
            base.Awake();

            meleePosition.enabled = false;
            meleeAttack = meleePosition.gameObject.AddComponent<MeleeAttack>();
            meleeAttack.crawler = this;
            meleeAttack.enabled = false;

            // Square values to avoid applying square root when checking distance.
            startShootingRadius *= startShootingRadius;
            stopShootingRadius *= stopShootingRadius;
            startMeleeRadius *= startMeleeRadius;
            stopMeleeRadius *= stopMeleeRadius;
            startChargeRadius *= startChargeRadius;
        }

        private void FixedUpdate()
        {
            if (!IsAlive)
                return;

            switch (state)
            {
                case State.Idle:
                {
                    if (HasPlayerInSight())
                        GoToHuntState();
                    break;
                }
                case State.HuntingPlayer:
                {
                    if (!HasPlayerInSight())
                    {
                        GoToChaseState();
                        break;
                    }

                    bool success = NavAgent.SetDestination(LastPlayerPosition);
                    Debug.Assert(success);
                    float sqrDistance = (LastPlayerPosition - transform.position).sqrMagnitude;
                    if (sqrDistance <= startMeleeRadius)
                    {
                        NavAgent.isStopped = true;
                        GoToMeleeState();
                    }
                    else if (sqrDistance <= startChargeRadius)
                        GoToChargeState();
                    else if (sqrDistance <= startShootingRadius)
                    {
                        NavAgent.isStopped = true;
                        GoToShootState();
                    }
                    break;
                }
                case State.ChasingPlayer:
                {
                    if (HasPlayerInSight())
                    {
                        float sqrDistance = (LastPlayerPosition - transform.position).sqrMagnitude;
                        if (sqrDistance <= startMeleeRadius)
                        {
                            NavAgent.isStopped = true;
                            GoToMeleeState();
                        }
                        else if (sqrDistance <= startChargeRadius)
                            GoToChargeState();
                        else if (sqrDistance <= startShootingRadius)
                        {
                            NavAgent.isStopped = true;
                            GoToShootState();
                        }
                        else
                            GoToHuntState();
                    }
                    else if (NavAgent.remainingDistance == 0)
                        GoToIdleState();
                    else
                    {
                        if (Time.frameCount % 10 == 0)
                        {
                            // Relcalculate path just in case.
                            bool success = NavAgent.SetDestination(LastPlayerPosition);
                            Debug.Assert(success);
                        }
                    }
                    break;
                }
                case State.Shooting:
                {
                    if (isInShootingAnimation)
                        break;

                    if (!HasPlayerInSight())
                    {
                        GoToChaseState();
                        break;
                    }

                    float sqrDistance = (LastPlayerPosition - transform.position).sqrMagnitude;
                    if (sqrDistance > stopShootingRadius)
                    {
                        GoToHuntState();
                        break;
                    }
                    else if (sqrDistance <= startMeleeRadius)
                    {
                        GoToMeleeState();
                        break;
                    }
                    else if (sqrDistance <= startChargeRadius)
                    {
                        GoToChargeState();
                        break;
                    }

                    LookAtPlayer();

                    ToShoot();

                    break;
                }
                case State.Melee:
                {
                    if (isInMeleeAnimation)
                        break;

                    if (!HasPlayerInSight())
                    {
                        GoToChaseState();
                        break;
                    }

                    float sqrDistance = (LastPlayerPosition - transform.position).sqrMagnitude;
                    if (sqrDistance > startMeleeRadius)
                    {
                        GoToHuntState();
                        break;
                    }

                    LookAtPlayer();

                    ToMelee();

                    break;
                }
                case State.ChargeToPlayer:
                {
                    if (!HasPlayerInSight())
                    {
                        GoToChaseState();
                        break;
                    }

                    float sqrDistance = (LastPlayerPosition - transform.position).sqrMagnitude;
                    if (sqrDistance <= startMeleeRadius)
                    {
                        GoToMeleeState();
                        break;
                    }

                    bool success = NavAgent.SetDestination(LastPlayerPosition);
                    Debug.Assert(success);
                    break;
                }
            }

            if (HasLightInRange())
            {
                LightEffect();
            }
        }
        
        protected override void LightEffect()
        {
            switch (Lantern.ActiveLantern.lanternType)
            {
                case Lantern.LanternType.White:
                    GoToIdleState();
                    break;
                
                case Lantern.LanternType.Red:
                    GoToHuntState();
                    break;
                
                case Lantern.LanternType.Blue:
                    GoToIdleState();
                    break;
            }
            
            base.LightEffect();
        }
        public void TriggerMouthAttackAnimation()
        {
            if (string.IsNullOrEmpty(mouthAnimationTrigger))
                Debug.LogWarning("Missing mouth animation trigger.");
            else
                mouthAnimator.SetTrigger(mouthAnimationTrigger);
        }

        private void GoToChargeState()
        {
            state = State.ChasingPlayer;
            NavAgent.isStopped = false;
            NavAgent.speed = initialSpeed * chargingSpeedMultiplier;
            bool success = NavAgent.SetDestination(LastPlayerPosition);
            Debug.Assert(success);

            TrySetAnimationTrigger(chargeAnimationTrigger, "charge");
        }

        private void GoToChaseState()
        {
            state = State.ChasingPlayer;
            NavAgent.isStopped = false;
            NavAgent.speed = initialSpeed;
            bool success = NavAgent.SetDestination(LastPlayerPosition);
            Debug.Assert(success);

            TrySetAnimationTrigger(chaseAnimationTrigger, "chase");
        }

        private void GoToHuntState()
        {
            state = State.HuntingPlayer;
            NavAgent.isStopped = false;
            NavAgent.speed = initialSpeed;
            bool success = NavAgent.SetDestination(LastPlayerPosition);
            Debug.Assert(success);

            TrySetAnimationTrigger(huntAnimationTrigger, "hunt");
        }

        private void GoToMeleeState()
        {
            state = State.Melee;
            isInMeleeAnimation = false; // Sometimes the flag may have a false positive if the animation was terminated abruptly.
            NavAgent.isStopped = true;
        }

        private void GoToShootState()
        {
            state = State.Shooting;
            isInShootingAnimation = false; // Sometimes the flag may have a false positive if the animation was terminated abruptly.
            NavAgent.isStopped = true;
        }

        private void ToShoot()
        {
            isInShootingAnimation = true;

            Try.PlayOneShoot(transform, shootSound, "shoot");

            if (!TrySetAnimationTrigger(shootAnimationTrigger, "shoot"))
                FromShoot();
        }

        private void FromShoot() => isInShootingAnimation = false;

        private void ToMelee()
        {
            isInMeleeAnimation = true;

            Try.PlayOneShoot(transform, meleeSound, "melee");

            if (!TrySetAnimationTrigger(meleeAnimationTrigger, "melee"))
            {
                Melee();
                FromMelee();
            }
        }

        private void Melee()
        {
            StartCoroutine(Work());
            IEnumerator Work()
            {
                meleePosition.enabled = true;
                yield return null;
                meleePosition.enabled = false;
            }
        }

        private void FromMelee() => isInMeleeAnimation = false;

        protected override void OnTakeDamage(float amount, bool isOnWeakspot)
        {
            base.OnTakeDamage(amount, isOnWeakspot);
            if (isOnWeakspot)
            {
                if (isInStunAnimation || IsInBlindAnimation || accumulatedImmunityUntil > Time.time)
                    return;
                if (Time.time >= accumulatedDamageCleanAt)
                    accumulatedDamage = 0;
                accumulatedDamage += amount;
                if (accumulatedDamage > stunDamageRequired)
                {
                    SaveStateAsPrevious();
                    state = State.Stunned;
                    accumulatedImmunityUntil = Time.time + stunImmunityDuration;

                    TrySetAnimationTrigger(stunAnimationTrigger, "stun");
                }
                else
                    accumulatedDamageCleanAt = Time.time + stunClearTimer;
            }
            if (state == State.Idle)
                GoToChaseState();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity animator.")]
        private void FromStun()
        {
            isInStunAnimation = false;
            accumulatedDamage = 0;
            accumulatedImmunityUntil = Time.time + stunImmunityDuration;
            LoadPreviousState();
            TrySetLastAnimationTrigger();
        }

        protected override void OnEndBlind()
        {
            isInStunAnimation = false; // Sometimes the flag may have a false positive if the animation was terminated abruptly.
            if (state == State.Idle)
                GoToChaseState();
        }

        public override void MakeAwareOfPlayerLocation()
        {
            base.MakeAwareOfPlayerLocation();
            switch (state)
            {
                case State.ChasingPlayer:
                    bool result = NavAgent.SetDestination(LastPlayerPosition);
                    Debug.Assert(result);
                    break;
                case State.Idle:
                    GoToChaseState();
                    break;
            }
        }

#if UNITY_EDITOR
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            Vector3 eyePosition = EyePosition;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(eyePosition, SqrtOnPlay(startShootingRadius));
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(eyePosition, SqrtOnPlay(stopShootingRadius));

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(eyePosition, SqrtOnPlay(startChargeRadius));

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(eyePosition, SqrtOnPlay(startMeleeRadius));
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(eyePosition, SqrtOnPlay(stopMeleeRadius));
        }

        private void OnValidate()
        {
            if (meleePosition != null)
                meleePosition.isTrigger = true;
            stopShootingRadius = Mathf.Min(sightRadius, stopShootingRadius);
            startShootingRadius = Mathf.Min(startShootingRadius, stopShootingRadius);
            startChargeRadius = Mathf.Min(startChargeRadius, startShootingRadius);
            stopMeleeRadius = Mathf.Min(stopMeleeRadius, startChargeRadius);
            stopMeleeRadius = Mathf.Min(sightRadius, stopMeleeRadius);
            startMeleeRadius = Mathf.Min(startMeleeRadius, stopMeleeRadius);
        }
#endif

        private sealed class MeleeAttack : MonoBehaviour
        {
            [NonSerialized]
            public Crawler crawler;

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
            private void OnTriggerEnter(Collider other)
            {
                PlayerBody player = other.GetComponentInParent<PlayerBody>();
                if (player != null)
                    player.TakeDamage(crawler.meleeDamage);
            }
        }
    }
}