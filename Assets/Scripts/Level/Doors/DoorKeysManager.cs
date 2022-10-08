using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Game.Level.Doors
{
    public sealed class DoorKeysManager : MonoBehaviour
    {
        private static DoorKeysManager instance;

        [SerializeField, Tooltip("Keys owned by the player")]
        private List<string> ownedKeys = new List<string>();

        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogError($"{nameof(DoorKeysManager)} is a singlenton.");
                Destroy(this);
                return;
            }

            instance = this;
        }

        public static bool HasKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                return true;
            return instance.ownedKeys.Contains(key);
        }

        public static void AddKey(string key)
        {
            Debug.Assert(!instance.ownedKeys.Contains(key), "Already contains that key.");
            instance.ownedKeys.Add(key);
        }

#if UNITY_EDITOR
        private void OnValidate() => ownedKeys = ownedKeys.Distinct().ToList();
#endif
    }
}