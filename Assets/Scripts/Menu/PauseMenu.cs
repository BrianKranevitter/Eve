using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool Paused;
    
    public GameObject menuRoot;

    public void TogglePauseMenu()
    {
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
        menuRoot.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void HidePauseMenu()
    {
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
}
