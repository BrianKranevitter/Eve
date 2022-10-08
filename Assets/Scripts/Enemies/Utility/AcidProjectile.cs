using Game.Player;

using System.Collections;

using UnityEngine;

namespace Game.Enemies
{
    public sealed class AcidProjectile : MonoBehaviour
    {
        [SerializeField, Tooltip("Amount of damage per second realized to player.")]
        private float damagePerSecond = 5;

        [SerializeField, Tooltip("Amount of seconds the acid damage last.")]
        private float damageDuration = 2;

        [SerializeField, Tooltip("Ticks per second.")]
        private float ticksPerSecond = 2;

        [SerializeField, Tooltip("Movement speed of this projectile.")]
        private float speed = 5;

        [SerializeField, Tooltip("Destruction countdown of this object.")]
        private float destructionColdown = 15;

        private bool hitPlayer;
        private WaitForSeconds wait;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Awake()
        {
            Destroy(gameObject, destructionColdown);
            wait = new WaitForSeconds(1 / ticksPerSecond);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void FixedUpdate() => transform.position += transform.forward * speed * Time.fixedDeltaTime;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void OnTriggerEnter(Collider other)
        {
            if (hitPlayer)
                return;

            PlayerBody playerBody = other.gameObject.GetComponentInParent<PlayerBody>();

            if (playerBody == null)
                return;

            hitPlayer = true;

            StartCoroutine(Work());

            IEnumerator Work()
            {
                float damagePerTick = damagePerSecond / ticksPerSecond;
                int ticks = Mathf.CeilToInt(damageDuration * ticksPerSecond);

                for (int i = 0; i < ticks; i++)
                {
                    playerBody.TakeDamage(damagePerTick);
                    yield return wait;
                }
            }
        }
    }
}