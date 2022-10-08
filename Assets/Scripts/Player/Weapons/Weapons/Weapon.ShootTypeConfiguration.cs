using Enderlook.Unity.AudioManager;
using Enderlook.Unity.Toolset.Attributes;

using Game.Utility;

using System;

using UnityEngine;

namespace Game.Player.Weapons
{
    public abstract partial class Weapon
    {
        [Serializable]
        private struct ShootTypeConfiguration
        {
            [SerializeField, Min(0), Tooltip("Cooldown after shooting in seconds.")]
            private float cooldown;

            [SerializeField, Tooltip("Name of the shoot animation trigger.")]
            private string shootAnimationTrigger;

            [SerializeField, Tooltip("Name of the shake trigger in Camera.")]
            private string shakeAnimationTrigger;

            [SerializeField, Tooltip("Sound played when shooting.")]
            private AudioUnit shootSound;

            [SerializeField, Tooltip("Spawned particles in the cannon.")]
            private GameObject shootParticle;

            [field: SerializeField, IsProperty, Tooltip("Whenever the shoot button of this weapon can be held down.")]
            public bool CanBeHeldDown { get; private set; }

            public bool Shoot(WeaponManager manager, Animator animator, Transform soundTransform, Transform shootParticleTransform, out float canShootAt)
            {
                manager.TrySetAnimationTriggerOnCamera(shakeAnimationTrigger, "shake");

                if (Try.SetAnimationTrigger(animator, shootAnimationTrigger, "shoot animation"))
                    canShootAt = Time.deltaTime + cooldown;
                else
                    canShootAt = Time.deltaTime + cooldown + animator.GetCurrentAnimatorStateInfo(0).length;

                Try.PlayOneShoot(soundTransform.position, shootSound, "shoot");

                if (shootParticle != null)
                {
                    Vector3 position = shootParticleTransform.position;
                    ParticleSystemPool.GetOrInstantiate(shootParticle, position, Quaternion.identity, shootParticleTransform);
                }
                else
                    Debug.LogWarning("Missing shoot particle prefab.");

                return false;
            }
        }
    }
}