using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enderlook.Unity.AudioManager;
using Enderlook.Unity.Toolset.Attributes;
using Game.Enemies;
using Game.Player;
using Game.Player.Weapons;
using Game.Utility;
using Kam.Utils.FSM;
using TMPro.SpriteAssetUtilities;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class TutorialCrawler : MonoBehaviour
{

    [FormerlySerializedAs("sightRadius")]
    [Header("Sight")]
    [SerializeField, Min(0), Tooltip("Determines at which radius the creature can see the player.")]
    protected float sightRadius_Idle;

    [field: SerializeField, IsProperty, Tooltip("Layers that can block enemy sight.")]
    protected LayerMask BlockSight { get; set; }
    
    [field: SerializeField, IsProperty, Tooltip("Layers that can block enemy interactions, but not sight.")]
    protected LayerMask BlockInteraction { get; set; }

    [SerializeField, Min(0), Tooltip("Height offset of eyes.")]
    private float eyeOffset = .5f;


    [Header("Animation Triggers")]
    [SerializeField, Tooltip("Name of the animation trigger when idle.")]
    public string idleAnimationTrigger;

    [SerializeField, Tooltip("Name of the animation trigger when it dies (the animation must execute `FromDeath()` event on the last frame).")]
    private string deathAnimationTrigger;

    [SerializeField, Tooltip("Name of the animation trigger when it dies from an attack on the weakspot (the animation must execute `FromDeath()` event on the last frame).")]
    private string deathWeakspotAnimationTrigger;

    [SerializeField, Tooltip("Name of the animation trigger when blinded (the animation must execute `FromBlind()` at the end).")]
    public string blindAnimationTrigger;
    

    [SerializeField, Tooltip("Name of the animation trigger when is charging towards player due to rage.")]
    private string chargeAnimationTrigger;

    [SerializeField, Tooltip("Name of the animation trigger used to buildRage (the animation must execute `Melee()` event at some point and `FromMelee()` at the end).")]
    private string rageBuildupAnimationTrigger;
    
    

    [Header("Sounds")]

    [SerializeField, Tooltip("Sound played when creature die.")]
    private AudioUnit deathSound;

    [SerializeField, Tooltip("Sound played when creature die on the weakspot.")]
    private AudioUnit deathWeakspotSound;


    [Header("Tutorial Success and Fails")]
    public List<TutorialPhaseAction> PhaseActions = new List<TutorialPhaseAction>();

    [System.Serializable]
    public struct TutorialPhaseAction
    {
        public TutorialPhase phase;
        public List<TutorialPhaseFailAction> failActions;
        public UnityEvent onSuccess;
    }
    
    [System.Serializable]
    public struct TutorialPhaseFailAction
    {
        public string name;
        public UnityEvent onFail;
    }
    
    private bool isInShootingAnimation;
    private bool isInMeleeAnimation;
    private bool isInStunAnimation;

    private bool Succeeded = false;
    public float failingCD = 5;

    public void PlayAudioOneShoot(AudioUnit audio)
    {
        KamAudioManager.instance.PlaySFX_AudioUnit(audio, transform.position);
        //AudioController.PlayOneShoot(audio, transform.position);
    }

    protected NavMeshAgent NavAgent { get; private set; }
    protected Animator Animator { get; private set; }

    protected Vector3 EyePosition
    {
        get
        {
            Vector3 center = transform.position;
            center.y += eyeOffset;
            return center;
        }
    }

    protected Vector3 LastPlayerPosition { get; private set; }
    protected Glowstick GlowstickToChase { get; private set; }

    private string LastAnimationTrigger;

    protected bool IsInBlindAnimation { get; set; }
    

    protected virtual void Awake()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();

        SetupTutorialFSM();
        
        NavAgent.isStopped = true;
    }

    protected virtual void FixedUpdate()
    {
        if (Succeeded || PauseMenu.Paused) return;
        
        _Fsm.FixedUpdate();
        _TutorialFsm.FixedUpdate();
    }

    private void Update()
    {
        if (Succeeded || PauseMenu.Paused) return;
        
        _Fsm.Update();
        _TutorialFsm.Update();
    }

    
    #region FSM

    private TutorialPhase currentPhase;
    private EventFSM<TutorialPhase> _TutorialFsm;
    protected void SetupTutorialFSM()
    {
        #region Declare
        
        var RageInterruption = new State<TutorialPhase>("RageInterruption");
        var Stun = new State<TutorialPhase>("Stun");
        var Angry = new State<TutorialPhase>("Angry");
        var AngryInterrupt = new State<TutorialPhase>("AngryInterrupt");
        
        #endregion

        #region MakeTransitions

        StateConfigurer.Create(RageInterruption)
            .SetTransition(TutorialPhase.Stun, Stun)
            .Done();

        StateConfigurer.Create(Stun)
            .SetTransition(TutorialPhase.Angry, Angry)
            .Done();
        
        StateConfigurer.Create(Angry)
            .SetTransition(TutorialPhase.AngryInterrupt, AngryInterrupt)
            .Done();
        
        StateConfigurer.Create(AngryInterrupt)
            .Done();

        #endregion

        #region StateBehaviour

        RageInterruption.OnEnter += x =>
        {
            Debug.Log($"{gameObject.name}: Rage interruption tutorial");
            currentPhase = TutorialPhase.RageInterruption;

            SetupFSM(TutorialPhase.RageInterruption);
        };
        
        Stun.OnEnter += x =>
        {
            Debug.Log($"{gameObject.name}: Stun tutorial");
            currentPhase = TutorialPhase.Stun;

            SetupFSM(TutorialPhase.Stun);
        };
        
        Angry.OnEnter += x =>
        {
            Debug.Log($"{gameObject.name}: Angry tutorial");
            currentPhase = TutorialPhase.Angry;

            SetupFSM(TutorialPhase.Angry);
        };
        
        AngryInterrupt.OnEnter += x =>
        {
            Debug.Log($"{gameObject.name}: AngryInterrupt tutorial");
            currentPhase = TutorialPhase.AngryInterrupt;

            SetupFSM(TutorialPhase.AngryInterrupt);
        };
        
        #endregion
        
        _TutorialFsm = new EventFSM<TutorialPhase>(RageInterruption);
    }

    #endregion
    
    #region FSM

    public EnemyState currentState;
    public EnemyState lastState;
    protected EventFSM<EnemyState> _Fsm;
    public enum EnemyState
    {
        Idle, Blinded_Player, ChasePlayer, RageBuildup_Player, ChaseGlowstick, RageBuildup_Glowstick, CertainKillMode, Dead, Blinded_Glowstick
    }

    public enum TutorialPhase
    {
        RageInterruption = 1, Stun = 2, Angry = 3, AngryInterrupt = 4
    }

    private void SetupFSM(TutorialPhase phase)
    {
        Succeeded = false;
        switch (phase)
        {
            case TutorialPhase.RageInterruption:
                SetupFSM_RageInterruption();
                break;
            
            case TutorialPhase.Stun:
                SetupFSM_Stun();
                break;
            
            case TutorialPhase.Angry:
                SetupFSM_Angry();
                break;
            
            case TutorialPhase.AngryInterrupt:
                SetupFSM_AngryInterrupt();
                break;
        }
    }
    void SetupFSM_RageInterruption()
    {
        #region Declare
        
        var Idle = new State<EnemyState>("Idle");
        var RageBuildup_Player = new State<EnemyState>("RageBuildup_Player");
        var Dead = new State<EnemyState>("Dead");

        #endregion

        #region MakeTransitions

        StateConfigurer.Create(Idle)
            .SetTransition(EnemyState.RageBuildup_Player, RageBuildup_Player)
            .SetTransition(EnemyState.Dead, Dead)
            .Done();

        StateConfigurer.Create(RageBuildup_Player)
            .SetTransition(EnemyState.Idle, Idle)
            .SetTransition(EnemyState.Dead, Dead)
            .Done();

        StateConfigurer.Create(Dead)
            .Done();

        #endregion

        #region StateBehaviour

        Idle.OnEnter += x =>
        {
            Debug.Log($"{gameObject.name}: IDLE");
            currentState = EnemyState.Idle;
            
            TrySetAnimationTrigger(idleAnimationTrigger, "idle",false, true);
        };
        
        Idle.OnUpdate += () =>
        {
            if (readyToFail)
            {
                PlayerLightBehaviors();
                
                Lantern.DistanceEffect lightEffect = HasPlayerLightInRange();
                if (lightEffect == Lantern.DistanceEffect.Close)
                {
                    TrySetAnimationTrigger(blindAnimationTrigger, "blind");
                    Fail("TooClose");
                }
            }
        };

        RageBuildup_Player.OnEnter += x =>
        {
            Debug.Log($"{gameObject.name}: RAGE BUILDUP PLAYER");
            
            currentState = EnemyState.RageBuildup_Player;

            TrySetAnimationTrigger(rageBuildupAnimationTrigger, "rage");
        };
        
        RageBuildup_Player.OnUpdate += () =>
        {
            if (CheckRage_Player())
            {
                _Fsm.SendInput(EnemyState.Idle);
                Success();
            }
        };


        Dead.OnEnter += x =>
        {
            
        };
        
        
        #endregion
        
        _Fsm = new EventFSM<EnemyState>(Idle);
    }
    
    void SetupFSM_Stun()
    {
        #region Declare
        
        var Idle = new State<EnemyState>("Idle");
        var Blinded_Player = new State<EnemyState>("Blinded_Player");
        var RageBuildup_Player = new State<EnemyState>("RageBuildup_Player");
        var Dead = new State<EnemyState>("Dead");
        
        #endregion

        #region MakeTransitions

        StateConfigurer.Create(Idle)
            .SetTransition(EnemyState.Blinded_Player, Blinded_Player)
            .SetTransition(EnemyState.RageBuildup_Player, RageBuildup_Player)
            .SetTransition(EnemyState.Dead, Dead)
            .Done();
        
        StateConfigurer.Create(Blinded_Player)
            .SetTransition(EnemyState.Idle, Idle)
            .SetTransition(EnemyState.Dead, Dead)
            .Done();

        StateConfigurer.Create(RageBuildup_Player)
            .SetTransition(EnemyState.Idle, Idle)
            .SetTransition(EnemyState.Dead, Dead)
            .Done();
        
        StateConfigurer.Create(Dead)
            .Done();

        #endregion

        #region StateBehaviour

        Idle.OnEnter += x =>
        {
            Debug.Log($"{gameObject.name}: IDLE");
            currentState = EnemyState.Idle;

            TrySetAnimationTrigger(idleAnimationTrigger, "idle",false, true);
        };
        
        Idle.OnUpdate += () =>
        {
            PlayerLightBehaviors();
        };
        
        
        
        Blinded_Player.OnEnter += x =>
        {
            Debug.Log($"{gameObject.name}: BLINDED PLAYER");
            
            CheckAndSaveLastState();
            
            currentState = EnemyState.Blinded_Player;

            IsInBlindAnimation = true;
            Animator.SetBool("Blinded", true);
            if (!TrySetAnimationTrigger(blindAnimationTrigger, "blind", false))
            {
                //If you were not able to set the animation, end the animation yourself
                //(This part is usually called at the end of the animation)
                FinishedAnimation(Animations.Blinded);
            }
            
            Success();
        };

        
        RageBuildup_Player.OnEnter += x =>
        {
            Debug.Log($"{gameObject.name}: RAGE BUILDUP PLAYER");
            CheckAndSaveLastState();
            
            currentState = EnemyState.RageBuildup_Player;
            
            TrySetAnimationTrigger(rageBuildupAnimationTrigger, "rage");
        };
    
        RageBuildup_Player.OnUpdate += () =>
        {
            if (CheckRage_Player())
            {
                _Fsm.SendInput(EnemyState.Idle);
                Fail();
            }
        };
        
        
        Dead.OnEnter += x =>
        {
            
        };
        
        
        #endregion
        
        _Fsm = new EventFSM<EnemyState>(Idle);
    }
    
    void SetupFSM_Angry()
    {
        #region Declare
        
        var Idle = new State<EnemyState>("Idle");
        var RageBuildup_Player = new State<EnemyState>("RageBuildup_Player");
        var Blinded_Player = new State<EnemyState>("Blinded_Player");
        var CertainKillMode = new State<EnemyState>("CertainKillMode");
        var Dead = new State<EnemyState>("Dead");
        
        #endregion

        #region MakeTransitions

        StateConfigurer.Create(Idle)
            .SetTransition(EnemyState.Blinded_Player, Blinded_Player)
            .SetTransition(EnemyState.RageBuildup_Player, RageBuildup_Player)
            .SetTransition(EnemyState.Dead, Dead)
            .Done();
        
        StateConfigurer.Create(CertainKillMode)
            .SetTransition(EnemyState.Idle, Idle)
            .SetTransition(EnemyState.Dead, Dead)
            .Done();

        StateConfigurer.Create(RageBuildup_Player)
            .SetTransition(EnemyState.Blinded_Player, Blinded_Player)
            .SetTransition(EnemyState.CertainKillMode, CertainKillMode)
            .SetTransition(EnemyState.Idle, Idle)
            .SetTransition(EnemyState.Dead, Dead)
            .Done();
        
        StateConfigurer.Create(Blinded_Player)
            .SetTransition(EnemyState.Idle, Idle)
            .SetTransition(EnemyState.Dead, Dead)
            .Done();
        
        StateConfigurer.Create(Dead)
            .Done();

        #endregion

        #region StateBehaviour

        Idle.OnEnter += x =>
        {
            Debug.Log($"{gameObject.name}: IDLE");
            currentState = EnemyState.Idle;

            TrySetAnimationTrigger(idleAnimationTrigger, "idle",false, true);
        };
        
        Idle.OnUpdate += () =>
        {
            PlayerLightBehaviors();
        };

        Blinded_Player.OnEnter += x =>
        {
            Debug.Log($"{gameObject.name}: BLINDED PLAYER");
            
            CheckAndSaveLastState();
            
            currentState = EnemyState.Blinded_Player;

            IsInBlindAnimation = true;
            Animator.SetBool("Blinded", true);
            
            Fail();
            
            if (!TrySetAnimationTrigger(blindAnimationTrigger, "blind", false))
            {
                //If you were not able to set the animation, end the animation yourself
                //(This part is usually called at the end of the animation)
                FinishedAnimation(Animations.Blinded);
            }
        };

        RageBuildup_Player.OnEnter += x =>
        {
            Debug.Log($"{gameObject.name}: RAGE BUILDUP PLAYER");
            CheckAndSaveLastState();
            
            currentState = EnemyState.RageBuildup_Player;
            
            TrySetAnimationTrigger(rageBuildupAnimationTrigger, "rage");
        };
    
        RageBuildup_Player.OnUpdate += () =>
        {
            if (CheckRage_Player())
            {
                _Fsm.SendInput(EnemyState.Idle);
                Fail("InterruptedAnimation");
            }
        };
        
        
        CertainKillMode.OnEnter += x =>
        {
            Debug.Log($"{gameObject.name}: CERTAIN KILL");
            currentState = EnemyState.CertainKillMode;

            TrySetAnimationTrigger(chargeAnimationTrigger, "certainKill");

            OnCertainKill_Enter(x);
            
            Success();
        };

        Dead.OnEnter += x =>
        {
            
        };
        
        
        #endregion
        
        _Fsm = new EventFSM<EnemyState>(Idle);
    }
    
    void SetupFSM_AngryInterrupt()
    {
        #region Declare
        
        var Idle = new State<EnemyState>("Idle");
        var CertainKillMode = new State<EnemyState>("CertainKillMode");
        var Dead = new State<EnemyState>("Dead");
        
        #endregion

        #region MakeTransitions

        StateConfigurer.Create(Idle)
            .SetTransition(EnemyState.Dead, Dead)
            .Done();
        
        StateConfigurer.Create(CertainKillMode)
            .SetTransition(EnemyState.Idle, Idle)
            .SetTransition(EnemyState.Dead, Dead)
            .Done();

        StateConfigurer.Create(Dead)
            .Done();

        #endregion

        NavAgent.isStopped = true;
        
        #region StateBehaviour

        Idle.OnEnter += x =>
        {
            Debug.Log($"{gameObject.name}: IDLE");
            currentState = EnemyState.Idle;

            TrySetAnimationTrigger(idleAnimationTrigger, "idle",false, true);
            
            Success();
        };


        CertainKillMode.OnEnter += x =>
        {
            Debug.Log($"{gameObject.name}: CERTAIN KILL");
            currentState = EnemyState.CertainKillMode;

            TrySetAnimationTrigger(chargeAnimationTrigger, "certainKill");

            OnCertainKill_Enter(x);
        };

        CertainKillMode.OnUpdate += CrawlerFlashingBehavior;
        
        Dead.OnEnter += x =>
        {
            
        };
        
        
        #endregion
        
        _Fsm = new EventFSM<EnemyState>(CertainKillMode);
    }

    void Success()
    {
        Succeeded = true;
        PhaseActions.First(x => x.phase == currentPhase).onSuccess.Invoke();
    }

    private bool readyToFail = true;
    void Fail(string failName = "default")
    {
        if (!readyToFail) return;
        
        Succeeded = false;
        PhaseActions.First(x => x.phase == currentPhase).failActions.First(x => x.name.ToLower() == failName.ToLower()).onFail.Invoke();
        readyToFail = false;
        Invoke(nameof(ResetFail), failingCD);
    }

    void ResetFail()
    {
        readyToFail = true;
    }
    
    public void SendInputToTutorial(int phase)
    {
        _TutorialFsm.SendInput((TutorialPhase)phase);
    }
    protected void CheckAndSaveLastState()
    {
        if (currentState != EnemyState.Blinded_Player && currentState != EnemyState.RageBuildup_Player && currentState != EnemyState.RageBuildup_Glowstick)
        {
            lastState = currentState;
        }
    }

    protected void LoadLastState()
    {
        _Fsm.SendInput(lastState);
    }

    protected bool CheckRage_Player()
    {
        Lantern.DistanceEffect distance = HasPlayerLightInRange();
                
        //Cases where you'd want to stay in rage
        bool case1 = distance == Lantern.DistanceEffect.Close &&
                     Lantern.ActiveLantern.lightType == Lantern.LightType.Red;

        bool case2 = distance == Lantern.DistanceEffect.Far &&
                     Lantern.ActiveLantern.lightType == Lantern.LightType.Red;
                
        bool case3 = distance == Lantern.DistanceEffect.Far &&
                     Lantern.ActiveLantern.lightType == Lantern.LightType.White;
                
        //If you are not in any of these cases, you are enraged.
        return !(case1 || case2 || case3);
    }

    public int flashingAmount = 5;
    public float flashingTime = 3;
    public float voiceLineRefresh = 4;
    private bool affectedByLight;
    private int flashCount = 0;
    private float timeSinceStart = 0;
    protected void CrawlerFlashingBehavior()
    {
        Lantern.DistanceEffect lightEffect = HasPlayerLightInRange();
        if (lightEffect == Lantern.DistanceEffect.Close)
        {
            if (Lantern.ActiveLantern.lightType == Lantern.LightType.White)
            {
                if (!affectedByLight)
                {
                    affectedByLight = true;
                    
                    flashCount++;
                    
                    if (flashCount >= flashingAmount)
                    {
                        _Fsm.SendInput(EnemyState.Idle);
                    }
                }
            }
            else
            {
                affectedByLight = false;
            }
            
        }
        else
        {
            affectedByLight = false;
        }
        
        if (lightEffect == Lantern.DistanceEffect.Far && currentPhase == TutorialPhase.AngryInterrupt)
        {
            Fail("TooFar");
        }

        if (flashCount > 0)
        {
            timeSinceStart += Time.deltaTime;

            if (timeSinceStart > flashingTime)
            {
                flashCount = 0;
                timeSinceStart = 0;
                
                Fail();
            }
        }
    }
    
    protected virtual void OnCertainKill_Enter(EnemyState state)
    {
        timeSinceStart = 0;
    }

    public enum Animations
    {
        Blinded, Melee, RageBuildup, Shoot
    }
    public void FinishedAnimation(Animations anim)
    {
        switch (anim)
        {
            case Animations.Blinded:
            {
                Animator.SetBool("Blinded", false);
                IsInBlindAnimation = false;
                
                //LoadLastState();

                isInStunAnimation = false; // Sometimes the flag may have a false positive if the animation was terminated abruptly.

                if (currentPhase == TutorialPhase.Angry)
                {
                    _Fsm.SendInput(EnemyState.Idle);
                }
                else
                {
                    if (currentState == EnemyState.Blinded_Glowstick)
                    {
                        _Fsm.SendInput(EnemyState.ChaseGlowstick);
                        break;
                    }
                
                    if(currentState == EnemyState.Blinded_Player)
                    {
                        _Fsm.SendInput(EnemyState.CertainKillMode);
                    }
                }
                
                break;
            }

            case Animations.Melee:
            {
                isInMeleeAnimation = false;
                break;
            }

            case Animations.RageBuildup:
            {
                if (currentPhase == TutorialPhase.RageInterruption || currentPhase == TutorialPhase.Stun)
                {
                    Fail();
                    _Fsm.SendInput(EnemyState.Idle);
                }
                else
                {
                    if (currentState == EnemyState.RageBuildup_Glowstick)
                    {
                        _Fsm.SendInput(EnemyState.ChaseGlowstick);
                        break;
                    }
                
                    if (currentState == EnemyState.RageBuildup_Player)
                    {
                        _Fsm.SendInput(EnemyState.CertainKillMode);
                    }
                }
                break;
            }

            case Animations.Shoot:
            {
                isInShootingAnimation = false;
                break;
            }
        }
    }
    #endregion

    protected virtual void PlayerLightBehaviors()
    {
        Lantern.DistanceEffect lightEffect = HasPlayerLightInRange();
        if (lightEffect != Lantern.DistanceEffect.None)
        {
            LightEffect(lightEffect);
        }
    }
    
    protected virtual void GlowstickBehaviors()
    {
        Lantern.DistanceEffect glowstickEffect = HasGlowstickInRange();
        if (glowstickEffect != Lantern.DistanceEffect.None)
        {
            GlowstickEffect(glowstickEffect);
        }
    }
    
    protected void LightEffect(Lantern.DistanceEffect lightEffect)
    {
        switch (lightEffect)
        {
            case Lantern.DistanceEffect.Far:
            {
                //Rage
                _Fsm.SendInput(EnemyState.RageBuildup_Player);
                break;
            }

            case Lantern.DistanceEffect.Close:
            {
                switch (Lantern.ActiveLantern.lightType)
                {
                    case Lantern.LightType.White:
                    {
                        //Stunned
                        _Fsm.SendInput(EnemyState.Blinded_Player);
                        break;
                    }

                    case Lantern.LightType.Red:
                    {
                        //Rage
                        _Fsm.SendInput(EnemyState.RageBuildup_Player);
                        break;
                    }
                }
                break;
            }
        }
    }
        
    protected void GlowstickEffect(Lantern.DistanceEffect lightEffect)
    {
        switch (lightEffect)
        {
            case Lantern.DistanceEffect.Far:
            {
                if (currentState != EnemyState.ChaseGlowstick)
                {
                    //Rage
                    _Fsm.SendInput(EnemyState.RageBuildup_Glowstick);
                }
                
                break;
            }

            case Lantern.DistanceEffect.Close:
            {
                if (currentState != EnemyState.ChaseGlowstick)
                {
                    //Stunned
                    _Fsm.SendInput(EnemyState.Blinded_Glowstick);
                }

                break;
            }
        }
    }


    /// <summary>
    /// Checks if you can interact with a point.
    /// </summary>
    /// <param name="point">The point you're trying to interact with</param>
    /// <returns>Returns false if there is something in between your eye position and the point you want to interact with.</returns>
    public bool CheckInteraction(Vector3 point)
    {
        return !Physics.Linecast(PlayerBody.Player.transform.position, EyePosition, BlockInteraction);
    }

    protected bool HasPlayerInSight(float sightRadius)
    {
        Vector3 eyePosition = EyePosition;
        if ((PlayerBody.Player.transform.position - eyePosition).sqrMagnitude < sightRadius
            && !Physics.Linecast(PlayerBody.Player.transform.position, eyePosition, BlockSight))
        {
            LastPlayerPosition = PlayerBody.Player.transform.position;
            return true;
        }
        return false;
    }

    protected void LookAtPlayer()
    {
        Vector3 lookPosition = LastPlayerPosition - transform.position;
        lookPosition.y = 0;
        transform.rotation = Quaternion.LookRotation(lookPosition);
    }

    public virtual void MakeAwareOfPlayerLocation()
    {
        SetLastPlayerPosition();
        
        switch (currentState)
        {
            case EnemyState.ChasePlayer:
                bool result = NavAgent.SetDestination(LastPlayerPosition);
                Debug.Assert(result);
                break;
            case EnemyState.Idle:
                _Fsm.SendInput(EnemyState.ChasePlayer);
                break;
        }
    }

    protected void SetLastPlayerPosition() => LastPlayerPosition = PlayerBody.Player.transform.position;

    protected Lantern.DistanceEffect HasPlayerLightInRange()
    {
        if(!Lantern.Active)
            return Lantern.DistanceEffect.None;
        
        Light light = Lantern.ActiveLight;
        if (light == null)
            return Lantern.DistanceEffect.None;
        
        Lantern flashlight = Lantern.ActiveLantern;;
        if (flashlight == null)
            return Lantern.DistanceEffect.None;
        
        
        Transform lightTranform = light.transform;

        Vector3 closestEnemyPoint = transform.position + Vector3.up * eyeOffset;
            
            /*colliders.Aggregate(Tuple.Create(Vector3.zero, float.PositiveInfinity), (acum, item) =>
        {
            Vector3 closestPoint = item.ClosestPoint(Lantern.ActiveLantern.transform.position);
            float distance = Vector3.Distance(closestPoint, Lantern.ActiveLantern.transform.position);
            if (distance < acum.Item2)
            {
                return Tuple.Create(closestPoint, distance);
            }

            return acum;
        }).Item1;*/


        Vector3 lightPosition = lightTranform.position;

        Vector3 lightDirection = closestEnemyPoint - lightPosition;
        float distanceToConeOrigin = lightDirection.sqrMagnitude;

        if (distanceToConeOrigin <= Lantern.ActiveLantern.interactionRange_Far)
        {
            Vector3 coneDirection = lightTranform.forward;
            float angle = Vector3.Angle(coneDirection, lightDirection);
            if (angle <= flashlight.interactionAngle)
            {
                if (!Physics.Linecast(closestEnemyPoint, lightPosition, BlockSight))
                {
                    //Light is hitting, now check distance.
                    
                    if (distanceToConeOrigin > Lantern.ActiveLantern.interactionRange_Close)
                    {
                        return Lantern.DistanceEffect.Far;
                    }
                    else
                    {
                        return Lantern.DistanceEffect.Close;
                    }
                }

            }
        }

        return Lantern.DistanceEffect.None;
    }
    
    protected Lantern.DistanceEffect HasGlowstickInRange()
    {
        if (Glowstick.activeGlowsticks.Count == 0)
        {
            GlowstickToChase = null;
            return Lantern.DistanceEffect.None;
        }
        
        List<Tuple<Lantern.DistanceEffect, Glowstick>> stickEffects = new List<Tuple<Lantern.DistanceEffect, Glowstick>>();
        foreach (var stick in Glowstick.activeGlowsticks)
        {
            Vector3 closestEnemyPoint = transform.position + Vector3.up * eyeOffset;
            Vector3 lightPosition = stick.transform.position;

            Vector3 lightDirection = closestEnemyPoint - lightPosition;
            float distanceToOrigin = lightDirection.sqrMagnitude;

            if (distanceToOrigin <= stick.interactionRange_Far)
            {
                if (!Physics.Linecast(closestEnemyPoint, lightPosition, BlockSight))
                {
                    //Light is hitting, now check distance.
                
                    if (distanceToOrigin > stick.interactionRange_Close)
                    {
                        stickEffects.Add(Tuple.Create(Lantern.DistanceEffect.Far, stick));
                    }
                    else
                    {
                        stickEffects.Add(Tuple.Create(Lantern.DistanceEffect.Close, stick));
                    }
                }
            }
            
            stickEffects.Add(Tuple.Create(Lantern.DistanceEffect.None, stick));
        }

        foreach (var item in stickEffects)
        {
            if (item.Item1 == Lantern.DistanceEffect.Close)
            {
                GlowstickToChase = item.Item2;
                return Lantern.DistanceEffect.Close;
            }
        }
        
        foreach (var item in stickEffects)
        {
            if (item.Item1 == Lantern.DistanceEffect.Far)
            {
                GlowstickToChase = item.Item2;
                return Lantern.DistanceEffect.Far;
            }
        }

        GlowstickToChase = null;
        return Lantern.DistanceEffect.None;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity animator.")]
    private void FromHurt() => TrySetLastAnimationTrigger();

    private void OnDeath(bool isOnWeakspot)
    {
        NavAgent.isStopped = true;
        NavAgent.enabled = false;

        foreach (Collider collider in GetComponentsInChildren<Collider>())
            collider.enabled = false;

        if (isOnWeakspot)
        {
            if (!Try.PlayOneShoot(transform, deathWeakspotSound, "death weakspot"))
                Try.PlayOneShoot(transform, deathSound, "death");

            if (!TrySetAnimationTrigger(deathWeakspotAnimationTrigger, "death weakspot", true, true))
                TrySetAnimationTrigger(deathAnimationTrigger, "death", true, true);
        }
        else
        {
            Try.PlayOneShoot(transform, deathSound, "death");
            TrySetAnimationTrigger(deathAnimationTrigger, "death", true, true);
        }
    }

    protected bool TrySetAnimationTrigger(string triggerName, string triggerMetaName, bool recordAsLastAnimation = true, bool disableAllTriggers = false)
    {
        if (disableAllTriggers)
        {
            foreach (AnimatorControllerParameter parameter in Animator.parameters)
            {
                if (parameter.type == AnimatorControllerParameterType.Trigger)
                    Animator.ResetTrigger(parameter.nameHash);
            }
        }

        if (Try.SetAnimationTrigger(Animator, triggerName, triggerMetaName))
        {
            if (recordAsLastAnimation)
                LastAnimationTrigger = triggerName;
            return true;
        }
        else
        {
            LastAnimationTrigger = "";
            return false;
        }
    }

    protected void TrySetLastAnimationTrigger()
    {
        if (!string.IsNullOrEmpty(LastAnimationTrigger))
            Animator.SetTrigger(LastAnimationTrigger);
    }
}

