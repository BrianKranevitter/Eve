using System;
using System.Collections.Generic;

using UnityEngine;

namespace Game.Player.Weapons
{
    public sealed class CumulativeSpreadModifier : WeaponModifier
    {
        [SerializeField, Min(0), Tooltip("Amount of spread gained per shoot.")]
        private float spreadGainShoot;

        [SerializeField, Min(0), Tooltip("Maximum spread that can be accumulated.")]
        private float maximumSpreadAccumulation;

        [SerializeField, Min(0), Tooltip("Start reducing spread after the specified seconds.")]
        private float spreadReductionWait;

        [SerializeField, Min(0), Tooltip("Spread reduced per second.")]
        private float spreadReducedPerSecond;

        [SerializeField, Tooltip("Container of the spread of this weapon.")]
        private CumulativeSpreadModifierContainer container;

#if UNITY_EDITOR
        private bool isInGizmos;
        protected override void IsInGizmos(bool isInGizmos) => this.isInGizmos = isInGizmos;
#endif

        public override void Modify(List<ShootInformation> information)
        {
            float spread = container.cumulativeSpread;
            float startReducingAt = spreadReductionWait +
#if UNITY_EDITOR
            container.lastNonGizmosShoot
#else
            container.lastShoot
#endif
            ;
            if (startReducingAt < Time.fixedTime)
            {
                spread -= (Time.fixedTime - startReducingAt) * spreadReducedPerSecond;
                spread = Math.Max(spread, 0);
            }

            container.lastShoot = Time.fixedTime;

#if UNITY_EDITOR
            if (!isInGizmos)
                container.lastNonGizmosShoot = Time.fixedTime;
#endif

            for (int i = 0; i < information.Count; i++)
            {
                ShootInformation info = information[i];
                info.Direction += UnityEngine.Random.insideUnitSphere * spread;
                info.Direction = info.Direction.normalized;
                information[i] = info;
            }

#if UNITY_EDITOR
            if (!isInGizmos)
                spread += Mathf.Min(spread + spreadGainShoot, maximumSpreadAccumulation);
#endif
            container.cumulativeSpread = spread;
        }
    }
}