using Game.Utility;

using System;

using UnityEngine;

namespace Game.Level.Triggers
{
    [Serializable]
    public sealed class PlayParticleSystemPlayerTriggerAction : PlayerTriggerAction
    {
        [SerializeField, Tooltip("Particles to spawn.")]
        private Pack[] particles;

        private GameObject[] stored;

        [Serializable]
        private struct Pack
        {
            [Tooltip("Particle system to instantiate.")]
            public GameObject prefab;

            [Tooltip("Center position where sound will be played.")]
            public Transform center;

            [Tooltip("If true, the particle system will be automatically extracted from a pool and returned when the particles ends.")]
            public bool pool;

            [Tooltip("If true, the particle system will stop when player get out of trigger.")]
            public bool stopWhenGetOut;
        }

        public override void OnTriggerEnter()
        {
            if (stored is null)
                stored = new GameObject[particles.Length];

            for (int i = 0; i < particles.Length; i++)
            {
                Pack pack = particles[i];

                if (pack.prefab == null)
                {
                    Debug.LogWarning("Missing prefab.");
                    continue;
                }

                if (pack.center == null)
                {
                    Debug.LogWarning("Missing center");
                    continue;
                }

                GameObject gameObject;
                if (pack.pool)
                    gameObject = ParticleSystemPool.GetOrInstantiate(pack.prefab);
                else
                    gameObject = UnityEngine.Object.Instantiate(pack.prefab);
                gameObject.transform.position = pack.center.position;

                stored[i] = gameObject;
            }
        }

        public override void OnTriggerExit()
        {
            for (int i = 0; i < particles.Length; i++)
            {
                Pack pack = particles[i];

                if (pack.prefab == null)
                    continue;

                if (pack.center == null)
                    continue;

                GameObject gameObject = stored[i];
                if (gameObject != null)
                    foreach (ParticleSystem particleSystem in gameObject.GetComponentsInChildren<ParticleSystem>())
                        particleSystem.Stop(true);
                stored[i] = default;
            }
        }
    }
}