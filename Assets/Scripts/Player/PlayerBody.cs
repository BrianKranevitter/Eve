using Game.Utility;

using UnityEngine;
using UnityEngine.UI;

namespace Game.Player
{
    [RequireComponent(typeof(HurtShaderController))]
    public sealed class PlayerBody : MonoBehaviour, IDamagable
    {
        private static PlayerBody player;
        public static PlayerBody Player
        {
            get {
#if UNITY_EDITOR
                if (!Application.isPlaying && player == null)
                    player = FindObjectOfType<PlayerBody>();
#endif
                return player;
            }
            private set => player = value;
        }

        public static bool IsAlive => player.currentHealth > 0;

        [SerializeField, Tooltip("Maximum amount of health the player has.")]
        private float health = 100;

        [Header("Setup")]
        [SerializeField, Tooltip("HUD to show health.")]
        private Text healthUI;

        [SerializeField, Tooltip("The animator trigger that goes to ShakeHard animation.")]
        private string shakeHardTrigger;

        [SerializeField, Tooltip("The animator trigger that goes to ShakeLight animation.")]
        private string shakeLightTrigger;

        private Animator playerCameraAnimator;

        private HurtShaderController hurtShaderController;

        private float currentHealth;

        public float currentHp
        {
            get
            {
                return currentHealth;
            }
        }
        
        private (int, float) lastValue;

        [SerializeField, Tooltip("Point where the player appears when �Tp key` is pressed")]
        private Transform tpPoint;

        [SerializeField, Tooltip("Key to tp at new spawn point")]
        private KeyCode tpKey;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Awake()
        {
            currentHealth = health;
            hurtShaderController = GetComponent<HurtShaderController>();
            playerCameraAnimator = GetComponentInChildren<Animator>();
            Player = this;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Update()
        {
            if (PauseMenu.Paused) return;
            
            if (Input.GetKeyDown(tpKey))
                transform.position = tpPoint.position;

            if (lastValue != ((int)currentHealth, health))
            {
                lastValue = ((int)currentHealth, health);
                healthUI.text = $"{(int)currentHealth}/{health}";
            }
        }

        public void TakeDamage(float amount)
        {
            if (PauseMenu.Paused) return;
            
            hurtShaderController.SetBlood(amount / 5); // We set the feedback accord in how much damage we took.

            if (amount < 10)
                playerCameraAnimator.SetTrigger(shakeLightTrigger);
            else
                playerCameraAnimator.SetTrigger(shakeHardTrigger);

            if (currentHealth < amount)
            {
                if (currentHealth > 0) // If we are already dead we can't die again
                {
                    currentHealth = 0;
                    OnDeath();
                }
            }
            else
                currentHealth -= amount;
        }

        private void OnDeath()
        {
            Debug.LogWarning("TODO: Implement player death.");
        }
    }
}
