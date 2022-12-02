using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FaceScanner : MonoBehaviour
{
    public float scanTime;

    public Animator anim;
    public string scanTrigger;
    public string interruptScanTrigger;
    public string accessDeniedTrigger;
    
    public UnityEvent onStartScan;
    public UnityEvent onStopScan;
    public UnityEvent onFinishScan;

    private bool scanning;
    private bool ableToStopScan = true;
    public void StartScanning()
    {
        if (scanning) return;

        ableToStopScan = true;
        scanning = true;
        onStartScan.Invoke();
        ResetTriggers();
        anim.SetTrigger(scanTrigger);
    }

    public void StopScan()
    {
        if (scanning && ableToStopScan)
        {
            scanning = false;
            onStopScan.Invoke();
            ResetTriggers();
            anim.SetTrigger(interruptScanTrigger);
        }
    }

    public void SetUnableToStopScan()
    {
        ableToStopScan = false;
    }
    
    public void FinishScan()
    {
        onFinishScan.Invoke();
        scanning = false;
    }

    public void InvalidScan()
    {
        if (scanning) return;

        ableToStopScan = true;
        scanning = true;
        onStartScan.Invoke();
        ResetTriggers();
        anim.SetTrigger(accessDeniedTrigger);
    }

    public void FinishInvalidScan()
    {
        scanning = false;
    }

    void ResetTriggers()
    {
        anim.ResetTrigger(scanTrigger);
        anim.ResetTrigger(interruptScanTrigger);
        anim.ResetTrigger(accessDeniedTrigger);
    }
}
