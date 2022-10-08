using Enderlook.Unity.AudioManager;

using Game.Level;
using Game.Utility;

using UnityEngine;

namespace Game.Player.Weapons
{
    [RequireComponent(typeof(OutLineModifier))]
    public sealed class WeaponPickup : MonoBehaviour, IPickup, IInteractable
    {
        [SerializeField, Tooltip("Name of weapon unlocked.")]
        private string weaponName;

        [SerializeField, Tooltip("Audio played on pickup.")]
        private AudioUnit audioOnPickup;

        private OutLineModifier outline;

        private void Awake() => outline = GetComponent<OutLineModifier>();

        void IInteractable.Interact() => Pickup();

        public void Pickup()
        {
            WeaponManager weaponManager = WeaponManager.Instance;

            weaponManager.UnlockWeapon(weaponName);

            Try.PlayOneShoot(transform.position, audioOnPickup, "on pickup");

            Destroy(gameObject);
        }

        void IInteractable.Highlight() => outline.Highlight();

        void IInteractable.Unhighlight() => outline.Unhighlight();

        void IInteractable.InSight() => outline.InSight();

        void IInteractable.OutOfSight() => outline.OutOfSight();
    }
}