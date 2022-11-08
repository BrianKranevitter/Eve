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
                active = false;
                HideInfo();
            }
            else
            {
                active = true;
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
        anim.SetTrigger("Go");
    }

    public void HideInfo()
    {
        anim.SetTrigger("Out");
    }
}
