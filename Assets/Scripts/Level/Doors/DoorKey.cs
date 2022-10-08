using Enderlook.Unity.AudioManager;

using Game.Player;
using Game.Utility;

using UnityEngine;

namespace Game.Level.Doors
{
    [SerializeField, RequireComponent(typeof(OutLineModifier))]
    public sealed class DoorKey : MonoBehaviour, IPickup, IInteractable
    {
        [SerializeField, Tooltip("Name of key obtained.")]
        private string key;

        [SerializeField, Tooltip("Audio played on pickup.")]
        private AudioUnit audioOnPickup;

        private OutLineModifier outline;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Awake() => outline = GetComponent<OutLineModifier>();

        void IInteractable.Interact() => Pickup();

        public void Pickup()
        {
            DoorKeysManager.AddKey(key);

            if (audioOnPickup != null)
                AudioController.PlayOneShoot(audioOnPickup, transform.position);
            else
                Debug.LogWarning("Missing on pickup audio.");

            Destroy(gameObject);
        }

        void IInteractable.Highlight() => outline.Highlight();

        void IInteractable.Unhighlight() => outline.Unhighlight();

        void IInteractable.InSight() => outline.InSight();

        void IInteractable.OutOfSight() => outline.OutOfSight();
    }
}