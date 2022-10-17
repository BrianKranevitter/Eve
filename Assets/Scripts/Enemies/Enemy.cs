using Enderlook.Unity.AudioManager;
using Enderlook.Unity.Toolset.Attributes;

using Game.Player;
using Game.Player.Weapons;
using Game.Utility;

using UnityEngine;
using UnityEngine.AI;

namespace Game.Enemies
{
    [RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(Animator))]
    public abstract class Enemy : MonoBehaviour, IDamagable, IBlindable
    {
        [SerializeField, Min(0), Tooltip("Maximum health of enemy.")]
        private float maximumHealth;

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

            // Square values to avoid applying square root when checking distance.
            sightRadius *= sightRadius;

            GoToIdleState();
        }

        protected void GoToIdleState()
        {
            state = State.Idle;

            NavAgent.isStopped = true;

            TrySetAnimationTrigger(idleAnimationTrigger, "idle");
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

            Vector3 enemyPosition = EyePosition;
            Vector3 lightPosition = lightTranform.position;

            Vector3 lightDirection = enemyPosition - lightPosition;
            float distanceToConeOrigin = lightDirection.sqrMagnitude;
            float range = flashlight.interactionRange;
            
            if (distanceToConeOrigin < range)
            {
                Vector3 coneDirection = lightTranform.forward;
                float angle = Vector3.Angle(coneDirection, lightDirection);
                if (angle < flashlight.interactionAngle)
                {
                    if (!Physics.Linecast(enemyPosition, lightPosition, BlockSight))
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