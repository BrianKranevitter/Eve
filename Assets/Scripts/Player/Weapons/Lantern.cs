using Enderlook.Unity.AudioManager;
using Enderlook.Unity.Toolset.Attributes;

using Game.Utility;

using UnityEngine;

namespace Game.Player.Weapons
{
    public sealed class Lantern : MonoBehaviour
    {
        public static Light ActiveLight { get; private set; }
        public static Lantern ActiveLantern { get; private set; }
        public static bool Active { get; private set; }

        [Header("Configuration")]
        
        [SerializeField, Tooltip("Key used to toggle light.")]
        private KeyCode lightKey = KeyCode.F;
        
        [SerializeField, Min(0), Tooltip("Duration of light in seconds.")]
        private float duration;
        
        [Min(0), Tooltip("Range at which the light starts interacting with things, such as enemies.")]
        public float interactionRange;
        [Min(0), Tooltip("Angle at which the light starts interacting with things, such as enemies.")]
        public float interactionAngle;

        public LanternType lanternType = LanternType.White;

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
        
        [SerializeField, ShowIf(nameof(haloLightRenderer), typeof(Renderer), null, false), Tooltip("Field of the shader used to set color of halo.")]
        private string haloLightColorFieldName;

        private new Light light;
        private Animator animator;

        
        [Header("Animator")]
        private float originalRange;
        private float originalIntensity;
        private float originalAngle;
        private float originalOpacity;

        public float animationOpacityMultiplier;
        public float animationRangeMultiplier;
        private bool turnedOff = false;

        private AmmunitionType ammunition;

        private Material batteryShader;
        private Material haloLightShader;

        private float currentDuration;

        private bool isInAnimation;

        public enum LanternType
        {
            Red, Blue, White
        }
        
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
            
            SetType(LanternType.White);

            if (Active)
            {
                SetOn();
            }
            else
            {
                SetOff();
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

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetType(LanternType.White);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetType(LanternType.Red);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SetType(LanternType.Blue);
            }

            if (Input.GetKeyDown(lightKey))
            {
                if (!Active)
                {
                    Active = true;
                    SetOn();
                }
                else
                {
                    Active = false;
                    SetOff();;
                }
                    
            }
            
            light.range = originalRange * animationRangeMultiplier;
            haloLightShader.SetFloat(haloLightOpacityFieldName, originalOpacity * animationOpacityMultiplier);
            
            if (light.intensity <= 0 || light.range <= 0 || light.spotAngle <= 0)
            {
                ActiveLight = null;
                ActiveLantern = null;
                light.enabled = false;
                if (haloLightRenderer != null)
                    haloLightRenderer.enabled = false;
            }
            else
            {
                ActiveLight = light;
                ActiveLantern = this;
                light.enabled = true;
                if (haloLightRenderer != null)
                    haloLightRenderer.enabled = true;
            }

            if (light.enabled)
            {
                if (currentDuration > 0)
                {
                    turnedOff = false;
                    currentDuration -= Time.deltaTime;

                    if (batteryShader != null && !string.IsNullOrEmpty(batteryPercentFieldName))
                        batteryShader.SetFloat(batteryPercentFieldName, Mathf.Pow(Mathf.Max(currentDuration, 0) / duration, batteryPercentFactor));
                }
                else
                {
                    if (!turnedOff)
                    {
                        if (!Try.SetAnimationTrigger(animator, outOfBatteryAnimationTrigger, "out of battery"))
                        {
                            SetOffImmediately();
                        }
                        else
                        {
                            turnedOff = true;
                        }
                    }
                    
                    Try.PlayOneShoot(transform, outOfBatterySound, "out of battery");
                }
            }
        }

        public void SetType(LanternType type)
        {
            if (light == null) return;

            lanternType = type;
            switch (type)
            {
                case LanternType.White: 
                    light.color = Color.white;
                    haloLightShader.SetColor(haloLightColorFieldName, Color.white);
                    break;
                case LanternType.Red:
                    light.color = Color.red;
                    haloLightShader.SetColor(haloLightColorFieldName, Color.red);
                    break;
                case LanternType.Blue:
                    light.color = Color.blue;
                    haloLightShader.SetColor(haloLightColorFieldName, Color.blue);
                    break;
                
                default:
                    SetType(LanternType.White);
                    break;
            }
            
            Debug.Log("Set light type to " + type);
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
            
            Debug.Log("SET OFF IMMEDIATELY");
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
