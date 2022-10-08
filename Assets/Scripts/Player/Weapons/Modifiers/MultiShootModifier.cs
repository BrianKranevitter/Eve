using System;
using System.Collections.Generic;

using UnityEngine;

namespace Game.Player.Weapons
{
    public sealed class MultiShootModifier : WeaponModifier
    {
        [SerializeField, Min(0), Tooltip("Additional bullets launched per bullet shoot.")]
        private int additionalBulletsMultiplier;

        [SerializeField, Tooltip("Whenever additional bullets consumes ammunition or not.")]
        private bool consumesAmmunition;

        public override void Modify(List<ShootInformation> information)
        {
            int count = information.Count;
            for (int i = 0; i < count; i++)
            {
                ShootInformation info = information[i];
                for (int j = 0; j < additionalBulletsMultiplier; j++)
                {
                    info.RequiresAmmunition = consumesAmmunition;
                    information.Add(info);
                }
            }
        }
    }
}