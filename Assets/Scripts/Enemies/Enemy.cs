using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Enderlook.Unity.AudioManager;
using Enderlook.Unity.Toolset.Attributes;

using Game.Player;
using Game.Player.Weapons;
using Game.Utility;

using UnityEngine;
using UnityEngine.AI;
using Color = UnityEngine.Color;

namespace Game.Enemies
{
    [RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(Animator))]
    public abstract class Enemy : MonoBehaviour, IDamagable, IBlindable
    {
        public static List<Enemy> allEnemies = new List<Enemy>();
        
        [SerializeField, Min(0), Tooltip("Maximum health of enemy.")]
        private float maximumHealth;
        
        [SerializeField, Tooltip("Colliders of the enemy")]
        private List<Collider> colliders;
        
        [Header("Sight")]
        [SerializeField, Min(0), Tooltip("Determines at which radius the creature can see the player.")]
        protected float sightRadius;

        [field: SerializeField, IsProperty, Tooltip("Layer that can block enemy sight.")]
        protected LayerMask BlockSight { get; set; }

        [SerializeField, Min(0), Tooltip("Height offset of eyes.")]
        private float eyeOffset = .5f;

        [Header("Animation Triggers")]
        [SerializeField, Tooltip("Name of the animation trigger when idle.")]
        private string idleAnimationTrigger;

        [SerializeField, Tooltip("Name of the animation trigger when it is hurt.")]
        private string hurtAnimationTrigger;

        [SerializeField, Tooltip("Name of the animation trigger when it is hurt on the weakspot.")]
        private string hurtWeakspotAnimationTrigger;

        [SerializeField, Tooltip("Name of the animation trigger when it dies (the animation must execute `FromDeath()` event on the last frame).")]
        private string deathAnimationTrigger;

        [SerializeField, Tooltip("Name of the animation trigger when it dies from an attack on the weakspot (the animation must execute `FromDeath()` event on the last frame).")]
        private string deathWeakspotAnimationTrigger;

        [SerializeField, Tooltip("Name of the animation trigger when blinded (the animation must execute `FromBlind()` at the end).")]
        private string blindAnimationTrigger;

        [Header("Sounds")]
        [SerializeField, Tooltip("Sound played when creature is hurt.")]
        private AudioFile hurtSound;

        [SerializeField, Tooltip("Sound played when creature is hurt on the weakspot.")]
        private AudioFile hurtWeakspotSound;

        [SerializeField, Tooltip("Sound played when creature die.")]
        private AudioFile deathSound;

        [SerializeField, Tooltip("Sound played when creature die on the weakspot.")]
        private AudioFile deathWeakspotSound;
        
        
        [Header("Escape")]
        [SerializeField, Min(0), Tooltip("Width of the oscillation when producing zig-zag.")]
        protected float oscillationWidth = 1.5f;

        [SerializeField, Min(0), Tooltip("Frequency of the oscillation when producing zig-zag.")]
        protected float oscillationFrequency = 3;

        [SerializeField, Min(0), Tooltip("Speed multiplier when escaping.")]
        protected float escapingSpeedMultiplier = 1;

        public void PlayAudioOneShoot(AudioUnit audio) => AudioController.PlayOneShoot(audio, transform.position);

        protected NavMeshAgent NavAgent { get; private set; }
        protected Animator Animator { get; private set; }

        protected bool IsAlive => currentHealth > 0;

        protected Vector3 EyePosition
        {
            get
            {
                Vector3 center = transform.position;
                center.y += eyeOffset;
                return center;
            }
        }

        protected Vector3 LastPlayerPosition { get; private set; }

        protected float initialSpeed;
        
        private float currentHealth;

        private string LastAnimationTrigger;

        protected bool IsInBlindAnimation { get; private set; }

        protected byte state;
        private byte previousState;

        protected class State
        {
            public const byte Idle = 0;
            public const byte Blinded = 1;
        }

        protected virtual void Awake()
        {
            currentHealth = maximumHealth;

            NavAgent = GetComponent<NavMeshAgent>();
            Animator = GetComponent<Animator>();

            initialSpeed = NavAgent.speed;

            // Square values to avoid applying square root when checking distance.
            sightRadius *= sightRadius;

            GoToIdleState();

            if (!allEnemies.Contains(this))
            {
                allEnemies.Add(this);
            }
        }

        private void OnDestroy()
        {
            if (allEnemies.Contains(this))
            {
                allEnemies.Remove(this);
            }
        }

        protected void GoToIdleState()
        {
            state = State.Idle;

            NavAgent.isStopped = true;

            TrySetAnimationTrigger(idleAnimationTrigger, "idle");
        }

        protected virtual void LightEffect()
        {
            switch (Lantern.ActiveLantern.lanternType)
            {
                case Lantern.LanternType.White:
                    NavAgent.isStopped = false;
                    NavAgent.speed = initialSpeed * escapingSpeedMultiplier;
                    SetEscapeDestination(PlayerBody.Player.transform.position);
                    break;
                
                case Lantern.LanternType.Red:
                    NavAgent.speed = initialSpeed * 3;
                    break;
                
                case Lantern.LanternType.Blue:
                    NavAgent.isStopped = true;
                    NavAgent.speed = 0;
                    
                    GoToIdleState();
                    break;
            }
        }
        
        protected void SetEscapeDestination(Vector3 playerPosition)
        {
            float distance = float.NegativeInfinity;
            Vector3 end = default;
            bool canOscillate = default;

            Vector3 eyePosition = EyePosition;

            playerPosition.y = eyePosition.y;

            Vector3 back = (eyePosition - playerPosition).normalized;
            Vector3 forward = -back;

            Check(back, true);

            Vector3 left = new Vector3(back.z, back.y, back.x);
            Check(left, false);
            Check(back + (left * .25f), true);
            Check(back + (left * .5f), true);
            Check(back + (left * .75f), false);
            Check(back + (left * 1f), false);
            Check(back + (left * 1.5f), false);
            Check(back + (left * 2f), false);
            Check(back + (left * 4f), false);
            Check(back + (left * 8f), false);
            Check(forward + (left * 8f), false);

            Vector3 right = new Vector3(back.z, back.y, -back.x);
            Check(right, false);
            Check(back + (right * .25f), false);
            Check(back + (right * .5f), false);
            Check(back + (right * .75f), false);
            Check(back + (right * 1f), false);
            Check(back + (right * 1.5f), false);
            Check(back + (right * 2f), false);
            Check(back + (right * 4f), false);
            Check(back + (right * 8f), false);
            Check(forward + (right * 8f), false);

            void Check(Vector3 direction, bool oscillate)
            {
                if (Physics.Raycast(eyePosition, direction, out RaycastHit hit, BlockSight)
                    && hit.distance > distance)
                {
                    distance = hit.distance;
                    end = hit.point;
                    canOscillate = oscillate;
                }
            }

            if (canOscillate && oscillationFrequency != 0 && oscillationWidth != 0)
            {
                Vector3 direction = (end - eyePosition).normalized;
                Vector3 perpendicular = new Vector3(direction.z, direction.y, direction.x);
                float wave = Mathf.Sin(Time.fixedTime * oscillationFrequency) * oscillationWidth;
                Vector3 desviation = perpendicular * oscillationWidth * wave;
                Vector3 newDirection = ((direction * .5f) + desviation).normalized;

                Vector3 destination;
                if (Physics.Raycast(eyePosition, newDirection, out RaycastHit hit, 1, BlockSight))
                    destination = hit.point;
                else
                    destination = eyePosition + newDirection;

                if (NavMesh.SamplePosition(destination, out NavMeshHit hit2, 4, NavMesh.AllAreas))
                    destination = hit2.position;

                if (!NavAgent.SetDestination(destination))
                {
                    if (NavMesh.SamplePosition(end, out hit2, 4, NavMesh.AllAreas))
                        end = hit2.position;
                    bool success = NavAgent.SetDestination(end);
                    Debug.Assert(success);
                }
            }
            else
            {
                if (NavMesh.SamplePosition(end, out NavMeshHit hit, 4, NavMesh.AllAreas))
                    end = hit.position;
                bool success = NavAgent.SetDestination(end);
                Debug.Assert(success);
            }
        }
        public void TakeDamage(float amount)
        {
            if (!IsAlive)
                return;

            Try.PlayOneShoot(transform, hurtSound, "hurt");

            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                OnDeath(false);
            }
            else
                OnTakeDamage(amount, false);
        }

        public void TakeDamageWeakSpot(float amount)
        {
            if (!IsAlive)
                return;

            Try.PlayOneShoot(transform, hurtWeakspotSound, "hurt weakspot");

            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                OnDeath(true);
            }
            else
                OnTakeDamage(amount, true);
        }

        protected bool HasPlayerInSight()
        {
            Vector3 eyePosition = EyePosition;
            if ((PlayerBody.Player.transform.position - eyePosition).sqrMagnitude < sightRadius
                && !Physics.Linecast(PlayerBody.Player.transform.position, eyePosition, BlockSight))
            {
                LastPlayerPosition = PlayerBody.Player.transform.position;
                return true;
            }
            return false;
        }

        protected void LookAtPlayer()
        {
            Vector3 lookPosition = LastPlayerPosition - transform.position;
            lookPosition.y = 0;
            transform.rotation = Quaternion.LookRotation(lookPosition);
        }

        protected virtual void OnTakeDamage(float amount, bool isOnWeakspot)
        {
            SetLastPlayerPosition();

            if (isOnWeakspot)
            {
                /*if (!Try.PlayOneShoot(transform, hurtWeakspotSound, "hurt weakspot"))
                    Try.PlayOneShoot(transform, hurtSound, "hurt");*/

                if (!TrySetAnimationTrigger(hurtWeakspotAnimationTrigger, "hurt weakspot", false))
                    TrySetAnimationTrigger(hurtAnimationTrigger, "hurt", false);
            }
            else
            {
                //Try.PlayOneShoot(transform, hurtSound, "hurt");
                TrySetAnimationTrigger(hurtAnimationTrigger, "hurt", false);
            }
        }

        public virtual void MakeAwareOfPlayerLocation() => SetLastPlayerPosition();

        protected void SetLastPlayerPosition() => LastPlayerPosition = PlayerBody.Player.transform.position;

        protected bool HasLightInRange()
        {
            Light light = Lantern.ActiveLight;
            if (light == null)
                return false;
            
            Lantern flashlight = Lantern.ActiveLantern;;
            if (flashlight == null)
                return false;
            
            
            Transform lightTranform = light.transform;
            
            Vector3 closestEnemyPoint = colliders.Aggregate(Tuple.Create(Vector3.zero, float.PositiveInfinity), (acum, item) =>
            {
                Vector3 closestPoint = item.ClosestPoint(Lantern.ActiveLantern.transform.position);
                float distance = Vector3.Distance(closestPoint, Lantern.ActiveLantern.transform.position);
                if (distance < acum.Item2)
                {
                    return Tuple.Create(closestPoint, distance);
                }

                return acum;
            }).Item1;


            Vector3 lightPosition = lightTranform.position;

            Vector3 lightDirection = closestEnemyPoint - lightPosition;
            float distanceToConeOrigin = lightDirection.sqrMagnitude;
            float range = flashlight.interactionRange;
            
            if (distanceToConeOrigin < range)
            {
                Vector3 coneDirection = lightTranform.forward;
                float angle = Vector3.Angle(coneDirection, lightDirection);
                if (angle < flashlight.interactionAngle)
                {
                    if (!Physics.Linecast(closestEnemyPoint, lightPosition, BlockSight))
                        return true;
                }
            }

            return false;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity animator.")]
        private void FromHurt() => TrySetLastAnimationTrigger();

        private void OnDeath(bool isOnWeakspot)
        {
            NavAgent.isStopped = true;
            NavAgent.enabled = false;

            foreach (Collider collider in GetComponentsInChildren<Collider>())
                collider.enabled = false;

            if (isOnWeakspot)
            {
                if (!Try.PlayOneShoot(transform, deathWeakspotSound, "death weakspot"))
                    Try.PlayOneShoot(transform, deathSound, "death");

                if (!TrySetAnimationTrigger(deathWeakspotAnimationTrigger, "death weakspot", true, true))
                    TrySetAnimationTrigger(deathAnimationTrigger, "death", true, true);
            }
            else
            {
                Try.PlayOneShoot(transform, deathSound, "death");
                TrySetAnimationTrigger(deathAnimationTrigger, "death", true, true);
            }
        }

        public void FromDeath() => Destroy(gameObject);

        protected bool TrySetAnimationTrigger(string triggerName, string triggerMetaName, bool recordAsLastAnimation = true, bool disableAllTriggers = false)
        {
            if (disableAllTriggers)
            {
                foreach (AnimatorControllerParameter parameter in Animator.parameters)
                {
                    if (parameter.type == AnimatorControllerParameterType.Trigger)
                        Animator.ResetTrigger(parameter.nameHash);
                }
            }

            if (Try.SetAnimationTrigger(Animator, triggerName, triggerMetaName))
            {
                if (recordAsLastAnimation)
                    LastAnimationTrigger = triggerName;
                return true;
            }
            else
            {
                LastAnimationTrigger = "";
                return false;
            }
        }

        protected void TrySetLastAnimationTrigger()
        {
            if (!string.IsNullOrEmpty(LastAnimationTrigger))
                Animator.SetTrigger(LastAnimationTrigger);
        }

        public void Blind()
        {
            SetLastPlayerPosition();

            if (state != State.Blinded)
            {
                SaveStateAsPrevious();
                state = State.Blinded;
            }

            IsInBlindAnimation = true;
            if (!TrySetAnimationTrigger(blindAnimationTrigger, "blind", false))
                FromBlind();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity animator.")]
        private void FromBlind()
        {
            IsInBlindAnimation = false;

            LoadPreviousState();

            TrySetLastAnimationTrigger();

            OnEndBlind();
        }

        protected abstract void OnEndBlind();

        protected void SaveStateAsPrevious() => previousState = state;

        protected void LoadPreviousState() => state = previousState;

#if UNITY_EDITOR
        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(EyePosition, SqrtOnPlay(sightRadius));
        }

        protected float SqrtOnPlay(float value) => Application.isPlaying ? Mathf.Sqrt(value) : value;
#endif
    }
}