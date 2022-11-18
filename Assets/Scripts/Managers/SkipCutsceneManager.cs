using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SkipCutsceneManager : MonoBehaviour
{
    public static SkipCutsceneManager i;
    [FormerlySerializedAs("reveal")] public Utilities_CanvasGroupReveal SkipReveal;
    public Utilities_CanvasGroupReveal textReveal;
    public KeyCode skipKey;
    
    [HideInInspector]
    public SkippableCutscene currentCutscene;

    private Action onUpdate = delegate {  };
    private void Awake()
    {
        i = this; 
    }

    private void Update()
    {
        onUpdate.Invoke();
    }

    private void Controls_Start()
    {
        if (Input.anyKey)
        {
            textReveal.Reveal();
            onUpdate =  Controls_WaitingForInput;
        }
    }
    private void Controls_WaitingForInput()
    {
        if (Input.GetKeyDown(skipKey))
        {
            Skip();
        }
    }

    public void SkipAction()
    {
        currentCutscene.OnSkip.Invoke();

        UnloadSkippable();
    }

    public void Skip()
    {
        if (currentCutscene == null) return;
        
        SkipReveal.RevealTemporary();
    }
    
    public void LoadSkippable(SkippableCutscene cutscene)
    {
        currentCutscene = cutscene;

        onUpdate = Controls_Start;
    }
    
    public void UnloadSkippable()
    {
        textReveal.Hide();
        currentCutscene = null;
        onUpdate = delegate{};
    }
}
