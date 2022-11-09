using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInstructionArm : MonoBehaviour
{
    public Animator anim;
    public TextMeshPro text;

    public KeyCode infoKey;

    private bool active = false;
    private void Update()
    {
        if (Input.GetKeyDown(infoKey))
        {
            if (active)
            {
                HideInfo();
            }
            else
            {
                ShowInfo();
            }
        }
    }

    public void SetInfo(string info)
    {
        text.text = info;
    }

    public void ShowInfo()
    {
        active = true;
        anim.SetTrigger("Go");
    }

    public void HideInfo()
    {
        active = false;
        anim.SetTrigger("Out");
    }
}
