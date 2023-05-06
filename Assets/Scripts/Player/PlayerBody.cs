using System.Collections;
using System.Collections.Generic;
using Game.Enemies;
using Game.Utility;

using UnityEngine;
using UnityEngine.UI;

namespace Game.Player
{
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
        
        [SerializeField, Tooltip("Bracelet renderers")]
        private List<Renderer> braceletRenderers;

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
        private static readonly int HurtAmount = Shader.PropertyToID("_HurtAmount");

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
            
            hurtShaderController.SetBlood(amount); // We set the feedback accord in how much damage we took.
            
            currentHealth -= amount;

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                IsAlive = false;
                
                OnDeath();
            }
            else
            {
                float hpReduction = Mathf.Ceil(health * 0.25f);
                float hpRange = health - hpReduction;
                float currentHp = health - currentHealth;

                float hurtAmount = currentHp > hpRange ? 1 : currentHp / hpRange;
                
                float feedbackAmount = Mathf.Clamp(hurtAmount, 0, 1);

                foreach (var braceletRenderer in braceletRenderers)
                {
                    braceletRenderer.material.SetFloat(HurtAmount,feedbackAmount);
                }
                
                
                /*
                if (amount.ToPercentageOfRange(0, health) < 25)
                    playerCameraAnimator.SetTrigger(shakeLightTrigger);
                else
                    playerCameraAnimator.SetTrigger(shakeHardTrigger);*/
            }
        }

        public void HealDamage(float amount)
        {
            if (PauseMenu.Paused || !IsAlive) return;
            
            currentHealth += amount;

            if (currentHealth > health)
            {
                currentHealth = health;
            }
            
            float hpReduction = Mathf.Ceil(health * 0.25f);
            float hpRange = health - hpReduction;
            float currentHp = health - currentHealth;

            float hurtAmount = currentHp > hpRange ? 1 : currentHp / hpRange;
                
            float feedbackAmount = Mathf.Clamp(hurtAmount, 0, 1);

            foreach (var braceletRenderer in braceletRenderers)
            {
                braceletRenderer.material.SetFloat(HurtAmount,feedbackAmount);
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
