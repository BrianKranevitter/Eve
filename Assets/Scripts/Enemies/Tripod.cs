using Enderlook.Unity.AudioManager;

using Game.Player;
using Game.Utility;

using System;
using System.Collections;
using System.Linq;

using UnityEngine;

namespace Game.Enemies
{
    public sealed class Tripod : Enemy
    {
        [Header("Sight")]
        [SerializeField, Min(0), Tooltip("Determines at which distance from player the creature start melee.")]
        private float startMeleeRadius = .75f;

        [SerializeField, Min(0), Tooltip("Determines at which distance from player the creature stop melee.")]
        private float stopMeleeRadius = 1.5f;

        [Header("Melee")]
        [SerializeField, Min(0), Tooltip("Amount of damage produced on melee strike.")]
        private float meleeDamage = 20;

        [SerializeField, Tooltip("Determines from which collider melee damage is done.")]
        private Collider meleePosition;

        [SerializeField, Min(0), Tooltip("Movement speed multiplier when chaing to player due to rage.")]
        private float chargingSpeedMultiplier = 1;

        [Header("Stun")]
        [SerializeField, Tooltip("Amount of accumulated damage required from weakspot to stun the creature.")]
        private float stunDamageRequired = 20;

        [SerializeField, Tooltip("Time required to lose accumulated damage for stun.")]
        private float stunClearTimer = 5;

        [SerializeField, Tooltip("Duration after an stun where creature doesn't accumulate damage.")]
        private float stunImmunityDuration = 2;

        [SerializeField, Tooltip("Colliders that are used for stun.")]
        private Collider[] stunColliders;

        [Header("Animation Triggers")]
        [SerializeField, Tooltip("Name of the animation trigger when leg is damaged (the animation must execute `FromStun()` at the end).")]
        private string stunAnimationTrigger;

        [SerializeField, Tooltip("Name of the animation trigger when is running towards the player to attack him.")]
        private string huntAnimationTrigger;

        [SerializeField, Tooltip("Name of the animation trigger when lose line of sight to player and is running to its last position.")]
        private string chaseAnimationTrigger;

        [SerializeField, Tooltip("Name of the animation trigger when is charging towards player due to rage.")]
        private string chargeAnimationTrigger;

        [SerializeField, Tooltip("Name of the animation trigger used to melee (the animation must execute `Melee()` event at some point and `FromMelee()` at the end).")]
        private string meleeAnimationTrigger;

        [Header("Sounds")]
        [SerializeField, Tooltip("Sound played on melee.")]
        private AudioFile meleeSound;

        private float initialSpeed;
        private MeleeAttack meleeAttack;

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
            public const byte Melee = 13;
            public const byte Stunned = 14;
        }

        protected override void Awake()
        {
            base.Awake();

            initialSpeed = NavAgent.speed;

            meleePosition.enabled = false;
            meleeAttack = meleePosition.gameObject.AddComponent<MeleeAttack>();
            meleeAttack.tripod = this;
            meleeAttack.enabled = false;

            foreach (GameObject go in stunColliders.Select(e => e.gameObject))
            {
                if (go == gameObject)
                    Debug.LogError("Leg colliders can't be attached to the gameobject that is attached Tripod. Use a children gameobject instead.");
                else
                    go.AddComponent<Stunner>().tripod = this;
            }

            // Square values to avoid applying square root when checking distance.
            startMeleeRadius *= startMeleeRadius;
            stopMeleeRadius *= stopMeleeRadius;
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
                        GoToMeleeState();
                    break;
                }
                case State.ChasingPlayer:
                {
                    if (HasPlayerInSight())
                    {
                        float sqrDistance = (LastPlayerPosition - transform.position).sqrMagnitude;
                        if (sqrDistance <= startMeleeRadius)
                            GoToMeleeState();
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
                case State.Melee:
                {
                    if (isInMeleeAnimation)
                        break;

                    if (!HasPlayerInSight())
                    {
                        GoToChaseState();
                        break;
                    }

                    LookAtPlayer();

                    float sqrDistance = (LastPlayerPosition - transform.position).sqrMagnitude;
                    if (sqrDistance > startMeleeRadius)
                    {
                        GoToHuntState();
                        break;
                    }

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
                meleeAttack.enabled = true;
                meleePosition.enabled = true;
                yield return null;
                meleeAttack.enabled = false;
                meleePosition.enabled = false;
            }
        }

        private void FromMelee() => isInMeleeAnimation = false;

        protected override void OnTakeDamage(float amount, bool isOnWeakspot)
        {
            base.OnTakeDamage(amount, isOnWeakspot);
            if (isOnWeakspot && HasPlayerInSight())
                GoToChargeState();
            else if (state == State.Idle)
                GoToChaseState();
        }

        private void FromStun()
        {
            isInStunAnimation = false;
            accumulatedDamage = 0;
            accumulatedImmunityUntil = Time.time + stunImmunityDuration;
            TrySetLastAnimationTrigger();
            LoadPreviousState();
        }

        private void TryStun(float amount)
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

                isInStunAnimation = true;
                if (!TrySetAnimationTrigger(stunAnimationTrigger, "stun", false))
                    FromStun();
            }
            else
                accumulatedDamageCleanAt = Time.time + stunClearTimer;
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

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(eyePosition, SqrtOnPlay(startMeleeRadius));
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(eyePosition, SqrtOnPlay(stopMeleeRadius));
        }

        private void OnValidate()
        {
            if (meleePosition != null)
                meleePosition.isTrigger = true;
            stopMeleeRadius = Mathf.Min(sightRadius, stopMeleeRadius);
            startMeleeRadius = Mathf.Min(startMeleeRadius, stopMeleeRadius);
        }
#endif

        private sealed class MeleeAttack : MonoBehaviour
        {
            [NonSerialized]
            public Tripod tripod;

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
            private void OnTriggerEnter(Collider other)
            {
                PlayerBody player = other.GetComponentInParent<PlayerBody>();
                if (player != null)
                    player.TakeDamage(tripod.meleeDamage);
            }
        }

        public sealed class Stunner : MonoBehaviour, IDamagable
        {
            [NonSerialized]
            public Tripod tripod;

            public void TakeDamage(float amount)
            {
                tripod.TakeDamage(amount);
                tripod.TryStun(amount);
            }
        }
    }
}