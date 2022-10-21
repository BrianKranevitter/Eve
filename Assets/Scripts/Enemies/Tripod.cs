using Enderlook.Unity.AudioManager;

using Game.Player;
using Game.Utility;

using System;
using System.Collections;
using System.Linq;
using Game.Player.Weapons;
using Kam.Utils.FSM;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Enemies
{
    public sealed class Tripod : Enemy
    {
        protected override void LightEffect(Lantern.DistanceEffect lightEffect)
        {
            switch (lightEffect)
            {
                case Lantern.DistanceEffect.Far:
                {
                    //Rage Animation
                    _Fsm.SendInput(EnemyState.RageBuildup);
                    break;
                }

                case Lantern.DistanceEffect.Close:
                {
                    switch (Lantern.ActiveLantern.lightType)
                    {
                        case Lantern.LightType.White:
                        {
                            //Rage
                            _Fsm.SendInput(EnemyState.RageBuildup);
                            break;
                        }

                        case Lantern.LightType.Red:
                        {
                            //Stunned
                            _Fsm.SendInput(EnemyState.Blinded);
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
                         Lantern.ActiveLantern.lightType == Lantern.LightType.White;

            bool case2 = distance == Lantern.DistanceEffect.Far &&
                         Lantern.ActiveLantern.lightType == Lantern.LightType.White;
                
            bool case3 = distance == Lantern.DistanceEffect.Far &&
                         Lantern.ActiveLantern.lightType == Lantern.LightType.Red;
                
            //If you are not in any of these cases, you are enraged.
            return !(case1 || case2 || case3);
        }

        protected override void OnTakeDamage(float amount, bool isOnWeakspot)
        {
            base.OnTakeDamage(amount, isOnWeakspot);
            if (isOnWeakspot && HasPlayerInSight())
                _Fsm.SendInput(EnemyState.ChargeToPlayer);
            else if (currentState == EnemyState.Idle)
                _Fsm.SendInput(EnemyState.ChasingPlayer);

            if (!IsAlive)
            {
                _Fsm.SendInput(EnemyState.Dead);
            }
        }
    }
}