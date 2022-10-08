using System.Collections.Generic;

using UnityEngine;

namespace Game.Player.Weapons
{
    public abstract class WeaponModifier : ScriptableObject
    {
        public abstract void Modify(List<ShootInformation> information);

        public static void ApplyModifiers(WeaponModifier[] modifiers, List<ShootInformation> information)
        {
            foreach (WeaponModifier modifier in modifiers)
                modifier.Modify(information);
        }

#if UNITY_EDITOR
        protected virtual void IsInGizmos(bool isInGizmos) { }

        public static void ApplyModifiersOnGizmos(WeaponModifier[] modifiers, List<ShootInformation> information)
        {
            foreach (WeaponModifier modifier in modifiers)
            {
                modifier.IsInGizmos(true);
                modifier.Modify(information);
                modifier.IsInGizmos(false);
            }
        }
#endif
    }
}