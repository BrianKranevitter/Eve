using System.Collections;
using Game.Enemies;
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

        public static bool IsAlive { get; private set; }

        [SerializeField, Tooltip("Maximum amount of health the player has.")]
        private float health = 100;

        [Header("Setup")]
        [SerializeField, Tooltip("HUD to show health.")]
        private Text healthUI;

        [SerializeField, Tooltip("The animator trigger that goes to ShakeHard animation.")]
        private string shakeHardTrigger;

        [SerializeField, Tooltip("The animator trigger that goes to ShakeLight animation.")]
        private string shakeLightTrigger;

        [Header("Death")]
        [SerializeField, Tooltip("The model used when dying by crawler")]
        private GameObject playerRoot;
        
        [SerializeField, Tooltip("The model used when dying by crawler")]
        private GameObject dyingModel_Crawler;
        
        [SerializeField, Tooltip("Camera Animator")]
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

        [Header("Death")]
        [SerializeField]
        private GameObject alignmentCamera;
        
        [SerializeField]
        private GameObject alignmentBody;
        
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Awake()
        {
            currentHealth = health;
            hurtShaderController = GetComponent<HurtShaderController>();
            Player = this;

            IsAlive = true;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Update()
        {
            if (PauseMenu.Paused) return;

            if (lastValue != ((int)currentHealth, health))
            {
                lastValue = ((int)currentHealth, health);
                healthUI.text = $"{(int)currentHealth}/{health}";
            }
        }

        public void TakeDamage(float amount)
        {
            if (PauseMenu.Paused || !IsAlive) return;
            
            hurtShaderController.SetBlood(amount / 5); // We set the feedback accord in how much damage we took.
            
            currentHealth -= amount;

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                IsAlive = false;
                
                OnDeath();
            }
            else
            {
                if (amount.ToPercentageOfRange(0, health) < 10)
                    playerCameraAnimator.SetTrigger(shakeLightTrigger);
                else
                    playerCameraAnimator.SetTrigger(shakeHardTrigger);
            }
        }

        private void OnDeath()
        {
            dyingModel_Crawler.gameObject.SetActive(true);
            dyingModel_Crawler.transform.parent = null;
            playerRoot.gameObject.SetActive(false);

            foreach (var enemy in Enemy.allEnemies)
            {
                enemy.gameObject.SetActive(false);
            }
        }

        
    }
}
