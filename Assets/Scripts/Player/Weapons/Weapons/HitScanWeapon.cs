using Game.Utility;

using System.Collections.Generic;

using UnityEngine;

namespace Game.Player.Weapons
{
    [RequireComponent(typeof(Animator))]
    public class HitScanWeapon : Weapon
    {
        [Header("Setup")]
        [SerializeField, Tooltip("Layers that can collide with the projectile.")]
        private LayerMask colliderLayer;

        [SerializeField, Tooltip("The layer that decal raycast detects.")]
        private LayerMask decalObjectiveLayer;

        [Header("Primary Shoot")]
        [SerializeField, Min(0), Tooltip("Maximum distance that the projectile will travel.")]
        private float primaryMaximumDistance;

        [SerializeField, Tooltip("Spawned particles on collision.")]
        private ParticlesPerSurface primaryParticles;

        [SerializeField, Tooltip("Spawned decals on collision.")]
        private GameObject primaryDecal;

        [SerializeField, Tooltip("Shoot modifiers.")]
        private WeaponModifier[] primaryModifiers;

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

        protected override void ToPrimaryShoot()
        {
            Vector3 direction = GetShootRay().direction;

            shoots.Add(new ShootInformation(direction));
            WeaponModifier.ApplyModifiers(primaryModifiers, shoots);

            foreach (ShootInformation information in shoots)
                CurrentMagazineAmmo = information.ProcessShoot(
                    primaryMaximumDistance, ref primaryParticles, primaryDecal,
                    CurrentMagazineAmmo, GetShootPointTransform(), colliderLayer, decalObjectiveLayer
#if UNITY_EDITOR
                    , gizmos
#endif
                    );

            shoots.Clear();
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
            Gizmos.DrawLine(shootPosition, shootPosition + (direction * primaryMaximumDistance));

            Gizmos.color = Color.blue;
            shoots.Add(new ShootInformation(direction));
            WeaponModifier.ApplyModifiersOnGizmos(primaryModifiers, shoots);
            foreach (ShootInformation information in shoots)
                Gizmos.DrawLine(shootPosition, shootPosition + (information.Direction * primaryMaximumDistance));
            shoots.Clear();

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
