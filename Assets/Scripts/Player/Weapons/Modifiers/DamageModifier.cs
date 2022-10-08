using Game.Enemies;
using Game.Utility;

using System;
using System.Collections.Generic;

using UnityEngine;

namespace Game.Player.Weapons
{
    public sealed class DamageModifier : WeaponModifier
    {
        [SerializeField, Min(0), Tooltip("Amount of damage produced per projectile on body.")]
        private float damageBody;

        [SerializeField, Min(0), Tooltip("Amount of damage produced per projectile on weak spot.")]
        private float damageWeakSpot;

        private Action<IDamagable> action;

        public override void Modify(List<ShootInformation> information)
        {
            if (action is null)
                action = (damagable) => damagable.TakeDamage(damagable is WeakSpot ? damageWeakSpot : damageBody);

            for (int i = 0; i < information.Count; i++)
            {
                ShootInformation info = information[i];
                info.OnShoot += action;
                information[i] = info;
            }
        }
    }
}