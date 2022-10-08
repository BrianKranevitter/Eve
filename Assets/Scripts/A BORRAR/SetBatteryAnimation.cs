using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBatteryAnimation : MonoBehaviour
{
    public Animator animator;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            animator.SetTrigger("LowBattery");
    }
}