using Game.Utility;

using System.Collections.Generic;

using UnityEngine;

namespace Game.Player.Weapons
{
    [RequireComponent(typeof(Animator))]
    public sealed class PhotonCannon : Weapon
    {
        [Header("Setup")]
        [SerializeField, Tooltip("Layers that can collide with the projectile.")]
        private LayerMask colliderLayer;

        [SerializeField, Tooltip("The layer that decal raycast detects.")]
        private LayerMask decalObjectiveLayer;

        [Header("Primary Shoot")]
        [SerializeField, Min(0), Tooltip("Maximum distance that the projectile will travel.")]
        private float primaryMaximumDistance;

        [SerializeField, Range(0, 360), Tooltip("Angle of light.")]
        private float primaryAngle;

        [Header("Secondary Shoot")]
        [SerializeField, Min(0), Tooltip("Maximum distance that the projectile will travel.")]
        private float secondaryMaximumDistance;

        [SerializeField, Tooltip("Spawned particles on collision.")]
        private ParticlesPerSurface secondaryParticles;

        [SerializeField, Tooltip("Spawned decals on collision.")]
        private GameObject secondaryDecal;

        [SerializeField, Tooltip("Shoot modifiers.")]
        private WeaponModifier[] secondaryModifiers;

#if UNITY_EDITOR
        private GizmosShootLines gizmos = new GizmosShootLines();
#endif

        private List<ShootInformation> shoots = new List<ShootInformation>();
        private Collider[] colliders = new Collider[1];
        private HashSet<IBlindable> blindables = new HashSet<IBlindable>();

        protected override void ToPrimaryShoot()
        {
            CurrentMagazineAmmo--;

            Vector3 direction = GetShootRay().direction;
            Vector3 shootPosition = GetShootPointPosition();

            int count = GetCollidersInRange();
            for (int i = 0; i < count; i++)
            {
                if (GetBlindable(i, shootPosition, direction, out IBlindable blindable))
                    blindables.Add(blindable);
            }

            foreach (IBlindable blindable in blindables)
                blindable.Blind();

            blindables.Clear();
        }

        protected override void ToSecondaryShoot()
        {
            Vector3 direction = GetShootRay().direction;

            shoots.Add(new ShootInformation(direction));
            WeaponModifier.ApplyModifiers(secondaryModifiers, shoots);

            foreach (ShootInformation information in shoots)
                CurrentMagazineAmmo = information.ProcessShoot(
                   secondaryMaximumDistance, ref secondaryParticles, secondaryDecal,
                   CurrentMagazineAmmo, GetShootPointTransform(), colliderLayer, decalObjectiveLayer
#if UNITY_EDITOR
                    , gizmos
#endif
                    );

            shoots.Clear();
        }

        private int GetCollidersInRange()
        {
            Vector3 shootPosition = GetShootPointPosition();
            // TODO: An sphere cast may reduce the ammount of misses.
            // TODO: An specific layer mask only for enemies will drastically reduce amount of overlaped colliders.
            int count = Physics.OverlapSphereNonAlloc(shootPosition, primaryMaximumDistance, colliders, colliderLayer);
            if (count == colliders.Length)
            {
                colliders = Physics.OverlapSphere(shootPosition, primaryMaximumDistance, colliderLayer);
                count = colliders.Length;
            }
            return count;
        }

        private bool GetBlindable(int i, Vector3 shootPosition, Vector3 direction, out IBlindable blindable)
        {
            Collider collider = colliders[i];
            Vector3 position = GetClosestPoint(collider, shootPosition);
            float distanceToConeOrigin = (position - shootPosition).sqrMagnitude;
            float maximumDistance = primaryMaximumDistance * primaryMaximumDistance;
            if (distanceToConeOrigin < maximumDistance)
            {
                Vector3 pointDirection = position - shootPosition;
                float angle = Vector3.Angle(direction, pointDirection);
                if (angle < primaryAngle * .5f)
                {
                    blindable = collider.gameObject.GetComponentInParent<IBlindable>();
                    if (blindable != null)
                    {
                        Vector3 directionToCollider = (position - shootPosition).normalized;
                        if (Physics.Raycast(shootPosition, directionToCollider, out RaycastHit hit, primaryMaximumDistance, colliderLayer))
                        {
                            if (hit.collider == collider)
                                return true;
                        }
                        else
                            return true;
                    }
                }
            }
            blindable = default;
            return false;
        }

        private static Vector3 GetClosestPoint(Collider collider, Vector3 point)
        {
            if (collider is MeshCollider meshCollider && !meshCollider.convex)
                return collider.transform.position;
            return collider.ClosestPoint(point);
        }

#if UNITY_EDITOR
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void OnDrawGizmos()
        {
            gizmos.OnDrawGizmos();

            if (!IsReady)
                return;

            Vector3 shootPosition = GetShootPointPosition();
            Vector3 direction = GetShootRay().direction;

            Gizmos.color = Color.white;
            Vector3 endPosition = shootPosition + (direction * primaryMaximumDistance);
            Gizmos.DrawLine(shootPosition, endPosition);

            // https://answers.unity.com/questions/21176/gizmo-question-how-do-i-create-a-field-of-view-usi.html
            float halfAngle = primaryAngle * .5f;

            Quaternion leftRayRotation = Quaternion.AngleAxis(-halfAngle, Vector3.up);
            Vector3 leftRayDirection = leftRayRotation * direction;
            Gizmos.DrawRay(shootPosition, leftRayDirection * primaryMaximumDistance);

            Quaternion rightRayRotation = Quaternion.AngleAxis(halfAngle, Vector3.up);
            Vector3 rightRayDirection = rightRayRotation * direction;
            Gizmos.DrawRay(shootPosition, rightRayDirection * primaryMaximumDistance);

            Quaternion upRayRotation = Quaternion.AngleAxis(-halfAngle, Vector3.right);
            Vector3 upRayDirection = upRayRotation * direction;
            Gizmos.DrawRay(shootPosition, upRayDirection * primaryMaximumDistance);

            Quaternion downRayRotation = Quaternion.AngleAxis(-halfAngle, Vector3.left);
            Vector3 downRayDirection = downRayRotation * direction;
            Gizmos.DrawRay(shootPosition, downRayDirection * primaryMaximumDistance);

            int count = GetCollidersInRange();

            Gizmos.color = Color.blue;
            for (int i = 0; i < count; i++)
            {
                if (GetBlindable(i, shootPosition, direction, out IBlindable _))
                    Gizmos.DrawLine(shootPosition, GetClosestPoint(colliders[i], shootPosition));
            }

            Gizmos.color = Color.gray;
            Gizmos.DrawLine(shootPosition, shootPosition + (direction * secondaryMaximumDistance));

            Gizmos.color = Color.cyan;
            shoots.Add(new ShootInformation(direction));
            WeaponModifier.ApplyModifiersOnGizmos(secondaryModifiers, shoots);
            foreach (ShootInformation information in shoots)
                Gizmos.DrawLine(shootPosition, shootPosition + (information.Direction * secondaryMaximumDistance));
            shoots.Clear();
        }
#endif
    }
}