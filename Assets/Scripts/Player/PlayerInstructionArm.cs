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
    public GameObject normalScreen;
    [HideInInspector]
    public bool active = false;
    
    [Header("Bestiary")]
    public KeyCode bestiaryKey;
    public KeyCode bestiaryForwardsKey;
    public KeyCode bestiaryBackwardsKey;
    public List<GameObject> bestiaryScreens = new List<GameObject>();
    private int currentBestiaryIndex;
    public bool bestiaryUnlocked;
    public bool bestiaryActive;

    Action UpdateControls = delegate {  };
    private void Start()
    {
        UpdateControls = InfoControls;
    }

    private void Update()
    {
        UpdateControls.Invoke();
    }

    private void InfoControls()
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
        
        if (active && bestiaryUnlocked && !bestiaryActive)
        {
            if (Input.GetKeyDown(bestiaryKey))
            {
                ShowBestiary();

                UpdateControls = BestiaryControls;
            }
        }
    }

    private void BestiaryControls()
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
        
        if (Input.GetKeyDown(bestiaryKey))
        {
            if (bestiaryActive)
            {
                HideBestiary();
            }
            else
            {
                ShowBestiary();
            }
        }
        
        if (bestiaryActive)
        {
            if (Input.GetKeyDown(bestiaryForwardsKey))
            {
                currentBestiaryIndex++;
                
                if (currentBestiaryIndex > bestiaryScreens.Count - 1)
                {
                    currentBestiaryIndex = 0;
                }
                
                SetBestiary(currentBestiaryIndex);
            }
            else if (Input.GetKeyDown(bestiaryBackwardsKey))
            {
                currentBestiaryIndex--;

                if (currentBestiaryIndex < 0)
                {
                    currentBestiaryIndex = bestiaryScreens.Count - 1;
                }
                
                SetBestiary(currentBestiaryIndex);
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

        if (bestiaryActive)
        {
            HideBestiary();
        }
    }

    public void HideInfo()
    {
        active = false;
        anim.SetTrigger("Out");
        
        if (bestiaryActive)
        {
            HideBestiary();
        }
    }

    public void UnlockBestiary()
    {
        bestiaryUnlocked = true;
    }

    public void ShowBestiary()
    {
        anim.SetTrigger("Bestiary");
        bestiaryActive = true;
    }

    public void FinishedShowBestiaryAnimation()
    {
        currentBestiaryIndex = 0;
        SetBestiary(currentBestiaryIndex);
        
        normalScreen.gameObject.SetActive(false);
    }

    public void HideBestiary()
    {
        anim.SetTrigger("Back");
        bestiaryActive = false;

        foreach (var screen in bestiaryScreens)
        {
            screen.gameObject.SetActive(false);
        }
        
        normalScreen.gameObject.SetActive(true);
    }

    public void SetBestiary(int index)
    {
        foreach (var screen in bestiaryScreens)
        {
            screen.gameObject.SetActive(false);
        }
        
        bestiaryScreens[index].SetActive(true);
    }

    public void OnHideInfoAnimationEnd()
    {
        anim.gameObject.SetActive(false);
        SetInfo(currentObjective);
    }
}
