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
    public string currentObjective;
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
        anim.gameObject.SetActive(false);
        anim.gameObject.SetActive(true);
        anim.SetTrigger("Go");
    }

    public void HideInfo()
    {
        active = false;
        anim.SetTrigger("Out");
    }

    public void OnHideInfoAnimationEnd()
    {
        anim.gameObject.SetActive(false);
        SetInfo(currentObjective);
    }
}
