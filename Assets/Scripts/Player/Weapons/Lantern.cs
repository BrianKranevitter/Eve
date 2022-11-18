using System;
using System.Collections.Generic;
using Enderlook.Unity.AudioManager;
using Enderlook.Unity.Toolset.Attributes;

using Game.Utility;

using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Game.Player.Weapons
{
    public sealed class Lantern : MonoBehaviour
    {
        [SerializeField]
        Animator handAnimator;

        public static Light ActiveLight { get; private set; }
        public static Lantern ActiveLantern { get; private set; }
        public static bool Active { get; private set; }

        [Header("Configuration")]
        
        [SerializeField, Tooltip("Key used to toggle light.")]
        private KeyCode lightKey = KeyCode.F;
        
        [SerializeField, Min(0), Tooltip("Duration of light in seconds.")]
        private float duration;
        
        [FormerlySerializedAs("interactionRange")] [Min(0), Tooltip("Minimum Range at which the light starts interacting with things, such as enemies, at CLOSE range.")]
        public float interactionRange_Close;
        
        [Min(0), Tooltip("Maximum range at which the light starts interacting with things, such as enemies, at the FAR range.")]
        public float interactionRange_Far;
        
        [Min(0), Tooltip("Angle at which the light starts interacting with things, such as enemies.")]
        public float interactionAngle;

        public LightType lightType = LightType.White;
        private List<LightType> lightList = new List<LightType> {LightType.White};
        private int currentIndex = 0;

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
        private AudioUnit reloadSound;

        [SerializeField, Tooltip("Sound played when running out of battery.")]
        private AudioUnit outOfBatterySound;

        [SerializeField, Tooltip("Sound played when turn on lantern.")]
        private AudioUnit turnOnSound;

        [SerializeField, Tooltip("Sound played when turn off lantern.")]
        private AudioUnit turnOffSound;
        
        [SerializeField, ShowIf(nameof(objectWithLanternRenderer), typeof(Renderer), null, false), Tooltip("Sound that will be played on low battery.")]
        private AudioUnit batteryLowLevelSound;

        [Header("Shader")]
        [SerializeField, Tooltip("Object with the material that presents lantern feedback.")]
        private Renderer objectWithLanternRenderer;

        [SerializeField, ShowIf(nameof(objectWithLanternRenderer), typeof(Renderer), null, false), Tooltip("Field of the shader used to set battery percent.")]
        private string batteryPercentFieldName;
        
        [SerializeField, ShowIf(nameof(objectWithLanternRenderer), typeof(Renderer), null, false), Tooltip("Field of the shader used to set battery color.")]
        private string batteryColorFieldName;
        
        [Range(0,100)]
        [SerializeField, ShowIf(nameof(objectWithLanternRenderer), typeof(Renderer), null, false), Tooltip("Threshhold of batteryLevel used to set low battery.")]
        private float batteryLowLevelPercentThreshhold;
        
        [SerializeField, ShowIf(nameof(objectWithLanternRenderer), typeof(Renderer), null, false), Tooltip("Field of the shader used to set battery low level.")]
        private string batteryLowLevelFieldName;
        
        

        private bool playedSound = false;
        
        [SerializeField, ShowIf(nameof(objectWithLanternRenderer), typeof(Renderer), null, false), Tooltip("Field of the shader used to set battery percent.")]
        private Color batteryWhiteColor;
        [SerializeField, ShowIf(nameof(objectWithLanternRenderer), typeof(Renderer), null, false), Tooltip("Field of the shader used to set battery percent.")]
        private Color batteryRedColor;
        [SerializeField, ShowIf(nameof(objectWithLanternRenderer), typeof(Renderer), null, false), Tooltip("Field of the shader used to set battery percent.")]
        private Color batteryBlueColor;
        

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
        private bool outOfBattery = false;

        private Material batteryShader;
        private Material haloLightShader;

        private float currentDuration;
        public int batteryAmount;

        private bool isInAnimation;

        public enum LightType
        {
            Red, Blue, White
        }

        public enum DistanceEffect
        {
            Close, Far, None
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
            
            SetType(LightType.White);

            if (Active)
            {
                SetOn();
            }
            else
            {
                SetOff();
            }
            
            ActiveLantern = this;
        }

        public void Initialize()
        {
            SetOffImmediately();
        }

        private void Update()
        {
            if (light == null)
                return;

            if (Input.GetKeyDown(lightKey))
            {
                if (outOfBattery)
                {
                    Debug.Log("Active: " + Active);
                }
                if (!outOfBattery || batteryAmount > 0)
                {
                    if (!Active)
                    {
                        SetOn();
                    }
                    else
                    {
                        SetOff();;
                    }
                }
            }

            if (lightList.Count > 1)
            {
                if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse))
                {
                    handAnimator.SetTrigger("GoUp");
                }
                else if (Input.GetMouseButtonDown((int)MouseButton.RightMouse))
                {
                    handAnimator.SetTrigger("GoDown");
                }
            }

            light.range = originalRange * animationRangeMultiplier;
            haloLightShader.SetFloat(haloLightOpacityFieldName, originalOpacity * animationOpacityMultiplier);
            
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

            float batteryPercent = Mathf.Max(currentDuration, 0) / duration;
            if (batteryShader != null)
            {
                if (!string.IsNullOrEmpty(batteryPercentFieldName))
                    batteryShader.SetFloat(batteryPercentFieldName, Mathf.Ceil(batteryPercent * 10f) / 10f);

                if (!string.IsNullOrEmpty(batteryLowLevelFieldName))
                {
                    if (batteryPercent < (batteryLowLevelPercentThreshhold / 100))
                    {
                        float speedScale = 3;
                        float sine = Mathf.Abs(Mathf.Sin((Time.time * speedScale)));
                        batteryShader.SetFloat(batteryLowLevelFieldName, sine);

                        if (light.enabled && !outOfBattery)
                        {
                            if (Math.Abs(sine - 1) < 0.1f && !playedSound)
                            {
                                //play sound once.
                                playedSound = true;
                                KamAudioManager.instance.PlaySFX_AudioUnit(batteryLowLevelSound, transform.position);
                            }
                            else if(sine < 0.1f && playedSound)
                            {
                                playedSound = false;
                            }
                        }
                    }
                    else
                    {
                        batteryShader.SetFloat(batteryLowLevelFieldName, 0);
                    }
                    
                }
                    
            }




            if (light.enabled)
            {
                if (currentDuration > 0)
                {
                    outOfBattery = false;
                    currentDuration -= Time.deltaTime;
                    
                    if (currentDuration < 0)
                    {
                        outOfBattery = true;
                        Active = false;
                        if (!Try.SetAnimationTrigger(animator, outOfBatteryAnimationTrigger, "out of battery"))
                        {
                            SetOffImmediately();
                        }

                        Try.PlayOneShoot(transform, outOfBatterySound, "out of battery");
                    }
                }
            }
        }

        public void SetType(LightType type)
        {
            if (light == null) return;

            lightType = type;
            switch (type)
            {
                case LightType.White: 
                    light.color = Color.white;
                    haloLightShader.SetColor(haloLightColorFieldName, Color.white);
                    batteryShader.SetColor(batteryColorFieldName, batteryWhiteColor);
                    break;
                case LightType.Red:
                    light.color = Color.red;
                    haloLightShader.SetColor(haloLightColorFieldName, Color.red);
                    batteryShader.SetColor(batteryColorFieldName, batteryRedColor);
                    break;
                case LightType.Blue:
                    light.color = Color.blue;
                    haloLightShader.SetColor(haloLightColorFieldName, Color.blue);
                    batteryShader.SetColor(batteryColorFieldName, batteryBlueColor);
                    break;
                
                default:
                    SetType(LightType.White);
                    break;
            }
        }

        public void UnlockType(LightType type)
        {
            if (!lightList.Contains(type))
            {
                lightList.Add(type);
            }
        }

        public void ScrollUp()
        {
            currentIndex++;
            if (currentIndex > lightList.Count - 1)
            {
                currentIndex = 0;
            }
            SetType(lightList[currentIndex]);
        }

        public void ScrollDown()
        {
            currentIndex--;
            if (currentIndex < 0)
            {
                currentIndex = lightList.Count - 1;
            }
            SetType(lightList[currentIndex]);
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
            
            Active = true;
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
            
            Active = false;
        }

        public void SetOff()
        {
            if (!Try.SetAnimationTrigger(animator, turnOnAnimationTrigger, "turn off"))
                SetOffImmediately();
            Try.PlayOneShoot(transform, turnOffSound, "turn off");
        }

        public void TryReload()
        {
            if (batteryAmount > 0)
            {
                batteryAmount--;
                isInAnimation = true;

                Try.PlayOneShoot(transform, reloadSound, "reload");
                if (!Try.SetAnimationTrigger(animator, reloadAnimationTrigger, "reload"))
                {
                    FromReloadLantern();
                }
            }
        }

        private void FromReloadLantern()
        {
            isInAnimation = false;
            animator.ResetTrigger("TurnOff");
            currentDuration = duration;
            SetOnImmediately();
        }
    }
}
