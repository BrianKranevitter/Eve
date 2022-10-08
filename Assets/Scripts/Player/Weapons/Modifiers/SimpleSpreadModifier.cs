using System.Collections.Generic;

using UnityEngine;

namespace Game.Player.Weapons
{
    public sealed class SimpleSpreadModifier : WeaponModifier
    {
        [SerializeField, Min(0), Tooltip("Determines the spread of the shoot.")]
        private float spread;

        public override void Modify(List<ShootInformation> information)
        {
            for (int i = 0; i < information.Count; i++)
            {
                ShootInformation info = information[i];
                info.Direction += UnityEngine.Random.insideUnitSphere * spread;
                info.Direction = info.Direction.normalized;
                information[i] = info;
            }
        }
    }
}