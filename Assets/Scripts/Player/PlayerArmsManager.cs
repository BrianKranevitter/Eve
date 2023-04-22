using System;
using System.Collections;
using System.Collections.Generic;
using Game.Player.Weapons;
using UnityEngine;

public class PlayerArmsManager : MonoBehaviour
{
    public static PlayerArmsManager i;
    public static bool HasPickedUpBattery = false;
    public static bool FirstTimeOutOfBattery = true;
    public static bool ableToShowInfo = true;
    
    [Header("Left")]
    public PlayerInstructionArm instructionArm;
    public PlayerThrow throwArm;
    public GameObject batteryFirstTimePickup;
    
    [Header("Right")]
    public GameObject headArm;
    private Animator headArmAnimator;
    
    [Header("Both")]
    public GameObject batteryRechargeArms;
    

    private void Awake()
    {
        i = this;
        headArmAnimator = headArm.GetComponent<Animator>();
    }

    public void InstructionArmAnim(Action callback)
    {
        CheckLeftArm(callback);
    }

    public void InstantCooldownBatteryAnim()
    {
        CheckBothArms(delegate { 
            headArm.SetActive(false);
            batteryRechargeArms.SetActive(true);
        });
        /*
        instructionArm.HideInfo(delegate {  
            headArm.SetActive(false);
            batteryRechargeArms.SetActive(true);
            instructionArm.ableToShowInfo = false;
        });*/
        
    }

    public void FromBatteryRecharge()
    {
        headArm.SetActive(true);
    }

    public void FirstTimeBatteryPickupAnim()
    {
        if (!HasPickedUpBattery)
        {
            CheckLeftArm(delegate
            {
                batteryFirstTimePickup.SetActive(true);
            });
            
            HasPickedUpBattery = true;
        }
    }
    public void FromFirstTimeBatteryPickup()
    {
        batteryFirstTimePickup.SetActive(false);
    }

    void CheckLeftArm(Action callback)
    {
        StartCoroutine(WaitingForLeftArm(callback));
    }

    IEnumerator WaitingForLeftArm(Action callback)
    {
        bool waitingForHideInfo = true;

        if (instructionArm.active)
        {
            instructionArm.HideInfo(delegate
            {
                waitingForHideInfo = false;
            });
        }
        else
        {
            waitingForHideInfo = false;
        }

        while (waitingForHideInfo || batteryFirstTimePickup.activeSelf || batteryRechargeArms.activeSelf)
        {
            yield return null;
        }
        
        callback.Invoke();
    }
    public void CheckRightArm(Action callback)
    {
        StartCoroutine(WaitingForRightArm(callback));
    }

    private bool waitingForSyncAnimation_RightArm = false;
    IEnumerator WaitingForRightArm(Action callback)
    {
        waitingForSyncAnimation_RightArm = true;
        headArmAnimator.SetTrigger("Sync");
        Debug.Log("Waiting");
        while (waitingForSyncAnimation_RightArm)
        {
            yield return null;
        }
        
        Debug.Log("Done");
        callback.Invoke();
    }

    public void AnimationEvent_SyncConfirmed()
    {
        waitingForSyncAnimation_RightArm = false;
    }
    
    public void CheckBothArms(Action callback)
    {
        StartCoroutine(WaitingForBothArms(callback));
    }
    
    IEnumerator WaitingForBothArms(Action callback)
    {
        bool waitingForLeftArm = true;
        CheckLeftArm(delegate{ waitingForLeftArm = false; });

        bool waitingForRightArm = true;
        CheckRightArm(delegate { waitingForRightArm = false; });

        while (waitingForLeftArm || waitingForRightArm)
        {
            yield return null;
        }
        
        callback.Invoke();
    }
}
