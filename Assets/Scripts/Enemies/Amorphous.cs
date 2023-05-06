using System;
using System.Collections;
using System.Collections.Generic;
using Game.Enemies;
using Game.Player.Weapons;
using Kam.Utils.FSM;
using UnityEngine;

public class Amorphous : MonoBehaviour
{
    [Header("Settings")]
    public float backSpeed;
    public float forwardSpeed;

    [Header("Body Animation")]
    public Animator bodyAnimator;
    public string idleAnimationTrigger;
    public string movingForwardAnimationTrigger;
    public string movingBackwardsAnimationTrigger;
    
    [Header("Eyes Animation")]
    public Animator eyesAnimator;
    public string openingEyesAnimationStateName;
    public string closingEyesAnimationStateName;

    [Header("Movement")]
    public Transform backWaypoint;
    public Transform forwardWaypoint;
    public Axis axisMovementType;
    public float arriveDistance;

    [Header("Light Detection")]
    public LayerMask BlockSight;
    
    public enum Axis
    {
        X, Y, Z, XY, XZ, YZ, XYZ
    }

    private void Awake()
    {
        SetupFSM();
    }

    private void Update()
    {
        _AmorphousFSM.Update();
    }

    #region FSM

        public EnemyState currentState;
        protected EventFSM<EnemyState> _AmorphousFSM;
        public enum EnemyState
        {
            Idle, Back, Forward, Dead
        }
        protected void SetupFSM()
        {
            #region Declare
            
            var Idle = new State<EnemyState>("Idle");
            var Back = new State<EnemyState>("Back");
            var Forward = new State<EnemyState>("Forward");
            var Dead = new State<EnemyState>("Dead");
            
            #endregion

            #region MakeTransitions

            StateConfigurer.Create(Idle)
                .SetTransition(EnemyState.Back, Back)
                .SetTransition(EnemyState.Forward, Forward)
                .SetTransition(EnemyState.Dead, Dead)
                .Done();
            
            StateConfigurer.Create(Back)
                .SetTransition(EnemyState.Idle, Idle)
                .SetTransition(EnemyState.Forward, Forward)
                .SetTransition(EnemyState.Dead, Dead)
                .Done();

            StateConfigurer.Create(Forward)
                .SetTransition(EnemyState.Idle, Idle)
                .SetTransition(EnemyState.Dead, Dead)
                .Done();

            StateConfigurer.Create(Dead)
                .Done();

            #endregion

            #region StateBehaviour

            Idle.OnEnter += x =>
            {
                Log($"{gameObject.name}: IDLE");
                currentState = EnemyState.Idle;

                bodyAnimator.SetTrigger(idleAnimationTrigger);

                eyesAnimator.SetFloat("Speed", -1);
                var info = eyesAnimator.GetCurrentAnimatorStateInfo(0);
                if (info.normalizedTime > 1f)
                {
                    eyesAnimator.Play(openingEyesAnimationStateName,0,1f);
                }

                StopMoving();
            };
            
            Idle.OnUpdate += () =>
            {
                PlayerLightBehaviors();
            };

            Back.OnEnter += state =>
            {
                Log($"{gameObject.name}: MOVING BACK");
                currentState = EnemyState.Back;

                bodyAnimator.SetTrigger(movingBackwardsAnimationTrigger);

                eyesAnimator.SetFloat("Speed", 1);
                var info = eyesAnimator.GetCurrentAnimatorStateInfo(0);
                if (info.normalizedTime < 0)
                {
                    eyesAnimator.Play(openingEyesAnimationStateName,0,0f);
                }


                MoveTowards(backWaypoint,axisMovementType,backSpeed,arriveDistance,()=>{_AmorphousFSM.SendInput(EnemyState.Idle);});
            };
            
            Back.OnUpdate += () =>
            {
                PlayerLightBehaviors();
            };
            
            Forward.OnEnter += state =>
            {
                Log($"{gameObject.name}: MOVING FORWARD");
                currentState = EnemyState.Forward;

                bodyAnimator.SetTrigger(movingForwardAnimationTrigger);
                
                MoveTowards(forwardWaypoint,axisMovementType,forwardSpeed,arriveDistance,()=>{_AmorphousFSM.SendInput(EnemyState.Idle);});
            };
            
            Forward.OnUpdate += () =>
            {
                
            };
            
            #endregion
            
            _AmorphousFSM = new EventFSM<EnemyState>(Idle);
        }
        
        #endregion
    private void LightEffect(Lantern.DistanceEffect lightEffect)
    {
        switch (lightEffect)
        {
            case Lantern.DistanceEffect.None:
                _AmorphousFSM.SendInput(EnemyState.Idle);
                break;
            
            case Lantern.DistanceEffect.Far:
            case Lantern.DistanceEffect.Close:
            {
                switch (Lantern.ActiveLantern.lightType)
                {
                    case Lantern.LightType.White:
                    {
                        //Move back
                        _AmorphousFSM.SendInput(EnemyState.Back);
                        break;
                    }

                    case Lantern.LightType.Red:
                    {
                        //Rage, move forward
                        _AmorphousFSM.SendInput(EnemyState.Forward);
                        break;
                    }
                }
                break;
            }
        }
    }

    protected virtual void PlayerLightBehaviors()
    {
        Lantern.DistanceEffect lightEffect = Lantern.HasPlayerLightInRange(transform, BlockSight);
        LightEffect(lightEffect);
    }


    public void MoveForward()
    {
        _AmorphousFSM.SendInput(EnemyState.Forward);
    }
    public void StopMoving()
    {
        if (moveTowards != null)
        {
            StopCoroutine(moveTowards);
        }
        moveTowards = null;
    }
    
    public void MoveTowards(Transform point, Axis axis, float speed, float arriveDistance, Action onArrive)
    {
        if (moveTowards != null)
        {
            StopCoroutine(moveTowards);
        }
        
        moveTowards = StartCoroutine(MoveTowards_Coroutine(point, axis, speed, arriveDistance, onArrive));
    }
    
    private Coroutine moveTowards = null;
    Vector3 destination = Vector3.zero;
    private IEnumerator MoveTowards_Coroutine(Transform point, Axis axis, float speed, float arriveDistance, Action onArrive)
    {
        switch (axis)
        {
            case Axis.X:
                destination = new Vector3(point.position.x, transform.position.y, transform.position.z);
                break;
            
            case Axis.Y:
                destination = new Vector3(transform.position.x, point.position.y, transform.position.z);
                break;
            
            case Axis.Z:
                destination = new Vector3(transform.position.x, transform.position.y, point.position.z);
                break;
            
            case Axis.XY:
                destination = new Vector3(point.position.x, point.position.y, transform.position.z);
                break;
            
            case Axis.XZ:
                destination = new Vector3(point.position.x, transform.position.y, point.position.z);
                break;
            
            case Axis.YZ:
                destination = new Vector3(transform.position.x, point.position.y, point.position.z);
                break;
            
            case Axis.XYZ:
                destination = point.position;
                break;
        }
        
        while (Vector3.Distance(transform.position, destination) > arriveDistance)
        {
            transform.position += (destination - transform.position).normalized * speed * Time.deltaTime;

            yield return null;
        }

        transform.position = destination;
        onArrive.Invoke();
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
}
