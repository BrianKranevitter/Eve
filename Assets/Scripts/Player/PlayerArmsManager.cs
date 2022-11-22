using System;
using System.Collections;
using System.Collections.Generic;
using Game.Player.Weapons;
using UnityEngine;

public class PlayerArmsManager : MonoBehaviour
{
    public static PlayerArmsManager i;
    public static bool HasPickedUpBattery = false;
    
    [Header("Left")]
    public PlayerInstructionArm instructionArm;
    public PlayerThrow throwArm;
    public GameObject batteryFirstTimePickup;
    
    [Header("Right")]
    public GameObject headArm;
    
    [Header("Both")]
    public GameObject batteryRechargeArms;
    

    private void Awake()
    {
        i = this;
    }

    public void RechargeBatteryAnim()
    {
        instructionArm.HideInfo();
        headArm.SetActive(false);
        batteryRechargeArms.SetActive(true);
    }

    public void FromBatteryRecharge()
    {
        headArm.SetActive(true);
    }

    public void FirstTimeBatteryPickupAnim()
    {
        if (!HasPickedUpBattery)
        {
            instructionArm.HideInfo();
            batteryFirstTimePickup.SetActive(true);
            HasPickedUpBattery = true;
        }
    }
    
}
