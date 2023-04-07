using Enderlook.Unity.AudioManager;
using Enderlook.Unity.Toolset.Attributes;
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
        
        
        [Tooltip("Type of battery")]
        public BatteryType batteryType;

        [ShowIf(nameof(batteryType), typeof(BatteryType), BatteryType.FasterCooldown, true), 
         Tooltip("This value will be added to the recovery speed of the flashlight. Which itself is a multiplier of Time.deltaTime.")]
        public float addedRecoverySpeed;
        
        [ShowIf(nameof(batteryType), typeof(BatteryType), BatteryType.MoreHeat, true), 
         Tooltip("This value will be added to the total duration of the flashlight before its overheat.")]
        public float addedDuration;
        
        [ShowIf(nameof(batteryType), typeof(BatteryType), BatteryType.LessTurnOnCost, true), 
         Tooltip("This value will be subtracted to the turn on cost of the flashlight.")]
        public float subtractedTurnOnCost;
        
        
        
        private OutLineModifier outline;

        private float nextAudioWhenFullAt;

        
        public enum BatteryType
        {
            InstantCooldown, FasterCooldown, MoreHeat, LessTurnOnCost
        }
        
        private void Awake() => outline = GetComponent<OutLineModifier>();

        void IInteractable.Interact() => Pickup();

        public void Pickup()
        {
            Lantern.ActiveLantern.PickupBattery(this);

            Try.PlayOneShoot(transform.position, audioOnPickup, "on pickup");

            Destroy(gameObject);
        }

        void IInteractable.Highlight() => outline.Highlight();

        void IInteractable.Unhighlight() => outline.Unhighlight();

        void IInteractable.InSight() => outline.InSight();

        void IInteractable.OutOfSight() => outline.OutOfSight();
    }
}