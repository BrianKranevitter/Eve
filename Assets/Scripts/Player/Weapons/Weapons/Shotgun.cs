using Game.Utility;

using UnityEngine;

namespace Game.Player.Weapons
{
    public sealed class Shotgun : HitScanWeapon
    {
        [Header("Reload")]
        [SerializeField, Tooltip("Name of the reload animation trigger to keep reloading.")]
        private string keepReloadAnimationTrigger;

        [SerializeField, Tooltip("Name of the reload end animation trigger.")]
        private string endReloadAnimationTrigger;

        protected override void AfterReload() { }

        public void MidReload()
        {
            CurrentMagazineAmmo++;
            CurrentTotalAmmo--;
            if (CurrentTotalAmmo > 0 && CurrentMagazineAmmo < MaximumMagazineAmmo)
                Try.SetAnimationTrigger(Animator, keepReloadAnimationTrigger, "keep reload");
            else
                Try.SetAnimationTrigger(Animator, endReloadAnimationTrigger, "end reload");
        }
    }
}