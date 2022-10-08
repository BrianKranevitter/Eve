using Enderlook.Unity.AudioManager;

using Game.Level;
using Game.Utility;

using UnityEngine;

namespace Game.Player.Weapons
{
    [RequireComponent(typeof(OutLineModifier))]
    public sealed class AmmunitionPickup : MonoBehaviour, IPickup, IInteractable
    {
        [SerializeField, Tooltip("Name of ammunition type restored.")]
        private string ammunitionName;

        [SerializeField, Min(0), Tooltip("Amount of restored ammunition.")]
        private int amountRestored;

        [SerializeField, Tooltip("Audio played on pickup.")]
        private AudioUnit audioOnPickup;

        [SerializeField, Tooltip("Audio played on pickup try when full.")]
        private AudioUnit audioWhenFull;

        [SerializeField, Tooltip("Amount of seconds to wait before being able to play audio when full again.")]
        private float audioWhenFullCooldown;

        private OutLineModifier outline;

        private float nextAudioWhenFullAt;

        private void Awake() => outline = GetComponent<OutLineModifier>();

        void IInteractable.Interact() => Pickup();

        public void Pickup()
        {
            WeaponManager weaponManager = WeaponManager.Instance;

            AmmunitionType ammuntion = weaponManager.GetAmmunitionType(ammunitionName);

            if (ammuntion.CurrentAmmunition == ammuntion.MaximumStored)
            {
                if (Time.time >= nextAudioWhenFullAt)
                {
                    Try.PlayOneShoot(transform.position, audioWhenFull, "when full");
                    nextAudioWhenFullAt = Time.time + audioWhenFullCooldown;
                }
                return;
            }

            ammuntion.CurrentAmmunition = Mathf.Min(ammuntion.CurrentAmmunition + amountRestored, ammuntion.MaximumStored);

            weaponManager.ForceTotalAmmunitionUIUpdate();

            Try.PlayOneShoot(transform.position, audioOnPickup, "on pickup");

            Destroy(gameObject);
        }

        void IInteractable.Highlight() => outline.Highlight();

        void IInteractable.Unhighlight() => outline.Unhighlight();

        void IInteractable.InSight() => outline.InSight();

        void IInteractable.OutOfSight() => outline.OutOfSight();
    }
}