using Enderlook.Unity.AudioManager;

using Game.Level;
using Game.Utility;

using UnityEngine;

namespace Game.Player.Weapons
{
    [RequireComponent(typeof(OutLineModifier))]
    public sealed class BatteryPickup : MonoBehaviour, IPickup, IInteractable
    {
        [SerializeField, Tooltip("Audio played on pickup.")]
        private AudioUnit audioOnPickup;

        private OutLineModifier outline;

        private float nextAudioWhenFullAt;

        private void Awake() => outline = GetComponent<OutLineModifier>();

        void IInteractable.Interact() => Pickup();

        public void Pickup()
        {
            Lantern.ActiveLantern.PickupBattery();

            Try.PlayOneShoot(transform.position, audioOnPickup, "on pickup");

            Destroy(gameObject);
        }

        void IInteractable.Highlight() => outline.Highlight();

        void IInteractable.Unhighlight() => outline.Unhighlight();

        void IInteractable.InSight() => outline.InSight();

        void IInteractable.OutOfSight() => outline.OutOfSight();
    }
}