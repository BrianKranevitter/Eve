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
        
        [SerializeField, Min(0), Tooltip("Maximum health of enemy.")]
        private float maximumHealth;

        
        
        [FormerlySerializedAs("sightRadius")]
        [Header("Sight")]
        [SerializeField, Min(0), Tooltip("Determines at which radius the creature can see the player.")]
        protected float sightRadius_Idle;

        [SerializeField, Min(0), Tooltip("Determines at which radius the creature can see the player.")]
        protected float sightRadius_Active = 500;
        
        [field: SerializeField, IsProperty, Tooltip("Layer that can block enemy sight.")]
        protected LayerMask BlockSight { get; set; }

        [SerializeField, Min(0), Tooltip("Height offset of eyes.")]
        private float eyeOffset = .5f;

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
        private AudioFile hurtSound;

        [SerializeField, Tooltip("Sound played when creature is hurt on the weakspot.")]
        private AudioFile hurtWeakspotSound;

        [SerializeField, Tooltip("Sound played when creature die.")]
        private AudioFile deathSound;

        [SerializeField, Tooltip("Sound played when creature die on the weakspot.")]
        private AudioFile deathWeakspotSound;
        
        [SerializeField, Tooltip("Sound played on melee.")]
        private AudioFile meleeSound;
        
        [SerializeField, Tooltip("Sound played on shoot.")]
        private AudioFile shootSound;
        
        
        
        
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

        public void PlayAudioOneShoot(AudioUnit audio) => AudioController.PlayOneShoot(audio, transform.position);

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
            
            Lantern.DistanceEffect lightEffect = HasPlayerLightInRange();
            if (lightEffect != Lantern.DistanceEffect.None)
            {
                LightEffect(lightEffect);
            }
            
            Lantern.DistanceEffect glowstickEffect = HasGlowstickInRange();
            if (glowstickEffect != Lantern.DistanceEffect.None)
            {
                GlowstickEffect(glowstickEffect);
            }
            
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
            Idle, Blinded, HuntingPlayer, ChasingPlayer, ChargeToPlayer, Melee, Shoot, RageBuildup, CertainKillMode, Dead, ChaseGlowstick
        }
        private void SetupFSM()
        {
            #region Declare
            
            var Idle = new State<EnemyState>("Idle");
            var Blinded = new State<EnemyState>("Blinded");
            var HuntingPlayer = new State<EnemyState>("HuntingPlayer");
            var ChasingPlayer = new State<EnemyState>("ChasingPlayer");
            var ChargeToPlayer = new State<EnemyState>("ChargeToPlayer");
            var Melee = new State<EnemyState>("Melee");
            var Shoot = new State<EnemyState>("Shoot");
            var RageBuildup = new State<EnemyState>("RageBuildup");
            var CertainKillMode = new State<EnemyState>("CertainKillMode");
            var Dead = new State<EnemyState>("Dead");
            var ChaseGlowstick = new State<EnemyState>("ChaseGlowstick");

            #endregion

            #region MakeTransitions

            StateConfigurer.Create(Idle)
                .SetTransition(EnemyState.Blinded, Blinded)
                .SetTransition(EnemyState.HuntingPlayer, HuntingPlayer)
                .SetTransition(EnemyState.ChasingPlayer, ChasingPlayer)
                .SetTransition(EnemyState.ChargeToPlayer, ChargeToPlayer)
                .SetTransition(EnemyState.Melee, Melee)
                .SetTransition(EnemyState.Shoot, Shoot)
                .SetTransition(EnemyState.RageBuildup, RageBuildup)
                .SetTransition(EnemyState.CertainKillMode, CertainKillMode)
                .SetTransition(EnemyState.Dead, Dead)
                .SetTransition(EnemyState.ChaseGlowstick, ChaseGlowstick)
                .Done();
            
            StateConfigurer.Create(Blinded)
                .SetTransition(EnemyState.Idle, Idle)
                .SetTransition(EnemyState.HuntingPlayer, HuntingPlayer)
                .SetTransition(EnemyState.ChasingPlayer, ChasingPlayer)
                .SetTransition(EnemyState.ChargeToPlayer, ChargeToPlayer)
                .SetTransition(EnemyState.Melee, Melee)
                .SetTransition(EnemyState.Shoot, Shoot)
                .SetTransition(EnemyState.RageBuildup, RageBuildup)
                .SetTransition(EnemyState.CertainKillMode, CertainKillMode)
                .SetTransition(EnemyState.Dead, Dead)
                .SetTransition(EnemyState.ChaseGlowstick, ChaseGlowstick)
                .Done();
            
            StateConfigurer.Create(HuntingPlayer)
                .SetTransition(EnemyState.Idle, Idle)
                .SetTransition(EnemyState.Blinded, Blinded)
                .SetTransition(EnemyState.ChasingPlayer, ChasingPlayer)
                .SetTransition(EnemyState.ChargeToPlayer, ChargeToPlayer)
                .SetTransition(EnemyState.Melee, Melee)
                .SetTransition(EnemyState.Shoot, Shoot)
                .SetTransition(EnemyState.RageBuildup, RageBuildup)
                .SetTransition(EnemyState.CertainKillMode, CertainKillMode)
                .SetTransition(EnemyState.Dead, Dead)
                .SetTransition(EnemyState.ChaseGlowstick, ChaseGlowstick)
                .Done();
            
            StateConfigurer.Create(ChasingPlayer)
                .SetTransition(EnemyState.Idle, Idle)
                .SetTransition(EnemyState.Blinded, Blinded)
                .SetTransition(EnemyState.HuntingPlayer, HuntingPlayer)
                .SetTransition(EnemyState.ChargeToPlayer, ChargeToPlayer)
                .SetTransition(EnemyState.Melee, Melee)
                .SetTransition(EnemyState.Shoot, Shoot)
                .SetTransition(EnemyState.RageBuildup, RageBuildup)
                .SetTransition(EnemyState.CertainKillMode, CertainKillMode)
                .SetTransition(EnemyState.Dead, Dead)
                .SetTransition(EnemyState.ChaseGlowstick, ChaseGlowstick)
                .Done();
            
            StateConfigurer.Create(ChargeToPlayer)
                .SetTransition(EnemyState.Idle, Idle)
                .SetTransition(EnemyState.Blinded, Blinded)
                .SetTransition(EnemyState.HuntingPlayer, HuntingPlayer)
                .SetTransition(EnemyState.ChasingPlayer, ChasingPlayer)
                .SetTransition(EnemyState.Melee, Melee)
                .SetTransition(EnemyState.Shoot, Shoot)
                .SetTransition(EnemyState.RageBuildup, RageBuildup)
                .SetTransition(EnemyState.CertainKillMode, CertainKillMode)
                .SetTransition(EnemyState.Dead, Dead)
                .SetTransition(EnemyState.ChaseGlowstick, ChaseGlowstick)
                .Done();
            
            StateConfigurer.Create(Melee)
                .SetTransition(EnemyState.Idle, Idle)
                .SetTransition(EnemyState.Blinded, Blinded)
                .SetTransition(EnemyState.HuntingPlayer, HuntingPlayer)
                .SetTransition(EnemyState.ChasingPlayer, ChasingPlayer)
                .SetTransition(EnemyState.ChargeToPlayer, ChargeToPlayer)
                .SetTransition(EnemyState.Shoot, Shoot)
                .SetTransition(EnemyState.RageBuildup, RageBuildup)
                .SetTransition(EnemyState.CertainKillMode, CertainKillMode)
                .SetTransition(EnemyState.Dead, Dead)
                .SetTransition(EnemyState.ChaseGlowstick, ChaseGlowstick)
                .Done();
            
            StateConfigurer.Create(Shoot)
                .SetTransition(EnemyState.Idle, Idle)
                .SetTransition(EnemyState.Blinded, Blinded)
                .SetTransition(EnemyState.HuntingPlayer, HuntingPlayer)
                .SetTransition(EnemyState.ChasingPlayer, ChasingPlayer)
                .SetTransition(EnemyState.ChargeToPlayer, ChargeToPlayer)
                .SetTransition(EnemyState.Melee, Melee)
                .SetTransition(EnemyState.RageBuildup, RageBuildup)
                .SetTransition(EnemyState.CertainKillMode, CertainKillMode)
                .SetTransition(EnemyState.Dead, Dead)
                .SetTransition(EnemyState.ChaseGlowstick, ChaseGlowstick)
                .Done();
            
            StateConfigurer.Create(RageBuildup)
                .SetTransition(EnemyState.Idle, Idle)
                .SetTransition(EnemyState.Blinded, Blinded)
                .SetTransition(EnemyState.HuntingPlayer, HuntingPlayer)
                .SetTransition(EnemyState.ChasingPlayer, ChasingPlayer)
                .SetTransition(EnemyState.ChargeToPlayer, ChargeToPlayer)
                .SetTransition(EnemyState.Melee, Melee)
                .SetTransition(EnemyState.Shoot, Shoot)
                .SetTransition(EnemyState.CertainKillMode, CertainKillMode)
                .SetTransition(EnemyState.Dead, Dead)
                .SetTransition(EnemyState.ChaseGlowstick, ChaseGlowstick)
                .Done();
            
            StateConfigurer.Create(CertainKillMode)
                .SetTransition(EnemyState.Idle, Idle)
                .SetTransition(EnemyState.Dead, Dead)
                .Done();

            StateConfigurer.Create(Dead)
                .Done();
            
            StateConfigurer.Create(ChaseGlowstick)
                .SetTransition(EnemyState.Idle, Idle)
                .SetTransition(EnemyState.Blinded, Blinded)
                .SetTransition(EnemyState.HuntingPlayer, HuntingPlayer)
                .SetTransition(EnemyState.ChasingPlayer, ChasingPlayer)
                .SetTransition(EnemyState.ChargeToPlayer, ChargeToPlayer)
                .SetTransition(EnemyState.Melee, Melee)
                .SetTransition(EnemyState.Shoot, Shoot)
                .SetTransition(EnemyState.RageBuildup, RageBuildup)
                .SetTransition(EnemyState.CertainKillMode, CertainKillMode)
                .SetTransition(EnemyState.Dead, Dead)
                .Done();
            
            #endregion

            #region StateBehaviour

            Idle.OnEnter += x =>
            {
                Debug.Log($"{gameObject.name}: IDLE");
                currentState = EnemyState.Idle;

                NavAgent.isStopped = true;

                TrySetAnimationTrigger(idleAnimationTrigger, "idle");
            };
            
            Idle.OnFixedUpdate += () =>
            {
                if (HasPlayerInSight(sightRadius_Idle))
                    _Fsm.SendInput(EnemyState.HuntingPlayer);
            };
            
            
            
            Blinded.OnEnter += x =>
            {
                Debug.Log($"{gameObject.name}: BLINDED");
                
                if (currentState != EnemyState.Blinded && currentState != EnemyState.RageBuildup)
                {
                    lastState = currentState;
                }
                
                currentState = EnemyState.Blinded;
                
                SetLastPlayerPosition();

                NavAgent.isStopped = true;
                NavAgent.velocity = Vector3.zero;

                IsInBlindAnimation = true;
                Animator.SetBool("Blinded", true);
                if (!TrySetAnimationTrigger(blindAnimationTrigger, "blind", false))
                {
                    //If you were not able to set the animation, end the animation yourself
                    //(This part is usually called at the end of the animation)
                    FinishedAnimation(EnemyState.Blinded);
                }
                    
                
            };



            HuntingPlayer.OnEnter += x =>
            {
                Debug.Log($"{gameObject.name}: HUNTING PLAYER");
                currentState = EnemyState.HuntingPlayer;
                NavAgent.isStopped = false;
                NavAgent.speed = initialSpeed;
                bool success = NavAgent.SetDestination(LastPlayerPosition);
                Debug.Assert(success);

                TrySetAnimationTrigger(huntAnimationTrigger, "hunt");
            };
            
            HuntingPlayer.OnFixedUpdate += () =>
            {
                if (!HasPlayerInSight(sightRadius_Active))
                {
                    _Fsm.SendInput(EnemyState.ChasingPlayer);
                    return;
                }

                bool success = NavAgent.SetDestination(LastPlayerPosition);
                Debug.Assert(success);
                
                
                float sqrDistance = (LastPlayerPosition - transform.position).sqrMagnitude;
                if (sqrDistance <= startMeleeRadius)
                {
                    _Fsm.SendInput(EnemyState.Melee);
                }
                else if (sqrDistance <= startChargeRadius)
                    _Fsm.SendInput(EnemyState.ChargeToPlayer);
                else if (sqrDistance <= startShootingRadius)
                {
                    _Fsm.SendInput(EnemyState.Shoot);
                }
            };

            
            
            
            ChasingPlayer.OnEnter += x =>
            {
                Debug.Log($"{gameObject.name}: CHASING PLAYER");
                
                currentState = EnemyState.ChasingPlayer;
                NavAgent.isStopped = false;
                NavAgent.speed = initialSpeed;
                bool success = NavAgent.SetDestination(LastPlayerPosition);
                Debug.Assert(success);

                TrySetAnimationTrigger(chaseAnimationTrigger, "chase");
            };
            
            ChasingPlayer.OnFixedUpdate += () =>
            {
                if (HasPlayerInSight(sightRadius_Active))
                {
                    float sqrDistance = (LastPlayerPosition - transform.position).sqrMagnitude;
                    if (sqrDistance <= startMeleeRadius)
                    {
                        _Fsm.SendInput(EnemyState.Melee);
                    }
                    else if (sqrDistance <= startChargeRadius)
                    {
                        _Fsm.SendInput(EnemyState.ChargeToPlayer);
                    }
                    else if (sqrDistance <= startShootingRadius)
                    {
                        _Fsm.SendInput(EnemyState.Shoot);
                    }
                    else
                    {
                        _Fsm.SendInput(EnemyState.HuntingPlayer);
                    }
                }
                else if (NavAgent.remainingDistance == 0)
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
            
            
            
            
            ChargeToPlayer.OnEnter += x =>
            {
                Debug.Log($"{gameObject.name}: CHARGE TO PLAYER");
                currentState = EnemyState.ChasingPlayer;
                NavAgent.isStopped = false;
                NavAgent.speed = initialSpeed * chargingSpeedMultiplier;
                bool success = NavAgent.SetDestination(LastPlayerPosition);
                Debug.Assert(success);

                TrySetAnimationTrigger(chargeAnimationTrigger, "charge");
            };
            
            ChargeToPlayer.OnFixedUpdate += () =>
            {
                if (!HasPlayerInSight(sightRadius_Active))
                {
                    _Fsm.SendInput(EnemyState.ChasingPlayer);
                    return;
                }

                float sqrDistance = (LastPlayerPosition - transform.position).sqrMagnitude;
                if (sqrDistance <= startMeleeRadius)
                {
                    _Fsm.SendInput(EnemyState.Melee);
                    return;
                }

                bool success = NavAgent.SetDestination(LastPlayerPosition);
                Debug.Assert(success);
            };
            
            
            
            
            Melee.OnEnter += x =>
            {
                Debug.Log($"{gameObject.name}: MELEE");
                currentState = EnemyState.Melee;
                isInMeleeAnimation = false; // Sometimes the flag may have a false positive if the animation was terminated abruptly.
                NavAgent.isStopped = true;
                NavAgent.velocity = Vector3.zero;
            };
            
            Melee.OnFixedUpdate += () =>
            {
                if (isInMeleeAnimation)
                    return;

                if (!HasPlayerInSight(sightRadius_Active))
                {
                    _Fsm.SendInput(EnemyState.ChasingPlayer);
                    return;
                }

                LookAtPlayer();

                float sqrDistance = (LastPlayerPosition - transform.position).sqrMagnitude;
                if (sqrDistance > startMeleeRadius)
                {
                    _Fsm.SendInput(EnemyState.HuntingPlayer);
                    return;
                }

                isInMeleeAnimation = true;

                Try.PlayOneShoot(transform, meleeSound, "melee");

                if (!TrySetAnimationTrigger(meleeAnimationTrigger, "melee"))
                {
                    this.Melee();
                    FinishedAnimation(EnemyState.Melee);
                }
            };




            Shoot.OnEnter += x =>
            {
                Debug.Log($"{gameObject.name}: SHOOT");
                currentState = EnemyState.Shoot;
                isInShootingAnimation = false; // Sometimes the flag may have a false positive if the animation was terminated abruptly.
                NavAgent.isStopped = true;
            };

            Shoot.OnFixedUpdate += () =>
            {
                if (isInShootingAnimation)
                    return;

                if (!HasPlayerInSight(sightRadius_Active))
                {
                    _Fsm.SendInput(EnemyState.ChasingPlayer);
                    return;
                }

                float sqrDistance = (LastPlayerPosition - transform.position).sqrMagnitude;
                
                if (sqrDistance > stopShootingRadius)
                {
                    _Fsm.SendInput(EnemyState.HuntingPlayer);
                    return;
                }
                else if (sqrDistance <= startMeleeRadius)
                {
                    _Fsm.SendInput(EnemyState.Melee);
                    return;
                }
                else if (sqrDistance <= startChargeRadius)
                {
                    _Fsm.SendInput(EnemyState.ChargeToPlayer);
                    return;
                }

                LookAtPlayer();

                isInShootingAnimation = true;

                Try.PlayOneShoot(transform, shootSound, "shoot");

                if (!TrySetAnimationTrigger(shootAnimationTrigger, "shoot"))
                {
                    FinishedAnimation(EnemyState.Shoot);
                }
            };
            
            
            
            
            RageBuildup.OnEnter += x =>
            {
                Debug.Log($"{gameObject.name}: RAGE BUILDUP");
                if (currentState != EnemyState.Blinded && currentState != EnemyState.RageBuildup)
                {
                    lastState = currentState;
                }
                
                currentState = EnemyState.RageBuildup;

                NavAgent.isStopped = true;
                NavAgent.velocity = Vector3.zero;
                
                TrySetAnimationTrigger(rageBuildupAnimationTrigger, "rage");
            };
            
            RageBuildup.OnFixedUpdate += () =>
            {
                if (CheckRage())
                {
                    LoadLastState();
                }
            };
            
            
            
            
            CertainKillMode.OnEnter += x =>
            {
                Debug.Log($"{gameObject.name}: CERTAIN KILL");
                currentState = EnemyState.CertainKillMode;

                NavAgent.isStopped = false;
                NavAgent.speed = initialSpeed * chargingSpeedMultiplier;
                bool success = NavAgent.SetDestination(LastPlayerPosition);
                Debug.Assert(success);
                
                LookAtPlayer();
                TrySetAnimationTrigger(chargeAnimationTrigger, "certainKill");
            };
            CertainKillMode.OnEnter += OnCertainKill_Enter;
            
            CertainKillMode.OnFixedUpdate += () =>
            {
                if (!HasPlayerInSight(sightRadius_Active))
                {
                    _Fsm.SendInput(EnemyState.ChasingPlayer);
                    return;
                }

                float sqrDistance = (LastPlayerPosition - transform.position).sqrMagnitude;
                if (sqrDistance <= rageKillDistance)
                {
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
            
            ChaseGlowstick.OnEnter += x =>
            {
                Debug.Log($"{gameObject.name}: CHARGE TO GLOWSTICK");
                currentState = EnemyState.ChasingPlayer;
                NavAgent.isStopped = false;
                NavAgent.speed = initialSpeed * chargingSpeedMultiplier;
                bool success = NavAgent.SetDestination(GlowstickToChase.transform.position);
                Debug.Assert(success);

                TrySetAnimationTrigger(chargeAnimationTrigger, "charge");
            };
            
            ChaseGlowstick.OnFixedUpdate += () =>
            {
                float sqrDistance = (GlowstickToChase.transform.position - transform.position).sqrMagnitude;
                if (sqrDistance <= startMeleeRadius)
                {
                    Destroy(GlowstickToChase.gameObject);
                    _Fsm.SendInput(EnemyState.Idle);
                    return;
                }
                
                bool success = NavAgent.SetDestination(GlowstickToChase.transform.position);
                Debug.Assert(success);
            };
            #endregion
            
            _Fsm = new EventFSM<EnemyState>(Idle);
        }

        protected void LoadLastState()
        {
            Debug.Log("TRIPOD LOAD LAST STATE");
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
        protected abstract bool CheckRage();

        protected virtual void OnCertainKill_Update()
        {
        }
        
        protected virtual void OnCertainKill_Enter(EnemyState state)
        {
            
        }
        
        public void FinishedAnimation(EnemyState state)
        {
            switch (state)
            {
                case EnemyState.Blinded:
                {
                    Lantern.DistanceEffect light = HasPlayerLightInRange();
                    Debug.Log("LIGHT WAS " + light);
                    
                    if (light == Lantern.DistanceEffect.Close &&
                        Lantern.ActiveLantern.lightType == Lantern.LightType.Red)
                    {
                        
                        break;
                    }
                    
                    NavAgent.isStopped = false;
                    
                    Animator.SetBool("Blinded", false);
                    IsInBlindAnimation = false;
                    
                    LoadLastState();

                    isInStunAnimation = false; // Sometimes the flag may have a false positive if the animation was terminated abruptly.
                    if (currentState == EnemyState.Idle)
                        _Fsm.SendInput(EnemyState.ChasingPlayer);
                    break;
                }

                case EnemyState.Melee:
                {
                    isInMeleeAnimation = false;
                    break;
                }

                case EnemyState.RageBuildup:
                {
                    if (GlowstickToChase != null)
                    {
                        _Fsm.SendInput(EnemyState.ChaseGlowstick);
                    }
                    else
                    {
                        _Fsm.SendInput(EnemyState.CertainKillMode);
                    }
                    
                    break;
                }

                case EnemyState.Shoot:
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
                case EnemyState.ChasingPlayer:
                    bool result = NavAgent.SetDestination(LastPlayerPosition);
                    Debug.Assert(result);
                    break;
                case EnemyState.Idle:
                    _Fsm.SendInput(EnemyState.ChasingPlayer);
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