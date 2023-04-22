using System;
using System.Collections.Generic;
using Enderlook.Unity.AudioManager;
using Enderlook.Unity.Toolset.Attributes;

using Game.Utility;

using UnityEngine;
using UnityEngine.Events;
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
        
        [SerializeField, Tooltip("Key used to recharge light.")]
        private KeyCode rechargeKey = KeyCode.R;

        [SerializeField, Min(0), Tooltip("Duration of light in seconds.")]
        private float duration;

        [SerializeField, Min(0), Tooltip("Starting level of battery.")]
        private float durationStart;
        
        [SerializeField, Min(0)]
        private float durationRecoverySpeed;
        
        [SerializeField, Min(0),Tooltip("The time that is subtracted from total time when turning on the flashlight, to not simply keep flickering it.")]
        private float turnOnTimeCost;
        
        [FormerlySerializedAs("interactionRange")] [Min(0), Tooltip("Minimum Range at which the light starts interacting with things, such as enemies, at CLOSE range.")]
        public float interactionRange_Close;
        
        [Min(0), Tooltip("Maximum range at which the light starts interacting with things, such as enemies, at the FAR range.")]
        public float interactionRange_Far;
        
        [Min(0), Tooltip("Angle at which the light starts interacting with things, such as enemies.")]
        public float interactionAngle;

        public LightType lightType = LightType.White;
        private List<LightType> lightList = new List<LightType> {LightType.White};
        private int currentIndex = 0;

        [Header("Animation")]
        [SerializeField, Tooltip("Animation trigger when replacing batteries. (The animation must execute FromReloadLantern() method.)")]
        private string reloadAnimationTrigger;
        
        [SerializeField, Tooltip("Animation trigger when replacing batteries. (The animation must execute FromReloadLantern() method.)")]
        private GameObject reloadAnimationObject;

        [SerializeField, Tooltip("Animation trigger when run out of battery.")]
        private string outOfBatteryAnimationTrigger;

        [SerializeField, Tooltip("Animation trigger when turn on lantern.")]
        private string turnOnAnimationTrigger;

        [SerializeField, Tooltip("Animation trigger when turn off lantern.")]
        private string turnOffAnimationTrigger;

        [Header("Sound")]
        [SerializeField, Tooltip("Sound played when replacing batteries.")]
        private AudioUnit reloadSound;

        [SerializeField, Tooltip("Sound played when running out of battery.")]
        private AudioUnit outOfBatterySound;

        [SerializeField, Tooltip("Sound played when turn on lantern.")]
        private AudioUnit turnOnSound;

        [SerializeField, Tooltip("Sound played when turn off lantern.")]
        private AudioUnit turnOffSound;
        
        [SerializeField, Tooltip("Sound played when you are unable to turn on the flashlight.")]
        private AudioUnit unableToTurnOnSound;
        
        [SerializeField, ShowIf(nameof(objectWithLanternRenderer), typeof(List<Renderer>), null, false), Tooltip("Sound that will be played on low battery.")]
        private AudioUnit batteryLowLevelSound;

        [Header("Shader")]
        [SerializeField, Tooltip("Object with the material that presents lantern feedback.")]
        public List<Renderer> objectWithLanternRenderer;

        [SerializeField, ShowIf(nameof(objectWithLanternRenderer), typeof(List<Renderer>), null, false), Tooltip("Field of the shader used to set battery percent.")]
        private string batteryPercentFieldName;
        
        [SerializeField, ShowIf(nameof(objectWithLanternRenderer), typeof(List<Renderer>), null, false), Tooltip("Field of the shader used to set battery color.")]
        private string batteryColorFieldName;
        
        [Range(0,100)]
        [SerializeField, ShowIf(nameof(objectWithLanternRenderer), typeof(List<Renderer>), null, false), Tooltip("Threshhold of batteryLevel used to set low battery.")]
        private float batteryLowLevelPercentThreshhold;
        
        [SerializeField, ShowIf(nameof(objectWithLanternRenderer), typeof(List<Renderer>), null, false), Tooltip("Field of the shader used to set battery low level.")]
        private string batteryLowLevelFieldName;
        
        

        private bool playedSound = false;
        
        [SerializeField, ShowIf(nameof(objectWithLanternRenderer), typeof(List<Renderer>), null, false), Tooltip("Field of the shader used to set battery percent.")]
        private Color batteryWhiteColor;
        [SerializeField, ShowIf(nameof(objectWithLanternRenderer), typeof(List<Renderer>), null, false), Tooltip("Field of the shader used to set battery percent.")]
        private Color batteryRedColor;
        [SerializeField, ShowIf(nameof(objectWithLanternRenderer), typeof(List<Renderer>), null, false), Tooltip("Field of the shader used to set battery percent.")]
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
        
        private Material haloLightShader;

        private float currentDuration;
        public int instaCoolDownBatteryAmount;

        private bool isInAnimation;

        public UnityEvent onFirstTimeOutOfBattery;
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

            currentDuration = durationStart;

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
                if (!outOfBattery /*|| batteryAmount > 0*/)
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
                else
                {
                    Debug.Log("Out of battery");
                    Try.PlayOneShoot(transform, unableToTurnOnSound, "Unable to turn on");
                }
            }
            
            if (Input.GetKeyDown(rechargeKey))
            {
                TryReload();
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

            bool cond1 = light.intensity <= 0;
            bool cond2 = light.range <= 0;
            bool cond3 = light.spotAngle <= 0;
            
            if (cond1 || cond2 || cond3)
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

            UpdateBatteryShader();

            if (light.enabled)
            {
                if (currentDuration > 0)
                {
                    outOfBattery = false;
                    currentDuration -= Time.deltaTime;
                    
                    if (currentDuration < 0)
                    {
                        outOfBattery = true;
                        if (PlayerArmsManager.FirstTimeOutOfBattery)
                        {
                            PlayerArmsManager.FirstTimeOutOfBattery = false;
                            
                            PlayerArmsManager.i.InstructionArmAnim(delegate
                            {
                                onFirstTimeOutOfBattery.Invoke();
                            });
                            
                        }
                        
                        Active = false;
                        if (Try.SetAnimationTrigger(animator, outOfBatteryAnimationTrigger, "out of battery"))
                        {
                            
                        }
                        else
                        {
                            SetOffImmediately();
                        }

                        Try.PlayOneShoot(transform, outOfBatterySound, "out of battery");
                    }
                }
                else
                {
                    
                }
            }
            else
            {
                currentDuration += durationRecoverySpeed * Time.deltaTime;

                if (currentDuration > duration)
                {
                    currentDuration = duration;
                    if (outOfBattery)
                    {
                        outOfBattery = false;

                        SetOffImmediately();
                        Try.SetAnimationTrigger(animator, reloadAnimationTrigger, "turn on");
                    }
                }
            }
        }

        private void UpdateBatteryShader()
        {
            #region BatteryShader
                float batteryPercent = Mathf.Max(currentDuration, 0) / duration;

                foreach (var obj in objectWithLanternRenderer)
                {
                    Material batteryShader = obj.material;
                    if (batteryShader != null)
                    {
                        if (!string.IsNullOrEmpty(batteryPercentFieldName))
                            batteryShader.SetFloat(batteryPercentFieldName, batteryPercent);

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
                }
                
            #endregion
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
                    foreach (var obj in objectWithLanternRenderer)
                    {
                        Material batteryShader = obj.material;
                        batteryShader.SetColor(batteryColorFieldName, batteryWhiteColor);
                    }

                    break;
                case LightType.Red:
                    light.color = Color.red;
                    haloLightShader.SetColor(haloLightColorFieldName, Color.red);
                    foreach (var obj in objectWithLanternRenderer)
                    {
                        Material batteryShader = obj.material;
                        batteryShader.SetColor(batteryColorFieldName, batteryRedColor);
                    }

                    break;
                case LightType.Blue:
                    light.color = Color.blue;
                    haloLightShader.SetColor(haloLightColorFieldName, Color.blue);
                    foreach (var obj in objectWithLanternRenderer)
                    {
                        Material batteryShader = obj.material;
                        batteryShader.SetColor(batteryColorFieldName, batteryBlueColor);
                    }
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
            
            if (currentDuration < turnOnTimeCost)
                return;
                
            light.intensity = originalIntensity;
            light.range = originalRange;
            light.spotAngle = originalAngle;
            light.enabled = true;

            haloLightShader.SetFloat(haloLightOpacityFieldName, originalOpacity);
            haloLightRenderer.enabled = true;
            
            Active = true;
            
            currentDuration -= turnOnTimeCost;
            
            Debug.Log("Test");
        }

        public void SetOn()
        {
            if (isInAnimation)
                return;
/*
            if (currentDuration <= 0)
            {
                TryReload();
                return;
            }*/

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
            if (currentDuration < duration)
            {
                if (instaCoolDownBatteryAmount > 0)
                {
                    instaCoolDownBatteryAmount--;
                    isInAnimation = true;
                    
                    PlayerArmsManager.i.InstantCooldownBatteryAnim();
                
                    /*Try.PlayOneShoot(transform, reloadSound, "reload");
                    if (!Try.SetAnimationTrigger(animator, reloadAnimationTrigger, "reload"))
                    {
                        FromReloadLantern();
                    }*/
                }
            }
        }

        public void PickupBattery(BatteryPickup battery)
        {
            PlayerArmsManager.i.FirstTimeBatteryPickupAnim();

            switch (battery.batteryType)
            {
                case BatteryPickup.BatteryType.InstantCooldown:
                    PickupBattery_InstantCooldown();
                    break;
                
                case BatteryPickup.BatteryType.FasterCooldown:
                    PickupBattery_FasterCooldown(battery.addedRecoverySpeed);
                    break;

                case BatteryPickup.BatteryType.MoreHeat:
                    PickupBattery_MoreHeat(battery.addedDuration);
                    break;
                
                case BatteryPickup.BatteryType.LessTurnOnCost:
                    PickupBattery_LessTurnOnCost(battery.subtractedTurnOnCost);
                    break;
            }
        }
        
        public void PickupBattery_InstantCooldown(int amount = 1)
        {
            instaCoolDownBatteryAmount += amount;
        }
        
        public void PickupBattery_FasterCooldown(float addedRecoverySpeed)
        {
            durationRecoverySpeed += addedRecoverySpeed;
        }
        
        public void PickupBattery_MoreHeat(float addedDuration)
        {
            duration += addedDuration;
        }
        
        public void PickupBattery_LessTurnOnCost(float subtractedTurnOnCost)
        {
            turnOnTimeCost -= subtractedTurnOnCost;
        }


        public void RestartLanternTime()
        {
            currentDuration = duration;
            
            UpdateBatteryShader();
        }
        
        public void FromReloadLantern()
        {
            isInAnimation = false;
            currentDuration = duration;
            SetOnImmediately();
            animator.ResetTrigger("TurnOff");
            currentDuration = duration;
            outOfBattery = false;


            animationOpacityMultiplier = 1;
            animationRangeMultiplier = 1;
        }
    }
}
