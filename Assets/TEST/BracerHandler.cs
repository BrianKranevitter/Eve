using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class BracerHandler : MonoBehaviour
{
    public Camera cam;
    public LayerMask mask;
    public Phase phase;
    Action onUpdate = delegate { };
    Animator anim;

    public enum Phase
    {
        None, AnyKey, Detection
    }

    void Start()
    {
        anim = GetComponent<Animator>();
    }
    
    void Update()
    {
        onUpdate.Invoke();
    }

    public void SetPhase_Detection()
    {
        onUpdate = Detection;
        phase = Phase.Detection;
    }
    
    public void SetPhase_AnyKey()
    {
        onUpdate = AnyKey;
        phase = Phase.AnyKey;
    }
    void Test()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            anim.SetTrigger("KeyPressed");
        if (Input.GetKeyDown(KeyCode.Alpha2))
            anim.SetTrigger("GoPlay");
        if (Input.GetKeyDown(KeyCode.Alpha3))
            anim.SetTrigger("Back");
        if (Input.GetKeyDown(KeyCode.Alpha4))
            anim.SetTrigger("GoSettings");
        if (Input.GetKeyDown(KeyCode.Alpha5))
            anim.SetTrigger("GoCredits");
        if (Input.GetKeyDown(KeyCode.Alpha6))
            anim.SetTrigger("GoExit");
    }

    void AnyKey()
    {
        if (Input.anyKey)
        {
            SetTrigger("KeyPressed");
        }
    }

    private Collider collider;
    private Utilities_ButtonEvents script;
    private Ray ray;
    void Detection()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit info, 999, mask))
        {
            if (info.collider != collider)
            {
                collider = info.collider;

                if (script != null)
                {
                    script.UnHover();
                }

                script = info.collider.gameObject.GetComponent<Utilities_ButtonEvents>();

                if (script != null)
                {
                    script.Hover();
                }
            }
        }
        else
        {
            script?.UnHover();
            script = null;
            collider = null;
        }
        
        if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse))
        {
            script?.Click();
        }
    }

    public void SetTrigger(string trigger)
    {
        ResetTriggers();
        anim.SetTrigger(trigger);
    }
    
    public void ResetTriggers()
    {
        anim.ResetTrigger("KeyPressed");
        anim.ResetTrigger("GoPlay");
        anim.ResetTrigger("Back");
        anim.ResetTrigger("GoSettings");
        anim.ResetTrigger("GoCredits");
        anim.ResetTrigger("GoExit");
    }
    
    private void OnDrawGizmos()
    {
        Debug.DrawLine(ray.origin,ray.GetPoint(999), Color.red);
    }
}
