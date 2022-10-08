using System;

using UnityEngine;

namespace Game.Utility
{
    [Serializable]
    public struct ParticlesPerSurface
    {
        [SerializeField, Tooltip("If true, the camera point of view will be used instead of the provided normal for rotation and distance calculations.")]
        private bool lookAtCamera;

        [SerializeField, Tooltip("Particle spawn when colliding with a body.")]
        private GameObject onBody;

        [SerializeField, Min(0), Tooltip("Distance from collision point to spawn body particle.")]
        private float onBodyDistance;

        [SerializeField, Tooltip("Particle spawn when colliding with a weakspot.")]
        private GameObject onWeakspot;

        [SerializeField, Min(0), Tooltip("Distance from collision point to spawn weakspot particle.")]
        private float onWeakspotDistance;

        [SerializeField, Tooltip("Particle spawn when colliding with something that is not body nor weakspot.")]
        private GameObject onOther;

        [SerializeField, Min(0), Tooltip("Distance from collision point to spawn other particle.")]
        private float onOtherDistance;

        public void OnBody(Vector3 position, Vector3 normal)
            => Spawn(onBody, onBodyDistance, position, normal, "body");

        public void OnWeakspot(Vector3 position, Vector3 normal)
            => Spawn(onWeakspot, onWeakspotDistance, position, normal, "weakspot");

        public void OnOther(Vector3 position, Vector3 normal)
            => Spawn(onOther, onOtherDistance, position, normal, "other");

        private void Spawn(GameObject prefab, float distance, Vector3 position, Vector3 normal, string name)
        {
            if (prefab == null)
            {
                Debug.LogWarning($"Missing on {name} particle prefab.");
                return;
            }

            Vector3 polygonNormal = normal;
            normal = lookAtCamera ? (Camera.main.transform.position - position).normalized : normal;
            position += normal * distance;
            Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, normal);
            GameObject particle = ParticleSystemPool.GetOrInstantiate(prefab, position, rotation);
            particle.transform.forward = polygonNormal;
        }
    }
}