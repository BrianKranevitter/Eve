using Game.Enemies;
using Game.Utility;

using System;

using UnityEngine;

namespace Game.Player.Weapons
{
    public struct ShootInformation
    {
        public Vector3 Direction;
        public Action<IDamagable> OnShoot;
        public bool RequiresAmmunition;

        public ShootInformation(Vector3 direction)
        {
            Direction = direction;
            OnShoot = null;
            RequiresAmmunition = true;
        }

        public int ProcessShoot(
            float maximumDistance,
            ref ParticlesPerSurface particlesPerSurface,
            GameObject decalPrefab,
            int currentMagazineAmmo,
            Transform shootPoint,
            LayerMask colliderLayer,
            LayerMask decalObjectiveLayer
#if UNITY_EDITOR
            , GizmosShootLines gizmos
#endif
            )
        {
            if (RequiresAmmunition)
            {
                if (currentMagazineAmmo == 0)
                    return currentMagazineAmmo;
                else
                    currentMagazineAmmo--;
            }

#if UNITY_EDITOR
            gizmos.Add(shootPoint, Direction, maximumDistance, Color.magenta);
#endif

            if (Physics.Raycast(shootPoint.position, Direction, out RaycastHit hitInfo, maximumDistance, colliderLayer))
            {
                IDamagable damagable = hitInfo.transform.GetComponentInParent<IDamagable>();

                if (damagable == null)
                {
                    particlesPerSurface.OnOther(hitInfo.point, hitInfo.normal);
#if UNITY_EDITOR
                    gizmos.Add(shootPoint, hitInfo.point, Color.yellow);
#endif
                    if (Physics.Raycast(shootPoint.position, Direction, out RaycastHit hitInfoDecal, maximumDistance, decalObjectiveLayer))
                    {
                        if (decalPrefab == null)
                            Debug.LogWarning("Missing decal prefab.");
                        else
                        {
                            if (hitInfoDecal.collider is MeshCollider)
                            {
                                GameObject decal = UnityEngine.Object.Instantiate(decalPrefab);
                                decal.transform.position = hitInfoDecal.point;
                                decal.transform.forward = -hitInfoDecal.normal;

#if UNITY_EDITOR
                                gizmos.Add(shootPoint, hitInfoDecal.point, Color.white);
#endif
                            }
                        }
                    }
                }
                else
                {
                    if (damagable is WeakSpot)
                        particlesPerSurface.OnWeakspot(hitInfo.point, hitInfo.normal);
                    else
                        particlesPerSurface.OnBody(hitInfo.point, hitInfo.normal);

                    OnShoot(damagable);

#if UNITY_EDITOR
                    gizmos.Add(shootPoint, hitInfo.point, Color.red);
#endif
                }
            }

            return currentMagazineAmmo;
        }
    }
}