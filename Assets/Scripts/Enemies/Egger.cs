using Enderlook.Unity.AudioManager;

using Game.Player;
using Game.Player.Weapons;
using Game.Utility;

using UnityEngine;
using UnityEngine.AI;

namespace Game.Enemies
{
    public sealed class Egger : Enemy
    {
        [Header("Sight")]
        [SerializeField, Min(0), Tooltip("Determines at which distance from player the creature start shooting.")]
        private float startShootingRadius = 2;

        [SerializeField, Min(0), Tooltip("Determines at which distance from player the creature stop shooting.")]
        private float stopShootingRadius = 3;

        [Header("Hurt Escape")]
        [SerializeField, Min(0), Tooltip("Duration in seconds that last panic after loosing line of sight with player.")]
        private float panicDuration = 2;

        [SerializeField, Min(0), Tooltip("Width of the oscillation when producing zig-zag.")]
        private float oscillationWidth = 1.5f;

        [SerializeField, Min(0), Tooltip("Frequency of the oscillation when producing zig-zag.")]
        private float oscillationFrequency = 3;

        [SerializeField, Min(0), Tooltip("Speed multiplier when escaping.")]
        private float escapingSpeedMultiplier = 1;

        [Header("Light Escape")]
        [SerializeField, Min(0), Tooltip("Duration in seconds that light sensibility last after getting out of light range.")]
        private float lightSensibilityDuration = 1;

        [SerializeField, Min(0), Tooltip("Duration in seconds that light sensibility can't be triggered after recoving from light.")]
        private float lightImmunityDuration = 1;

        [Header("Shoot")]
        [SerializeField, Tooltip("Projectile prefab shooted.")]
        private GameObject projectilePrefab;

        [SerializeField, Tooltip("Determines from which point projectiles are shooted.")]
        private Transform shootPosition;

        [SerializeField, Min(0), Tooltip("Amount of seconds of cooldown between each shoot.")]
        private float shootingCooldown = 1;

        [Header("Animation Triggers")]
        [SerializeField, Tooltip("Name of the animation trigger when is running towards the player to attack him.")]
        private string huntAnimationTrigger;

        [SerializeField, Tooltip("Name of the animation trigger when lose line of sight to player and is running to its last position.")]
        private string chaseAnimationTrigger;

        [SerializeField, Tooltip("Name of the animation trigger used to shoot (the animation must execute `Shoot()` event at some point and `FromShoot()` at the end).")]
        private string shootAnimationTrigger;

        [SerializeField, Tooltip("Name of the animation trigger when is running from the player to espace.")]
        private string escapeFromPlayerAnimationTrigger;

        [SerializeField, Tooltip("Name of the animation trigger when is running from the light to espace.")]
        private string escapeFromLightAnimationTrigger;

        [Header("Sounds")]
        [SerializeField, Tooltip("Sound played on shoot.")]
        private AudioFile shootSound;

        private float initialSpeed;

        private bool isInShootingAnimation;

        private float nextShoot;
        private float panicsEndsAt;
        private float lightImmunityEndsAt;

        private new class State : Enemy.State
        {
            public const byte HuntingPlayer = 10;
            public const byte ChasingPlayer = 11;
            public const byte ShootingToPlayer = 12;
            public const byte EscapingFromPlayer = 13;
            public const byte EscapingFromLight = 14;
        }

        protected override void Awake()
        {
            base.Awake();

            initialSpeed = NavAgent.speed;

            // Square values to avoid applying square root when checking distance.
            startShootingRadius *= startShootingRadius;
            stopShootingRadius *= stopShootingRadius;
        }

        private void FixedUpdate()
        {
            if (!IsAlive)
                return;

            switch (state)
            {
                case State.Idle:
                {
                    if (HasLightInRange() && TryGoToEscapeFromLightState())
                        break;
                    if (HasPlayerInSight())
                        GoToHuntState();
                    break;
                }
                case State.HuntingPlayer:
                {
                    if (HasLightInRange() && TryGoToEscapeFromLightState())
                        break;

                    if (!HasPlayerInSight())
                    {
                        GoToChaseState();
                        break;
                    }

                    bool success = NavAgent.SetDestination(LastPlayerPosition);
                    Debug.Assert(success);
                    if ((LastPlayerPosition - transform.position).sqrMagnitude <= startShootingRadius)
                        GoToShootState();
                    break;
                }
                case State.ChasingPlayer:
                {
                    if (HasLightInRange() && TryGoToEscapeFromLightState())
                        break;
                    if (HasPlayerInSight())
                    {
                        if ((LastPlayerPosition - transform.position).sqrMagnitude <= startShootingRadius)
                            GoToShootState();
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
                case State.ShootingToPlayer:
                {
                    if (isInShootingAnimation)
                        break;

                    if (HasLightInRange() && TryGoToEscapeFromLightState())
                        break;

                    if (!HasPlayerInSight())
                    {
                        GoToChaseState();
                        break;
                    }
                    else if ((LastPlayerPosition - transform.position).sqrMagnitude > stopShootingRadius)
                    {
                        GoToHuntState();
                        break;
                    }

                    LookAtPlayer();

                    if (Time.time >= nextShoot)
                        ToShoot();

                    break;
                }
                case State.EscapingFromPlayer:
                {
                    if (HasPlayerInSight())
                    {
                        panicsEndsAt = Time.fixedTime + panicDuration;
                        SetEscapeDestination(LastPlayerPosition);
                    }
                    else if (panicsEndsAt > Time.fixedTime)
                        SetEscapeDestination(LastPlayerPosition);
                    else
                        GoToIdleState();
                    break;
                }
                case State.EscapingFromLight:
                {
                    if (HasLightInRange())
                    {
                        panicsEndsAt = Time.fixedTime + lightSensibilityDuration;
                        SetEscapeDestination(LastPlayerPosition);
                    }
                    else if (panicsEndsAt > Time.fixedTime)
                        SetEscapeDestination(LastPlayerPosition);
                    else
                    {
                        lightImmunityEndsAt = Time.fixedTime + lightImmunityDuration;
                        GoToIdleState();
                    }
                    break;
                }
            }
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

        private bool TryGoToEscapeFromLightState()
        {
            if (Time.fixedTime < lightImmunityEndsAt)
                return false;

            state = State.EscapingFromLight;
            NavAgent.isStopped = false;
            NavAgent.speed = initialSpeed * escapingSpeedMultiplier;
            SetEscapeDestination(PlayerBody.Player.transform.position);

            TrySetAnimationTrigger(escapeFromLightAnimationTrigger, "escape from light");
            return true;
        }

        private void GoToEscapeFromPlayerState()
        {
            state = State.EscapingFromPlayer;
            NavAgent.isStopped = false;
            NavAgent.speed = initialSpeed * escapingSpeedMultiplier;
            SetEscapeDestination(PlayerBody.Player.transform.position);

            TrySetAnimationTrigger(escapeFromPlayerAnimationTrigger, "escape");
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

        private void GoToShootState()
        {
            state = State.ShootingToPlayer;
            NavAgent.isStopped = true;
            isInShootingAnimation = false; // Sometimes the flag may have a false positve if the animation was terminated abruptly.
        }

        private void ToShoot()
        {
            isInShootingAnimation = true;

            if (!TrySetAnimationTrigger(shootAnimationTrigger, "shoot"))
            {
                Shoot();
                FromShoot();
            }
        }

        private void Shoot()
        {
            GameObject projectile = Instantiate(projectilePrefab);
            projectile.transform.position = shootPosition.position;
            projectile.transform.LookAt(LastPlayerPosition);

            Try.PlayOneShoot(transform, shootSound, "shoot");
        }

        private void FromShoot()
        {
            isInShootingAnimation = false;
            nextShoot = Time.time + shootingCooldown;
        }

        private bool HasLightInRange()
        {
            Light light = Lantern.ActiveLight;
            if (light == null)
                return false;

            Transform lightTranform = light.transform;

            Vector3 enemyPosition = EyePosition;
            Vector3 lightPosition = lightTranform.position;

            Vector3 lightDirection = enemyPosition - lightPosition;
            float distanceToConeOrigin = lightDirection.sqrMagnitude;
            float range = light.range;
            if (distanceToConeOrigin < range * range)
            {
                Vector3 coneDirection = lightTranform.forward;
                float angle = Vector3.Angle(coneDirection, lightDirection);
                if (angle < light.spotAngle  * .5f)
                {
                    if (!Physics.Linecast(enemyPosition, lightPosition, BlockSight))
                        return true;
                }
            }

            return false;
        }

        protected override void OnTakeDamage(float amount, bool isOnWeakspot)
        {
            base.OnTakeDamage(amount, isOnWeakspot);
            GoToEscapeFromPlayerState();
        }

        private void SetEscapeDestination(Vector3 playerPosition)
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

        protected override void OnEndBlind()
        {
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

            float distance = float.NegativeInfinity;
            Vector3 end = default;
            bool canOscillate = default;

            Vector3 playerPosition = PlayerBody.Player.transform.position;
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
                if (Physics.Raycast(eyePosition, direction, out RaycastHit hit, BlockSight))
                {
                    Gizmos.color = oscillate ? Color.red : Color.yellow;
                    Gizmos.DrawLine(eyePosition, hit.point);

                    if (hit.distance > distance)
                    {
                        distance = hit.distance;
                        end = hit.point;
                        canOscillate = oscillate;
                    }
                }
                else
                {
                    Gizmos.color = oscillate ? Color.cyan : Color.blue;
                    Gizmos.DrawLine(eyePosition, eyePosition + direction);
                }
            }

            Gizmos.color = Color.green;
            Gizmos.DrawLine(eyePosition, end);

            if (canOscillate && oscillationFrequency != 0 && oscillationWidth != 0)
            {
                Gizmos.color = Color.magenta;
                Vector3 direction = (end - eyePosition).normalized;

                Vector3 perpendicular = new Vector3(direction.z, direction.y, direction.x);
                const int c = 5;
                for (int i = 0; i < c * 10; i++)
                {
                    Gizmos.DrawLine(Point(i), Point(i + 1));

                    Vector3 Point(float v)
                    {
                        v /= c;
                        float wave = Mathf.Sin((v + Time.fixedTime) * oscillationFrequency) * oscillationWidth;
                        Vector3 newDirection = perpendicular * oscillationWidth * wave;
                        return eyePosition + (direction * v) + newDirection;
                    }
                }

                float wave = Mathf.Sin(Time.fixedTime * oscillationFrequency) * oscillationWidth;
                Vector3 desviation = perpendicular * oscillationWidth * wave;
                Vector3 newDirection = (direction * .5f) + desviation;

                Vector3 destination;
                if (Physics.Raycast(eyePosition, newDirection, out RaycastHit hit, 1, BlockSight))
                    destination = hit.point;
                else
                    destination = eyePosition + newDirection;

                Gizmos.color = Color.gray;
                Gizmos.DrawLine(eyePosition, destination);
                if (NavMesh.SamplePosition(destination, out NavMeshHit hit2, 4, NavMesh.AllAreas))
                    destination = hit2.position;

                Gizmos.color = Color.black;
                Gizmos.DrawLine(eyePosition, destination);
            }
            else
            {
                if (NavMesh.SamplePosition(end, out NavMeshHit hit, 4, NavMesh.AllAreas))
                    end = hit.position;

                Gizmos.color = Color.gray;
                Gizmos.DrawLine(eyePosition, end);
            }

            if (NavAgent != null)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(transform.position, NavAgent.destination);
            }
        }

        private void OnValidate()
        {
            stopShootingRadius = Mathf.Min(sightRadius, stopShootingRadius);
            startShootingRadius = Mathf.Min(startShootingRadius, stopShootingRadius);
        }
#endif
    }
}