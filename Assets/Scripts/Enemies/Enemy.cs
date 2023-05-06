using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Enderlook.Unity.AudioManager;
using Enderlook.Unity.Toolset.Attributes;

using Game.Player;
using Game.Player.Weapons;
using Game.Utility;
using Kam.Utils.FSM;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Color = UnityEngine.Color;

namespace Game.Enemies
{
    [RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(Animator))]
    public abstract class Enemy : MonoBehaviour, IDamagable
    {
        public static List<Enemy> allEnemies = new List<Enemy>();

        public bool tutorialEnemyBehavior = false;
        
        [SerializeField, Min(0), Tooltip("Maximum health of enemy.")]
        private float maximumHealth;

        
        
        [FormerlySerializedAs("sightRadius")]
        [Header("Sight")]
        [SerializeField, Min(0), Tooltip("Determines at which radius the creature can see the player.")]
        protected float sightRadius_Idle;

        [SerializeField, Min(0), Tooltip("Determines at which radius the creature can see the player.")]
        protected float sightRadius_Active = 500;
        
        [field: SerializeField, IsProperty, Tooltip("Layers that can block enemy sight.")]
        protected LayerMask BlockSight { get; set; }
        
        [field: SerializeField, IsProperty, Tooltip("Layers that can block enemy interactions, but not sight.")]
        protected LayerMask BlockInteraction { get; set; }

        [SerializeField, Min(0), Tooltip("Height offset of eyes.")]
        protected float eyeOffset = .5f;

        [SerializeField, Min(0), Tooltip("Determines the distance at which the creature kills the player in ragemode.")]
        private float rageKillDistance = 1.5f;

        [SerializeField, Min(0), Tooltip("Determines at which distance from player the creature start shooting.")]
        private float startShootingRadius = 3;

        [SerializeField, Min(0), Tooltip("Determines at which distance from player the creature stop shooting.")]
        private float stopShootingRadius = 4;

        [SerializeField, Min(0), Tooltip("Determines at which distance from player the creature start charging.")]
        private float startChargeRadius = 2;

        [SerializeField, Min(0), Tooltip("Determines at which distance from player the creature start melee.")]
        private float startMeleeRadius = .75f;

        [SerializeField, Min(0), Tooltip("Determines at which distance from player the creature stop melee.")]
        private float stopMeleeRadius = 1.5f;
        
        
        [Header("Animation Triggers")]
        [SerializeField, Tooltip("Name of the animation trigger when idle.")]
        public string idleAnimationTrigger;

        [SerializeField, Tooltip("Name of the animation trigger when it is hurt.")]
        private string hurtAnimationTrigger;

        [SerializeField, Tooltip("Name of the animation trigger when it is hurt on the weakspot.")]
        private string hurtWeakspotAnimationTrigger;

        [SerializeField, Tooltip("Name of the animation trigger when it dies (the animation must execute `FromDeath()` event on the last frame).")]
        private string deathAnimationTrigger;

        [SerializeField, Tooltip("Name of the animation trigger when it dies from an attack on the weakspot (the animation must execute `FromDeath()` event on the last frame).")]
        private string deathWeakspotAnimationTrigger;

        [SerializeField, Tooltip("Name of the animation trigger when blinded (the animation must execute `FromBlind()` at the end).")]
        public string blindAnimationTrigger;
        
        [SerializeField, Tooltip("Name of the animation trigger when is running towards the player to attack him.")]
        private string huntAnimationTrigger;
        
        [SerializeField, Tooltip("Name of the animation trigger when lose line of sight to player and is running to its last position.")]
        private string chaseAnimationTrigger;

        [SerializeField, Tooltip("Name of the animation trigger when is charging towards player due to rage.")]
        private string chargeAnimationTrigger;

        [SerializeField, Tooltip("Name of the animation trigger used to shoot (the animation must execute `Shoot()` event at some point and `FromShoot()` at the end).")]
        private string shootAnimationTrigger;
        
        [SerializeField, Tooltip("Name of the animation trigger used to melee (the animation must execute `Melee()` event at some point and `FromMelee()` at the end).")]
        private string meleeAnimationTrigger;
        
        [SerializeField, Tooltip("Name of the animation trigger used to buildRage (the animation must execute `Melee()` event at some point and `FromMelee()` at the end).")]
        private string rageBuildupAnimationTrigger;
        
        

        [Header("Sounds")]
        [SerializeField, Tooltip("Sound played when creature is hurt.")]
        private AudioUnit hurtSound;

        [SerializeField, Tooltip("Sound played when creature is hurt on the weakspot.")]
        private AudioUnit hurtWeakspotSound;

        [SerializeField, Tooltip("Sound played when creature die.")]
        private AudioUnit deathSound;

        [SerializeField, Tooltip("Sound played when creature die on the weakspot.")]
        private AudioUnit deathWeakspotSound;
        
        [SerializeField, Tooltip("Sound played on melee.")]
        private AudioUnit meleeSound;
        
        [SerializeField, Tooltip("Sound played on shoot.")]
        private AudioUnit shootSound;
        
        
        
        
        [Header("Escape")]
        [SerializeField, Min(0), Tooltip("Width of the oscillation when producing zig-zag.")]
        protected float oscillationWidth = 1.5f;

        [SerializeField, Min(0), Tooltip("Frequency of the oscillation when producing zig-zag.")]
        protected float oscillationFrequency = 3;

        [SerializeField, Min(0), Tooltip("Speed multiplier when escaping.")]
        protected float escapingSpeedMultiplier = 1;
        
        
        
        
        [Header("Melee")]
        [SerializeField, Min(0), Tooltip("Amount of damage produced on melee strike.")]
        protected float meleeDamage = 20;

        [SerializeField, Tooltip("Determines from which collider melee damage is done.")]
        protected Collider meleePosition;

        [SerializeField, Min(0), Tooltip("Movement speed multiplier when chaing to player due to rage.")]
        private float chargingSpeedMultiplier = 1;
        
        
        private MeleeAttack meleeAttack;
        
        
        private bool isInShootingAnimation;
        private bool isInMeleeAnimation;
        private bool isInStunAnimation;

        public void PlayAudioOneShoot(AudioUnit audio)
        {
            KamAudioManager.instance.PlaySFX_AudioUnit(audio, transform.position);
            //AudioController.PlayOneShoot(audio, transform.position);
        }

        protected NavMeshAgent NavAgent { get; private set; }
        protected Animator Animator { get; private set; }

        protected bool IsAlive => currentHealth > 0;

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

        protected float initialSpeed;
        
        private float currentHealth;

        private string LastAnimationTrigger;

        protected bool IsInBlindAnimation { get; set; }
        

        protected virtual void Awake()
        {
            currentHealth = maximumHealth;

            NavAgent = GetComponent<NavMeshAgent>();
            Animator = GetComponent<Animator>();

            if (tutorialEnemyBehavior)
            {
                NavAgent.speed = 0;
            }
            
            initialSpeed = NavAgent.speed;
            
            meleePosition.enabled = false;
            meleeAttack = meleePosition.gameObject.AddComponent<MeleeAttack>();
            meleeAttack.tripod = this;
            meleeAttack.enabled = false;

            // Square values to avoid applying square root when checking distance.
            sightRadius_Idle *= sightRadius_Idle;
            startShootingRadius *= startShootingRadius;
            stopShootingRadius *= stopShootingRadius;
            startMeleeRadius *= startMeleeRadius;
            stopMeleeRadius *= stopMeleeRadius;
            startChargeRadius *= startChargeRadius;

            if (!allEnemies.Contains(this))
            {
                allEnemies.Add(this);
            }
            
            SetupFSM();
        }

        protected virtual void FixedUpdate()
        {
            if (!IsAlive)
                return;

            _Fsm.FixedUpdate();
        }

        private void Update()
        {
            _Fsm.Update();
        }

        #region FSM

        public EnemyState currentState;
        public EnemyState lastState;
        protected EventFSM<EnemyState> _Fsm;
        public enum EnemyState
        {
            Idle, Blinded_Player, ChasePlayer, RageBuildup_Player, ChaseGlowstick, RageBuildup_Glowstick, CertainKillMode, Dead, Blinded_Glowstick
        }
        protected virtual void SetupFSM()
        {
            #region Declare
            
            var Idle = new State<EnemyState>("Idle");
            var Blinded_Player = new State<EnemyState>("Blinded_Player");
            var ChasePlayer = new State<EnemyState>("ChasingPlayer");
            var RageBuildup_Player = new State<EnemyState>("RageBuildup_Player");
            
            var Blinded_Glowstick = new State<EnemyState>("Blinded_Glowstick");
            var ChaseGlowstick = new State<EnemyState>("ChaseGlowstick");
            var RageBuildup_Glowstick = new State<EnemyState>("RageBuildup_Glowstick");
            
            var CertainKillMode = new State<EnemyState>("CertainKillMode");
            var Dead = new State<EnemyState>("Dead");
            

            #endregion

            #region MakeTransitions

            StateConfigurer.Create(Idle)
                .SetTransition(EnemyState.Blinded_Player, Blinded_Player)
                .SetTransition(EnemyState.ChasePlayer, ChasePlayer)
                .SetTransition(EnemyState.RageBuildup_Player, RageBuildup_Player)
                .SetTransition(EnemyState.Blinded_Glowstick, Blinded_Glowstick)
                .SetTransition(EnemyState.ChaseGlowstick, ChaseGlowstick)
                .SetTransition(EnemyState.RageBuildup_Glowstick, RageBuildup_Glowstick)
                .SetTransition(EnemyState.CertainKillMode, CertainKillMode)
                .SetTransition(EnemyState.Dead, Dead)
                .Done();
            
            StateConfigurer.Create(Blinded_Player)
                .SetTransition(EnemyState.Idle, Idle)
                .SetTransition(EnemyState.ChasePlayer, ChasePlayer)
                .SetTransition(EnemyState.RageBuildup_Player, RageBuildup_Player)
                .SetTransition(EnemyState.Blinded_Glowstick, Blinded_Glowstick)
                .SetTransition(EnemyState.ChaseGlowstick, ChaseGlowstick)
                .SetTransition(EnemyState.RageBuildup_Glowstick, RageBuildup_Glowstick)
                .SetTransition(EnemyState.CertainKillMode, CertainKillMode)
                .SetTransition(EnemyState.Dead, Dead)
                .Done();

            StateConfigurer.Create(ChasePlayer)
                .SetTransition(EnemyState.Idle, Idle)
                .SetTransition(EnemyState.Blinded_Player, Blinded_Player)
                .SetTransition(EnemyState.RageBuildup_Player, RageBuildup_Player)
                .SetTransition(EnemyState.Blinded_Glowstick, Blinded_Glowstick)
                .SetTransition(EnemyState.ChaseGlowstick, ChaseGlowstick)
                .SetTransition(EnemyState.RageBuildup_Glowstick, RageBuildup_Glowstick)
                .SetTransition(EnemyState.CertainKillMode, CertainKillMode)
                .SetTransition(EnemyState.Dead, Dead)
                .Done();

            StateConfigurer.Create(RageBuildup_Player)
                .SetTransition(EnemyState.Idle, Idle)
                .SetTransition(EnemyState.Blinded_Player, Blinded_Player)
                .SetTransition(EnemyState.ChasePlayer, ChasePlayer)
                .SetTransition(EnemyState.Blinded_Glowstick, Blinded_Glowstick)
                .SetTransition(EnemyState.ChaseGlowstick, ChaseGlowstick)
                .SetTransition(EnemyState.RageBuildup_Glowstick, RageBuildup_Glowstick)
                .SetTransition(EnemyState.CertainKillMode, CertainKillMode)
                .SetTransition(EnemyState.Dead, Dead)
                .Done();
            
            StateConfigurer.Create(CertainKillMode)
                .SetTransition(EnemyState.Idle, Idle)
                .SetTransition(EnemyState.RageBuildup_Glowstick, RageBuildup_Glowstick)
                .SetTransition(EnemyState.Blinded_Glowstick, Blinded_Glowstick)
                .SetTransition(EnemyState.Dead, Dead)
                .Done();

            StateConfigurer.Create(Dead)
                .Done();
            
            StateConfigurer.Create(Blinded_Glowstick)
                .SetTransition(EnemyState.Idle, Idle)
                .SetTransition(EnemyState.Blinded_Player, Blinded_Player)
                .SetTransition(EnemyState.ChasePlayer, ChasePlayer)
                .SetTransition(EnemyState.RageBuildup_Player, RageBuildup_Player)
                .SetTransition(EnemyState.ChaseGlowstick, ChaseGlowstick)
                .SetTransition(EnemyState.RageBuildup_Glowstick, RageBuildup_Glowstick)
                .SetTransition(EnemyState.CertainKillMode, CertainKillMode)
                .SetTransition(EnemyState.Dead, Dead)
                .Done();
            
            StateConfigurer.Create(ChaseGlowstick)
                .SetTransition(EnemyState.Idle, Idle)
                .SetTransition(EnemyState.Blinded_Player, Blinded_Player)
                .SetTransition(EnemyState.ChasePlayer, ChasePlayer)
                .SetTransition(EnemyState.RageBuildup_Player, RageBuildup_Player)
                .SetTransition(EnemyState.Blinded_Glowstick, Blinded_Glowstick)
                .SetTransition(EnemyState.RageBuildup_Glowstick, RageBuildup_Glowstick)
                .SetTransition(EnemyState.CertainKillMode, CertainKillMode)
                .SetTransition(EnemyState.Dead, Dead)
                .Done();
            
            StateConfigurer.Create(RageBuildup_Glowstick)
                .SetTransition(EnemyState.Idle, Idle)
                .SetTransition(EnemyState.Blinded_Player, Blinded_Player)
                .SetTransition(EnemyState.ChasePlayer, ChasePlayer)
                .SetTransition(EnemyState.RageBuildup_Player, RageBuildup_Player)
                .SetTransition(EnemyState.Blinded_Glowstick, Blinded_Glowstick)
                .SetTransition(EnemyState.ChaseGlowstick, ChaseGlowstick)
                .SetTransition(EnemyState.CertainKillMode, CertainKillMode)
                .SetTransition(EnemyState.Dead, Dead)
                .Done();
            
            #endregion

            #region StateBehaviour

            Idle.OnEnter += x =>
            {
                Log($"{gameObject.name}: IDLE");
                currentState = EnemyState.Idle;

                NavAgent.isStopped = true;

                TrySetAnimationTrigger(idleAnimationTrigger, "idle",false, true);
            };
            
            Idle.OnUpdate += () =>
            {
                if (HasPlayerInSight(sightRadius_Idle))
                {
                    _Fsm.SendInput(EnemyState.CertainKillMode);
                    return;
                }
                
                PlayerLightBehaviors();
                GlowstickBehaviors();
            };
            
            
            
            Blinded_Player.OnEnter += x =>
            {
                Log($"{gameObject.name}: BLINDED PLAYER");
                
                CheckAndSaveLastState();
                
                currentState = EnemyState.Blinded_Player;

                NavAgent.isStopped = true;
                NavAgent.speed = initialSpeed;
                NavAgent.velocity = Vector3.zero;

                IsInBlindAnimation = true;
                Animator.SetBool("Blinded", true);
                if (!TrySetAnimationTrigger(blindAnimationTrigger, "blind", false))
                {
                    //If you were not able to set the animation, end the animation yourself
                    //(This part is usually called at the end of the animation)
                    FinishedAnimation(Animations.Blinded);
                }
            };
            


            ChasePlayer.OnEnter += x =>
            {
                Log($"{gameObject.name}: CHASING PLAYER");
                
                currentState = EnemyState.ChasePlayer;
                
                NavAgent.isStopped = false;
                NavAgent.speed = initialSpeed;
                bool success = NavAgent.SetDestination(LastPlayerPosition);
                Debug.Assert(success);

                TrySetAnimationTrigger(chaseAnimationTrigger, "chase");
            };
            
            ChasePlayer.OnUpdate += () =>
            {
                PlayerLightBehaviors();
                GlowstickBehaviors();

                if (HasPlayerInSight(sightRadius_Active))
                {
                    _Fsm.SendInput(EnemyState.CertainKillMode);
                }
                else if (NavAgent.remainingDistance < 1f)
                {
                    _Fsm.SendInput(EnemyState.Idle);
                }
                else
                {
                    if (Time.frameCount % 10 == 0)
                    {
                        // Relcalculate path just in case.
                        bool success = NavAgent.SetDestination(LastPlayerPosition);
                        Debug.Assert(success);
                    }
                }
            };



            RageBuildup_Player.OnEnter += x =>
            {
                Log($"{gameObject.name}: RAGE BUILDUP PLAYER");
                CheckAndSaveLastState();

                bool wasInGlowstickRage = currentState == EnemyState.RageBuildup_Glowstick;
                currentState = EnemyState.RageBuildup_Player;

                NavAgent.isStopped = true;
                NavAgent.velocity = Vector3.zero;

                if (!wasInGlowstickRage)
                {
                    TrySetAnimationTrigger(rageBuildupAnimationTrigger, "rage");
                }
            };
            
            RageBuildup_Player.OnUpdate += () =>
            {
                if (CheckRage_Player())
                {
                    LoadLastState();
                }
            };
            
            
            
            Blinded_Glowstick.OnEnter += x =>
            {
                Log($"{gameObject.name}: BLINDED GLOWSTICK");
                
                CheckAndSaveLastState();
                
                currentState = EnemyState.Blinded_Glowstick;

                NavAgent.isStopped = true;
                NavAgent.speed = initialSpeed;
                NavAgent.velocity = Vector3.zero;

                IsInBlindAnimation = true;
                Animator.SetBool("Blinded", true);
                if (!TrySetAnimationTrigger(blindAnimationTrigger, "blind", false))
                {
                    //If you were not able to set the animation, end the animation yourself
                    //(This part is usually called at the end of the animation)
                    FinishedAnimation(Animations.Blinded);
                }
            };
            
            
            
            ChaseGlowstick.OnEnter += x =>
            {
                Log($"{gameObject.name}: CHASE GLOWSTICK");
                currentState = EnemyState.ChaseGlowstick;
                NavAgent.isStopped = false;
                NavAgent.speed = initialSpeed * chargingSpeedMultiplier;

                TrySetAnimationTrigger(chaseAnimationTrigger, "chase");
            };
            
            ChaseGlowstick.OnUpdate += () =>
            {
                PlayerLightBehaviors();

                if (GlowstickToChase == null)
                {
                    _Fsm.SendInput(EnemyState.Idle);
                    return;
                }
                
                bool success = NavAgent.SetDestination(GlowstickToChase.transform.position);
                Debug.Assert(success);
                
                float sqrDistance = (GlowstickToChase.transform.position - transform.position).sqrMagnitude;
                if (sqrDistance <= rageKillDistance && CheckInteraction(GlowstickToChase.transform.position))
                {
                    Destroy(GlowstickToChase.gameObject);
                    _Fsm.SendInput(EnemyState.Idle);
                }
            };
            


            RageBuildup_Glowstick.OnEnter += x =>
            {
                Log($"{gameObject.name}: RAGE BUILDUP GLOWSTICK");
                CheckAndSaveLastState();

                bool wasInPlayerRage = currentState == EnemyState.RageBuildup_Player;
                currentState = EnemyState.RageBuildup_Glowstick;

                NavAgent.isStopped = true;
                NavAgent.velocity = Vector3.zero;

                if (!wasInPlayerRage)
                {
                    TrySetAnimationTrigger(rageBuildupAnimationTrigger, "rage");
                }
            };
            
            RageBuildup_Glowstick.OnUpdate += () =>
            {
                PlayerLightBehaviors();

                if (GlowstickToChase == null)
                {
                    LoadLastState();
                }
            };
            
            
            
            CertainKillMode.OnEnter += x =>
            {
                Log($"{gameObject.name}: CERTAIN KILL");
                currentState = EnemyState.CertainKillMode;

                NavAgent.isStopped = false;
                NavAgent.speed = initialSpeed * chargingSpeedMultiplier;
                SetLastPlayerPosition();
                bool success = NavAgent.SetDestination(LastPlayerPosition);
                Debug.Assert(success);
                
                TrySetAnimationTrigger(chargeAnimationTrigger, "certainKill");

                OnCertainKill_Enter(x);
            };

            CertainKillMode.OnUpdate += () =>
            {
                GlowstickBehaviors();
                
                if (!HasPlayerInSight(sightRadius_Active))
                {
                    if (NavAgent.remainingDistance < 0.1f)
                    {
                        _Fsm.SendInput(EnemyState.Idle);
                    }
                }

                float sqrDistance = (LastPlayerPosition - transform.position).sqrMagnitude;
                if (sqrDistance <= rageKillDistance && CheckInteraction(LastPlayerPosition))
                {
                    if (tutorialEnemyBehavior) return;
                    
                    PlayerBody.Player.TakeDamage(PlayerBody.Player.currentHp);
                    return;
                }

                bool success = NavAgent.SetDestination(LastPlayerPosition);
                Debug.Assert(success);
            };
            
            CertainKillMode.OnUpdate += OnCertainKill_Update;
            
            
            
            Dead.OnEnter += x =>
            {
                
            };
            
            
            #endregion
            
            _Fsm = new EventFSM<EnemyState>(Idle);
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

        protected virtual void Melee()
        {
            StartCoroutine(Work());
            IEnumerator Work()
            {
                meleeAttack.enabled = true;
                meleePosition.enabled = true;
                yield return null;
                meleeAttack.enabled = false;
                meleePosition.enabled = false;
            }
        }
        protected abstract bool CheckRage_Player();

        protected virtual void OnCertainKill_Update()
        {
        }
        
        protected virtual void OnCertainKill_Enter(EnemyState state)
        {
            
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
                    NavAgent.isStopped = false;
                    
                    Animator.SetBool("Blinded", false);
                    IsInBlindAnimation = false;
                    
                    //LoadLastState();

                    isInStunAnimation = false; // Sometimes the flag may have a false positive if the animation was terminated abruptly.
                    
                    if (currentState == EnemyState.Blinded_Glowstick)
                    {
                        _Fsm.SendInput(EnemyState.ChaseGlowstick);
                        break;
                    }
                    
                    if(currentState == EnemyState.Blinded_Player)
                    {
                        _Fsm.SendInput(EnemyState.CertainKillMode);
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
                    if (currentState == EnemyState.RageBuildup_Glowstick)
                    {
                        _Fsm.SendInput(EnemyState.ChaseGlowstick);
                        break;
                    }
                    
                    if (currentState == EnemyState.RageBuildup_Player)
                    {
                        _Fsm.SendInput(EnemyState.CertainKillMode);
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
        
        private void OnDestroy()
        {
            if (allEnemies.Contains(this))
            {
                allEnemies.Remove(this);
            }
        }

        protected virtual void PlayerLightBehaviors()
        {
            Lantern.DistanceEffect lightEffect = Lantern.HasPlayerLightInRange(transform, BlockSight,Vector3.up * eyeOffset);
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
        
        protected virtual void LightEffect(Lantern.DistanceEffect lightEffect)
        {
            switch (lightEffect)
            {
                case Lantern.DistanceEffect.Far:
                {
                    switch (Lantern.ActiveLantern.lightType)
                    {
                        case Lantern.LightType.White:
                        {
                            break;
                        }


                        case Lantern.LightType.Red:
                        {
                            break;
                        }
                        
                        case Lantern.LightType.Blue:
                        {
                            break;
                        }
                    }
                    break;
                }

                case Lantern.DistanceEffect.Close:
                {
                    switch (Lantern.ActiveLantern.lightType)
                    {
                        case Lantern.LightType.White:
                        {
                            /*
                            NavAgent.isStopped = false;
                            NavAgent.speed = initialSpeed * escapingSpeedMultiplier;
                            SetEscapeDestination(PlayerBody.Player.transform.position);*/
                            break;
                        }


                        case Lantern.LightType.Red:
                        {
                            /*
                            NavAgent.speed = initialSpeed * 3;*/
                            break;
                        }
                        
                        case Lantern.LightType.Blue:
                        {
                            /*
                            NavAgent.isStopped = true;
                            NavAgent.speed = 0;
                    
                            GoToIdleState();*/
                            break;
                        }
                    }
                    break;
                }
            }
        }
        protected virtual void GlowstickEffect(Lantern.DistanceEffect lightEffect)
        {
            switch (lightEffect)
            {
                case Lantern.DistanceEffect.Far:
                {
                    switch (Lantern.ActiveLantern.lightType)
                    {
                        case Lantern.LightType.White:
                        {
                            break;
                        }


                        case Lantern.LightType.Red:
                        {
                            break;
                        }
                        
                        case Lantern.LightType.Blue:
                        {
                            break;
                        }
                    }
                    break;
                }

                case Lantern.DistanceEffect.Close:
                {
                    switch (Lantern.ActiveLantern.lightType)
                    {
                        case Lantern.LightType.White:
                        {
                            /*
                            NavAgent.isStopped = false;
                            NavAgent.speed = initialSpeed * escapingSpeedMultiplier;
                            SetEscapeDestination(PlayerBody.Player.transform.position);*/
                            break;
                        }


                        case Lantern.LightType.Red:
                        {
                            /*
                            NavAgent.speed = initialSpeed * 3;*/
                            break;
                        }
                        
                        case Lantern.LightType.Blue:
                        {
                            /*
                            NavAgent.isStopped = true;
                            NavAgent.speed = 0;
                    
                            GoToIdleState();*/
                            break;
                        }
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
        protected void SetEscapeDestination(Vector3 playerPosition)
        {
            float distance = float.NegativeInfinity;
            Vector3 end = default;
            bool canOscillate = default;

            Vector3 eyePosition = EyePosition;

            playerPosition.y = eyePosition.y;

            Vector3 back = (eyePosition - playerPosition).normalized;
            Vector3 forward = -back;

            Check(back, true);

            Vector3 left = new Vector3(back.z, back.y, back.x);
            Check(left, false);
            Check(back + (left * .25f), true);
            Check(back + (left * .5f), true);
            Check(back + (left * .75f), false);
            Check(back + (left * 1f), false);
            Check(back + (left * 1.5f), false);
            Check(back + (left * 2f), false);
            Check(back + (left * 4f), false);
            Check(back + (left * 8f), false);
            Check(forward + (left * 8f), false);

            Vector3 right = new Vector3(back.z, back.y, -back.x);
            Check(right, false);
            Check(back + (right * .25f), false);
            Check(back + (right * .5f), false);
            Check(back + (right * .75f), false);
            Check(back + (right * 1f), false);
            Check(back + (right * 1.5f), false);
            Check(back + (right * 2f), false);
            Check(back + (right * 4f), false);
            Check(back + (right * 8f), false);
            Check(forward + (right * 8f), false);

            void Check(Vector3 direction, bool oscillate)
            {
                if (Physics.Raycast(eyePosition, direction, out RaycastHit hit, BlockSight)
                    && hit.distance > distance)
                {
                    distance = hit.distance;
                    end = hit.point;
                    canOscillate = oscillate;
                }
            }

            if (canOscillate && oscillationFrequency != 0 && oscillationWidth != 0)
            {
                Vector3 direction = (end - eyePosition).normalized;
                Vector3 perpendicular = new Vector3(direction.z, direction.y, direction.x);
                float wave = Mathf.Sin(Time.fixedTime * oscillationFrequency) * oscillationWidth;
                Vector3 desviation = perpendicular * oscillationWidth * wave;
                Vector3 newDirection = ((direction * .5f) + desviation).normalized;

                Vector3 destination;
                if (Physics.Raycast(eyePosition, newDirection, out RaycastHit hit, 1, BlockSight))
                    destination = hit.point;
                else
                    destination = eyePosition + newDirection;

                if (NavMesh.SamplePosition(destination, out NavMeshHit hit2, 4, NavMesh.AllAreas))
                    destination = hit2.position;

                if (!NavAgent.SetDestination(destination))
                {
                    if (NavMesh.SamplePosition(end, out hit2, 4, NavMesh.AllAreas))
                        end = hit2.position;
                    bool success = NavAgent.SetDestination(end);
                    Debug.Assert(success);
                }
            }
            else
            {
                if (NavMesh.SamplePosition(end, out NavMeshHit hit, 4, NavMesh.AllAreas))
                    end = hit.position;
                bool success = NavAgent.SetDestination(end);
                Debug.Assert(success);
            }
        }
        public void TakeDamage(float amount)
        {
            if (!IsAlive)
                return;

            Try.PlayOneShoot(transform, hurtSound, "hurt");

            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                OnDeath(false);
            }
            else
                OnTakeDamage(amount, false);
        }

        public void TakeDamageWeakSpot(float amount)
        {
            if (!IsAlive)
                return;

            Try.PlayOneShoot(transform, hurtWeakspotSound, "hurt weakspot");

            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                OnDeath(true);
            }
            else
                OnTakeDamage(amount, true);
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

        protected virtual void OnTakeDamage(float amount, bool isOnWeakspot)
        {
            SetLastPlayerPosition();

            if (isOnWeakspot)
            {
                /*if (!Try.PlayOneShoot(transform, hurtWeakspotSound, "hurt weakspot"))
                    Try.PlayOneShoot(transform, hurtSound, "hurt");*/

                if (!TrySetAnimationTrigger(hurtWeakspotAnimationTrigger, "hurt weakspot", false))
                    TrySetAnimationTrigger(hurtAnimationTrigger, "hurt", false);
            }
            else
            {
                //Try.PlayOneShoot(transform, hurtSound, "hurt");
                TrySetAnimationTrigger(hurtAnimationTrigger, "hurt", false);
            }
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

        public void SendInput(EnemyState state)
        {
            _Fsm.SendInput(state);
        }
        
        [Header("Debug")]
        public bool ableToDebug = false;
        private void Log(string message)
        {
            if (ableToDebug)
            {
                Debug.Log(message);
            }
        }
        
#if UNITY_EDITOR
        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(EyePosition, SqrtOnPlay(sightRadius_Idle));
        }

        protected float SqrtOnPlay(float value) => Application.isPlaying ? Mathf.Sqrt(value) : value;
#endif
        
        private sealed class MeleeAttack : MonoBehaviour
        {
            [NonSerialized]
            public Enemy tripod;

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
            private void OnTriggerEnter(Collider other)
            {
                PlayerBody player = other.GetComponentInParent<PlayerBody>();
                if (player != null)
                    player.TakeDamage(tripod.meleeDamage);
            }
        }
    }
}