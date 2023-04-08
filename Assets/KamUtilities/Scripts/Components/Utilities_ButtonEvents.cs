using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Utilities_ButtonEvents : MonoBehaviour
{
    public GameObject scalableObject;
    public Vector3 scaleOnHover;
    private Vector3 originalScale;
    
    public TextMeshProUGUI text;
    public Color textColorOnHover;
    private Color originalColor;
    
    
    public UnityEvent onHover;
    public UnityEvent onUnhover;
    public UnityEvent onClick;

    private bool Hovering;

    private void Start()
    {
        if(scalableObject != null)
            originalScale = scalableObject.transform.localScale;
        
        if(text != null)
            originalColor = text.color;
    }

    public void Hover()
    {
        if (Hovering) return;

        if(scalableObject != null)
            scalableObject.transform.localScale = scaleOnHover;
        
        if(text != null)
            text.color = textColorOnHover;
        
        onHover.Invoke();
        Hovering = true;
    }

    public void UnHover()
    {
        if (!Hovering) return;
        
        if(scalableObject != null)
            scalableObject.transform.localScale = originalScale;
        
        if(text != null)
            text.color = originalColor;
        
        onUnhover.Invoke();
        Hovering = false;
    }

    public void Click()
    {
        onClick.Invoke();
    }
}
