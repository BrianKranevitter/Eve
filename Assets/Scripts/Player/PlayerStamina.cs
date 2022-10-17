using UnityEngine;

namespace Game.Player
{
    public sealed class PlayerStamina : MonoBehaviour
    {
        [SerializeField, Tooltip("Maximum amount of stamina the player has.")]
        private float stamina = 100;

        [SerializeField, Tooltip("Amount of stamina recovered per second while resting.")]
        private float staminaFullRestPerSecond = 18;

        [SerializeField, Tooltip("Amounts of seconds to start full resting.")]
        private float staminaFullRestStartSeconds = .75f;

        [SerializeField, Tooltip("Amount of stamina recovered per second while not resting.")]
        private float staminaPartialRestPerSecond = 8;

        [SerializeField, Tooltip("Amounts of seconds to start partial resting.")]
        private float staminaPartialRestStartSeconds = 1.25f;

        [SerializeField, Tooltip("Minimum amount of stamina required to start running.")]
        private float minimumStaminaToRun = 40;

        [SerializeField, Tooltip("Amount of stamina consumed per second while running.")]
        private float runStaminaPerSecond = 16;

        private float currentStamina;
        private float startRestingOn;
        private RestType restType;

        public float StaminaPercent => currentStamina / stamina;

        private enum RestType
        {
            Idle,
            Walking,
            Running,
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Awake() => currentStamina = stamina;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void FixedUpdate()
        {
            if (PauseMenu.Paused) return;
            
            switch (restType)
            {
                case RestType.Idle:
                    if (startRestingOn <= Time.fixedTime)
                        currentStamina = Mathf.Min(currentStamina + (staminaFullRestPerSecond * Time.fixedDeltaTime), stamina);
                    break;
                case RestType.Walking:
                    if (startRestingOn <= Time.fixedTime)
                        currentStamina = Mathf.Min(currentStamina + (staminaPartialRestPerSecond * Time.fixedDeltaTime), stamina);
                    break;
                case RestType.Running:
                    currentStamina -= runStaminaPerSecond * Time.fixedDeltaTime;
                    if (currentStamina <= 0)
                    {
                        restType = RestType.Walking;
                        currentStamina = 0;
                    }
                    break;
            }
        }

        public bool TryRun()
        {
            if (restType == RestType.Running)
                return true;

            if (currentStamina < minimumStaminaToRun)
                return false;

            restType = RestType.Running;
            return true;
        }

        public void Walk()
        {
            if (restType != RestType.Walking)
            {
                restType = RestType.Walking;
                startRestingOn = Time.fixedTime + staminaPartialRestStartSeconds;
            }
        }

        public void Rest()
        {
            if (restType != RestType.Idle)
            {
                restType = RestType.Idle;
                startRestingOn = Time.fixedTime + staminaFullRestStartSeconds;
            }
        }
    }
}
