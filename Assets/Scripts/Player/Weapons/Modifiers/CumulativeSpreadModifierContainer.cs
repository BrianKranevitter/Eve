using System;

using UnityEngine;

namespace Game.Player.Weapons
{
    public sealed class CumulativeSpreadModifierContainer : ScriptableObject
    {
        [NonSerialized]
        public float cumulativeSpread;

        [NonSerialized]
        public float lastShoot;

#if UNITY_EDITOR
        [NonSerialized]
        public float lastNonGizmosShoot;
#endif
    }
}