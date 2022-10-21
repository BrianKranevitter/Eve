using Enderlook.Unity.AudioManager;

using Game.Player;
using Game.Utility;

using System;
using System.Collections;
using Game.Player.Weapons;
using UnityEngine;

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

        protected override bool CheckRage()
        {
            Lantern.DistanceEffect distance = HasLightInRange();
                
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
    }
}