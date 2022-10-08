using System.Collections.Generic;

using UnityEngine;

namespace Game.Utility
{
    /// <summary>
    /// A pool for expirable objects with <see cref="ParticleSystem"/>(s).
    /// </summary>
    public static class ParticleSystemPool
    {
        private static int poolSize = 100;

        private static Dictionary<object, Stack<object>> pool = new Dictionary<object, Stack<object>>();

        public static int PoolSize {
            get => poolSize;
            set {
                if (poolSize == value)
                    return;
                poolSize = value;
                Dictionary<object, Stack<object>> newPool = new Dictionary<object, Stack<object>>(pool.Count);
                foreach (KeyValuePair<object, Stack<object>> kvp in pool)
                {
                    Stack<object> stack = new Stack<object>(poolSize);
                    for (int i = 0; i < poolSize; i++)
                    {
                        if (kvp.Value.TryPop(out object obj))
                            stack.Push(obj);
                    }
                    while (kvp.Value.TryPop(out object obj))
                    {
                        if (obj == null)
                            continue;

                        if (obj is GameObject g)
                            Object.Destroy(g);
                        else
                            Object.Destroy(((Component)obj).gameObject);
                    }
                    if (stack.Count > 0)
                        newPool.Add(kvp.Key, stack);
                }
                pool = newPool;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Purge()
        {
            Dictionary<object, Stack<object>> newPool = new Dictionary<object, Stack<object>>(pool.Count);
            Stack<object> stack = new Stack<object>(poolSize);
            foreach (KeyValuePair<object, Stack<object>> kvp in pool)
            {
                for (int i = 0; i < poolSize; i++)
                {
                    if (kvp.Value.TryPop(out object obj))
                        stack.Push(obj);
                }
                while (kvp.Value.TryPop(out object obj))
                {
                    if (obj == null)
                        continue;

                    if (obj is GameObject g)
                        Object.Destroy(g);
                    else
                        Object.Destroy(((Component)obj).gameObject);
                }
                if (stack.Count > 0)
                    newPool.Add(kvp.Key, stack);
                stack = kvp.Value;
                stack.Clear();
            }
        }

        /// <summary>
        /// Get an expirable object from the pool or instantiates a new one if the pool is empty.<br/>
        /// The object is automatically returned to pool on expire.
        /// </summary>
        /// <param name="prefab">Prefab of the object.</param>
        /// <returns>A pooled object.</returns>
        public static ParticleSystem GetOrInstantiate(ParticleSystem prefab)
        {
            if (pool.TryGetValue(prefab, out Stack<object> stack))
            {
                while (stack.TryPop(out object e))
                {
                    ParticleSystem t = (ParticleSystem)e;
                    if (t == null)
                        continue;

                    GameObject gameObject = t.gameObject;
                    gameObject.hideFlags = HideFlags.None;
                    gameObject.SetActive(true);
                    return t;
                }
            }

            ParticleSystem instance = Object.Instantiate(prefab);
            instance.gameObject.AddComponent<PoolReturner>().key = prefab;
            return instance;
        }

        /// <inheritdoc cref="GetOrInstantiate(ParticleSystem)"/>
        /// <param name="parent">Parent transform of the object.</param>
        public static ParticleSystem GetOrInstantiate(ParticleSystem prefab, Transform parent)
        {
            ParticleSystem instance = GetOrInstantiate(prefab);
            instance.transform.SetParent(parent);
            return instance;
        }

        /// <inheritdoc cref="GetOrInstantiate{T}(T)"/>
        /// <param name="position">Position of the object.</param>
        /// <param name="rotation">Rotation of the object.</param>
        public static ParticleSystem GetOrInstantiate(ParticleSystem prefab, Vector3 position, Quaternion rotation)
        {
            ParticleSystem instance = GetOrInstantiate(prefab);
            Transform transform = instance.transform;
            transform.position = position;
            transform.rotation = rotation;
            return instance;
        }

        /// <inheritdoc cref="GetOrInstantiate(ParticleSystem)"/>
        /// <param name="position">Position of the object.</param>
        /// <param name="rotation">Rotation of the object.</param>
        /// <param name="parent">Parent transform of the object.</param>
        public static ParticleSystem GetOrInstantiate(ParticleSystem prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            ParticleSystem instance = GetOrInstantiate(prefab);
            Transform transform = instance.transform;
            transform.SetParent(parent);
            transform.position = position;
            transform.rotation = rotation;
            return instance;
        }

        /// <summary>
        /// Get an expirable object from the pool or instantiates a new one if the pool is empty.<br/>
        /// The object is automatically returned to pool on expire.
        /// </summary>
        /// <param name="prefab">Prefab of the object.</param>
        /// <returns>A pooled object.</returns>
        public static GameObject GetOrInstantiate(GameObject prefab)
        {
            if (pool.TryGetValue(prefab, out Stack<object> stack))
            {
                while (stack.TryPop(out object e))
                {
                    GameObject gameObject = (GameObject)e;
                    if (gameObject == null)
                        continue;
                    gameObject.hideFlags = HideFlags.None;
                    gameObject.SetActive(true);
                    return gameObject;
                }
            }

            GameObject instance = Object.Instantiate(prefab);
            instance.gameObject.AddComponent<PoolReturner>().key = prefab;
            return instance;
        }

        /// <inheritdoc cref="GetOrInstantiate(GameObject)"/>
        /// <param name="parent">Parent transform of the object.</param>
        public static GameObject GetOrInstantiate(GameObject prefab, Transform parent)
        {
            GameObject instance = GetOrInstantiate(prefab);
            instance.transform.SetParent(parent);
            return instance;
        }

        /// <inheritdoc cref="GetOrInstantiate(GameObject)"/>
        /// <param name="position">Position of the object.</param>
        /// <param name="rotation">Rotation of the object.</param>
        public static GameObject GetOrInstantiate(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            GameObject instance = GetOrInstantiate(prefab);
            Transform transform = instance.transform;
            transform.position = position;
            transform.rotation = rotation;
            return instance;
        }

        /// <inheritdoc cref="GetOrInstantiate(GameObject)"/>
        /// <param name="position">Position of the object.</param>
        /// <param name="rotation">Rotation of the object.</param>
        /// <param name="parent">Parent transform of the object.</param>
        public static GameObject GetOrInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            GameObject instance = GetOrInstantiate(prefab);
            Transform transform = instance.transform;
            transform.SetParent(parent);
            transform.position = position;
            transform.rotation = rotation;
            return instance;
        }

        private static void ReturnToPool(object key, GameObject gameObject)
        {
            if (!pool.TryGetValue(key, out Stack<object> stack))
            {
                stack = new Stack<object>(poolSize);
                pool.Add(key, stack);
            }

            if (stack.Count < poolSize)
            {
                gameObject.hideFlags = HideFlags.HideInHierarchy;
                gameObject.SetActive(false);
                gameObject.transform.SetParent(null);
                stack.Push(gameObject);
            }
            else
                Object.Destroy(gameObject);
        }

        private sealed class PoolReturner : MonoBehaviour
        {
            public object key;
            private ParticleSystem[] particles;

            private void Awake()
            {
                particles = GetComponentsInChildren<ParticleSystem>();

                if (particles.Length == 0 )
                    Debug.LogError($"Expirable object doesn't contains any {nameof(ParticleSystem)}. Undefined behaviour.");
            }

            private void OnEnable()
            {
                foreach (ParticleSystem particle in particles)
                    if (particle.main.playOnAwake)
                        particle.Play(true);
            }

            private void Update()
            {
                foreach (ParticleSystem particle in particles)
                    if (particle.IsAlive(true))
                        return;
                ReturnToPool(key, gameObject);
            }
        }
    }
}