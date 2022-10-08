using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Game.Player
{
    [RequireComponent(typeof(PlayerStamina)), DefaultExecutionOrder(1)]
    public sealed class FatigueForcer : MonoBehaviour
    {
        [SerializeField, Tooltip("Determines each how many seconds the effect is updated.")]
        private float timeBetweenUpdates = .5f;

        [SerializeField, Tooltip("Determines how fast explosion blur will grow down.")]
        private float explosionBlurGrowDownSpeed;

        [SerializeField, Tooltip("The panting sound.")]
        private AudioSource fatigueSound;

        private PlayerStamina stamina;
        private FatiguePPSSettings fatigueShader;

        private float nextUpdate;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Awake()
        {
            stamina = GetComponent<PlayerStamina>();
            GetComponentInChildren<PostProcessVolume>().profile.TryGetSettings(out fatigueShader);
            nextUpdate = Time.time + timeBetweenUpdates;

            if (fatigueShader == null)
                Debug.LogWarning("Missing fatigue post process.");
            else
                fatigueShader._GlobalIntensity.value = 1 - stamina.StaminaPercent;

            if (fatigueSound == null)
                Debug.Log("Missing fatigue sound.");
            else
                fatigueSound.volume = 1 - stamina.StaminaPercent;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Update()
        {
            if (Time.time >= nextUpdate)
            {
                nextUpdate = Time.time + timeBetweenUpdates;

                if (fatigueShader == null)
                    Debug.LogWarning("Missing fatigue post process.");
                else
                    fatigueShader._GlobalIntensity.value = 1 - stamina.StaminaPercent;
            }

            if (fatigueSound != null)
                fatigueSound.volume = 1 - stamina.StaminaPercent;

            ResetExplosionBlur();
        }

        public void Explosion(float value)
        {
            fatigueShader._ExplosionBlurIntensity.value += value;
        }

        void ResetExplosionBlur()
        {
            if (fatigueShader._ExplosionBlurIntensity.value > 0)
                fatigueShader._ExplosionBlurIntensity.value -= Time.deltaTime * explosionBlurGrowDownSpeed;
            else if (fatigueShader._ExplosionBlurIntensity.value < 0)
                fatigueShader._ExplosionBlurIntensity.value = 0;
        }
    }
}