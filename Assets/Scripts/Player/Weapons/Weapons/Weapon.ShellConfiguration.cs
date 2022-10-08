using System;

using UnityEngine;

using Random = UnityEngine.Random;

namespace Game.Player.Weapons
{
    public abstract partial class Weapon
    {
        [Serializable]
        private struct ShellConfiguration
        {
            [SerializeField, Tooltip("Prefab of the shell.")]
            private Rigidbody prefab;

            [SerializeField, Min(0), Tooltip("Minimal force applied to the shell when dropped.")]
            private float minForce;

            [SerializeField, Min(0), Tooltip("Maximum force applied to the shell when dropped.")]
            private float maxForce;

            [SerializeField, Min(0), Tooltip("Time in seconds to destroy the prefab.")]
            private float destroyIn;

            [SerializeField, Tooltip("Spawn point well shell is spawned.")]
            private Transform spawnPoint;

            public void Spawn()
            {
                if (prefab == null)
                {
                    if (spawnPoint == null)
                        return;

                    Debug.LogWarning("Missing shell prefab.");
                    return;
                }
                else if (spawnPoint == null)
                {
                    Debug.LogWarning("Missing spawn point of shell prefab.");
                    return;
                }

                Rigidbody rigidbody = Instantiate(prefab);
                rigidbody.transform.position = spawnPoint.position;
                rigidbody.transform.forward = new Vector3(Random.Range(0, 3.5f), Random.Range(0, 3.5f), Random.Range(0, 3.5f));
                rigidbody.AddForce(rigidbody.transform.forward * Random.Range(minForce, maxForce));
                Destroy(rigidbody.gameObject, destroyIn);
            }

#if UNITY_EDITOR
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
            private void OnValidate()
            {
                minForce = Mathf.Min(minForce, maxForce);
                maxForce = Mathf.Max(maxForce, minForce);
            }
#endif
        }
    }
}