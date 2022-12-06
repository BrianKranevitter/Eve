using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool Paused;
    
    public GameObject menuRoot;
    
    [Tooltip("If input can show pause menu or not")]
    public bool controllable = true;

    private void Awake()
    {
        Paused = false;
    }

    public void TogglePauseMenu()
    {
        if (!controllable) return;
        if (menuRoot.activeSelf)
        {
            HidePauseMenu();
            UnPause();
        }
        else
        {
            ShowPauseMenu();
            Pause();
        }
    }
    
    public void ShowPauseMenu()
    {
        if (!controllable) return;
        menuRoot.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void HidePauseMenu()
    {
        if (!controllable) return;
        menuRoot.SetActive(false);
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    public void Pause()
    {
        Paused = true;
    }

    public void UnPause()
    {
        Paused = false;
    }

    public void SetControllable(bool state)
    {
        controllable = state;
    }
}
