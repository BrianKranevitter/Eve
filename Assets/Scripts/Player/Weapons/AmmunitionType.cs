using System;

using UnityEngine;

namespace Game.Player.Weapons
{
    [Serializable]
    public sealed class AmmunitionType
    {
        [field: SerializeField, Tooltip("Name of the ammunition type.")]
        public string Name { get; private set; }

        [field: SerializeField, Min(0), Tooltip("Maximum ammunition that can be stored.")]
        public int MaximumStored { get; private set; }

        [SerializeField, Tooltip("Sets initial ammunition.")]
        public int CurrentAmmunition;
    }
}