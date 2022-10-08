using Game.Utility;

using UnityEngine;

namespace Game.Enemies
{
    public sealed class WeakSpot : MonoBehaviour, IDamagable
    {
        [SerializeField, Min(0), Tooltip("Damage multiplier on this area.")]
        private float damageMultiplier = 1;

        private Enemy enemy;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Awake()
        {
            Transform parent = transform.parent;
            while (enemy == null)
            {
                if (parent == null)
                     Debug.LogError("Enemy script not found on parent hierarchy transform.");
                enemy = parent.GetComponent<Enemy>();
                parent = parent.parent;
            }
        }

        public void TakeDamage(float amount) => enemy.TakeDamageWeakSpot(amount * damageMultiplier);
    }
}