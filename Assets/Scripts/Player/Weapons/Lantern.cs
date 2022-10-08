using Enderlook.Unity.AudioManager;
using Enderlook.Unity.Toolset.Attributes;

using Game.Utility;

using UnityEngine;

namespace Game.Player.Weapons
{
    public sealed class Lantern : MonoBehaviour
    {
        public static Light ActiveLight { get; private set; }

        [Header("Configuration")]
        [SerializeField, Min(0), Tooltip("Duration of light in seconds.")]
        private float duration;

        [Header("Animation Triggers")]
        [SerializeField, Tooltip("Animation trigger when replacing batteries. (The animation must execute FromReloadLantern() method.)")]
        private string reloadAnimationTrigger;

        [SerializeField, Tooltip("Animation trigger when run out of battery.")]
        private string outOfBatteryAnimationTrigger;

        [SerializeField, Tooltip("Animation trigger when turn on lantern.")]
        private string turnOnAnimationTrigger;

        [SerializeField, Tooltip("Animation trigger when turn off lantern.")]
        private string turnOffAnimationTrigger;

        [SerializeField, Tooltip("Name of the ammunition type used.")]
        private string ammunitionName;

        [Header("Sound")]
        [SerializeField, Tooltip("Sound played when replacing batteries.")]
        private AudioFile reloadSound;

        [SerializeField, Tooltip("Sound played when running out of battery.")]
        private AudioFile outOfBatterySound;

        [SerializeField, Tooltip("Sound played when turn on lantern.")]
        private AudioFile turnOnSound;

        [SerializeField, Tooltip("Sound played when turn off lantern.")]
        private AudioFile turnOffSound;

        [Header("Shader")]
        [SerializeField, Tooltip("Object with the material that presents lantern feedback.")]
        private Renderer objectWithLanternRenderer;

        [SerializeField, ShowIf(nameof(objectWithLanternRenderer), typeof(Renderer), null, false), Tooltip("Field of the shader used to set battery percent.")]
        private string batteryPercentFieldName;

        [SerializeField, ShowIf(nameof(batteryPercentFieldName), typeof(string), null, false), Min(0.001f), Tooltip("Factor at which battery percent is reduced.")]
        private float batteryPercentFactor = 1;

        [SerializeField, Tooltip("The object with the material that has the halo light effect.")]
        private Renderer haloLightRenderer;

        [SerializeField, ShowIf(nameof(haloLightRenderer), typeof(Renderer), null, false), Tooltip("Field of the shader used to set opacity of halo.")]
        private string haloLightOpacityFieldName;

        private new Light light;
        private Animator animator;

        private float originalRange;
        private float originalIntensity;
        private float originalAngle;
        private float originalOpacity;

        private AmmunitionType ammunition;

        private Material batteryShader;
        private Material haloLightShader;

        private float currentDuration;

        private bool isInAnimation;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Awake()
        {
            light = GetComponentInChildren<Light>();
            if (light == null)
                Debug.LogError("Missing Light component in object or children.");
            animator = GetComponent<Animator>();

            light.enabled = false;
            originalRange = light.range;
            originalIntensity = light.intensity;
            originalAngle = light.spotAngle;

            currentDuration = duration;

            if (objectWithLanternRenderer != null)
            {
                batteryShader = objectWithLanternRenderer.material;
                if (batteryShader == null)
                    Debug.LogWarning("Object with lantern doesn't have material.");
                else if (string.IsNullOrEmpty(batteryPercentFieldName))
                    Debug.LogWarning("Missing batery percent opacity field name.");
            }

            if (haloLightRenderer != null)
            {
                haloLightRenderer.enabled = false;
                haloLightShader = haloLightRenderer.material;
                if (haloLightShader == null)
                    Debug.LogWarning("Halo light doesn't have material.");
                else if (string.IsNullOrEmpty(haloLightOpacityFieldName))
                    Debug.LogWarning("Missing halo light opacity field name.");
            }
        }

        public void Initialize(WeaponManager manager)
        {
            ammunition = manager.GetAmmunitionType(ammunitionName);
            SetOffImmediately();
        }

        private void Update()
        {
            if (light == null)
                return;

            if (light.intensity <= 0 || light.range <= 0 || light.spotAngle <= 0)
            {
                ActiveLight = null;
                light.enabled = false;
                if (haloLightRenderer != null)
                    haloLightRenderer.enabled = false;
            }
            else
            {
                ActiveLight = light;
                light.enabled = true;
                if (haloLightRenderer != null)
                    haloLightRenderer.enabled = true;
            }

            if (light.enabled)
            {
                if (currentDuration > 0)
                {
                    currentDuration -= Time.deltaTime;

                    if (batteryShader != null && !string.IsNullOrEmpty(batteryPercentFieldName))
                        batteryShader.SetFloat(batteryPercentFieldName, Mathf.Pow(Mathf.Max(currentDuration, 0) / duration, batteryPercentFactor));
                }
                else
                {
                    if (!Try.SetAnimationTrigger(animator, outOfBatteryAnimationTrigger, "out of battery"))
                        SetOffImmediately();
                    Try.PlayOneShoot(transform, outOfBatterySound, "out of battery");
                }
            }
        }

        public void SetOnImmediately()
        {
            if (light == null)
                return;

            if (currentDuration <= 0)
                return;

            light.intensity = originalIntensity;
            light.range = originalRange;
            light.spotAngle = originalAngle;
            light.enabled = true;

            haloLightShader.SetFloat(haloLightOpacityFieldName, originalOpacity);
            haloLightRenderer.enabled = true;
        }

        public void SetOn()
        {
            if (isInAnimation)
                return;

            if (currentDuration <= 0)
            {
                TryReload();
                return;
            }

            if (!Try.SetAnimationTrigger(animator, turnOnAnimationTrigger, "turn on"))
                SetOnImmediately();
            Try.PlayOneShoot(transform, turnOnSound, "turn on");
        }

        public void SetOffImmediately()
        {
            if (light == null)
                return;

            isInAnimation = false;

            light.intensity = 0;
            light.range = 0;
            light.spotAngle = 0;
            light.enabled = false;

            haloLightShader.SetFloat(haloLightOpacityFieldName, 0);
            haloLightRenderer.enabled = false;
        }

        public void SetOff()
        {
            if (!Try.SetAnimationTrigger(animator, turnOnAnimationTrigger, "turn off"))
                SetOffImmediately();
            Try.PlayOneShoot(transform, turnOffSound, "turn off");
        }

        private void TryReload()
        {
            if (ammunition.CurrentAmmunition == 0)
                return;

            isInAnimation = true;

            Try.PlayOneShoot(transform, reloadSound, "reload");
            if (!Try.SetAnimationTrigger(animator, reloadAnimationTrigger, "reload"))
            {
                FromReloadLantern();
                SetOnImmediately();
            }
        }

        private void FromReloadLantern()
        {
            isInAnimation = false;
            ammunition.CurrentAmmunition--;
            animator.ResetTrigger("TurnOff");
            currentDuration = duration;
        }
    }
}
