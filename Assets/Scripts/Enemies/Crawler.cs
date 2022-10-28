using Enderlook.Unity.AudioManager;

using Game.Player;
using Game.Utility;

using System;
using System.Collections;
using Game.Player.Weapons;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Enemies
{
    public sealed class Crawler : Enemy
    {
        [Header("Animation Triggers")]
        [SerializeField, Tooltip("The mouth object with animation of acid puke.")]
        private Animator mouthAnimator;
        
        [SerializeField, Tooltip("Name of the animation trigger for mouth.")]
        private string mouthAnimationTrigger;
        
        public void TriggerMouthAttackAnimation()
        {
            if (string.IsNullOrEmpty(mouthAnimationTrigger))
                Debug.LogWarning("Missing mouth animation trigger.");
            else
                mouthAnimator.SetTrigger(mouthAnimationTrigger);
        }

        protected override void LightEffect(Lantern.DistanceEffect lightEffect)
        {
            switch (lightEffect)
            {
                case Lantern.DistanceEffect.Far:
                {
                    //Rage
                    _Fsm.SendInput(EnemyState.RageBuildup);
                    break;
                }

                case Lantern.DistanceEffect.Close:
                {
                    switch (Lantern.ActiveLantern.lightType)
                    {
                        case Lantern.LightType.White:
                        {
                            //Stunned
                            _Fsm.SendInput(EnemyState.Blinded);
                            break;
                        }

                        case Lantern.LightType.Red:
                        {
                            //Rage
                            _Fsm.SendInput(EnemyState.RageBuildup);
                            break;
                        }
                    }
                    break;
                }
            }
            
            base.LightEffect(lightEffect);
        }
        
        protected override void GlowstickEffect(Lantern.DistanceEffect lightEffect)
        {
            switch (lightEffect)
            {
                case Lantern.DistanceEffect.Far:
                {
                    if (currentState == EnemyState.Idle)
                    {
                        //Rage
                        _Fsm.SendInput(EnemyState.RageBuildup);
                    }
                    break;
                }

                case Lantern.DistanceEffect.Close:
                {
                    if (currentState == EnemyState.Idle)
                    {
                        //Stunned
                        _Fsm.SendInput(EnemyState.Blinded);
                    }
                    break;
                }
            }

            base.GlowstickEffect(lightEffect);
        }

        protected override bool CheckRage()
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
        private bool affectedByLight;
        private int flashCount = 0;
        private float timeSinceStart = 0;
        protected override void OnCertainKill_Update()
        {
            base.OnCertainKill_Update();
            
            if (HasPlayerLightInRange() != Lantern.DistanceEffect.Close && Lantern.ActiveLantern.lightType == Lantern.LightType.White)
            {
                if (!affectedByLight)
                {
                    affectedByLight = true;
                    
                    flashCount++;
                    
                    if (flashCount > flashingAmount)
                    {
                        _Fsm.SendInput(EnemyState.Idle);
                    }
                }
            }
            else
            {
                affectedByLight = false;
            }

            if (flashCount > 0)
            {
                timeSinceStart += Time.deltaTime;

                if (timeSinceStart > flashingTime)
                {
                    flashCount = 0;
                    timeSinceStart = 0;
                }
            }
        }

        protected override void OnCertainKill_Enter(EnemyState state)
        {
            base.OnCertainKill_Enter(state);
            
            timeSinceStart = 0;
        }
    }
}