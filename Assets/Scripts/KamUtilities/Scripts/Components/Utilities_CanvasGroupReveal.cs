using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Utilities_CanvasGroupReveal : MonoBehaviour
{
    [SerializeField] private CanvasGroup obj;
    [SerializeField] public float revealTime;
    [SerializeField] private float decreasingAlphaValue;


    [SerializeField] private UnityEvent onRevealed;
    [SerializeField] private UnityEvent onHidden;
    
    [SerializeField] private UnityEvent onTempRevealed;
    [SerializeField] private UnityEvent onTempHidden;

    public bool revealed = false;

    private void Awake()
    {
        revealed = obj.alpha == 1;
    }

    public void RevealTemporary()
    {
        StopAllCoroutines();
        temporary = StartCoroutine(RevealTemporaryCoroutine());
    }

    public void Reveal()
    {
        if (hiding)
        {
            StopCoroutine(hide);
        }
        
        if (revealing)
        {
            StopCoroutine(reveal);
        }
        
        reveal = StartCoroutine(RevealCoroutine());
    }
    
    public void Hide()
    {
        if (revealing)
        {
            StopCoroutine(reveal);
        }
        
        if (hiding)
        {
            StopCoroutine(hide);
        }
        
        hide = StartCoroutine(HideCoroutine());
    }
    
    private bool showing;
    public void RevealToggle()
    {
        if (!showing)
        {
            Reveal();
            showing = true;
        }
        else
        {
            Hide();
            showing = false;
        }
    }
    
    private bool tempRunning { get => temporary != null;}
    private Coroutine temporary = null;
    private IEnumerator RevealTemporaryCoroutine()
    {
        while (obj.alpha < 1)
        {
            obj.alpha += decreasingAlphaValue;
            yield return new WaitForEndOfFrame();
        }
        
        onTempRevealed.Invoke();
        
        
        yield return new WaitForSeconds(revealTime);
        
        while (obj.alpha > 0)
        {
            obj.alpha -= decreasingAlphaValue;
            yield return new WaitForEndOfFrame();
        }
        
        onTempHidden.Invoke();
    }
    
    private bool revealing { get => reveal != null;}
    private Coroutine reveal = null;
    private IEnumerator RevealCoroutine()
    {
        while (obj.alpha < 1)
        {
            obj.alpha += decreasingAlphaValue;
            yield return new WaitForEndOfFrame();
        }
        
        onRevealed.Invoke();
        revealed = true;
    }

    private bool hiding { get => hide != null;}
    private Coroutine hide = null;
    private IEnumerator HideCoroutine()
    {
        while (obj.alpha > 0)
        {
            obj.alpha -= decreasingAlphaValue;
            yield return new WaitForEndOfFrame();
        }
        
        onHidden.Invoke();
        revealed = false;
    }
}
