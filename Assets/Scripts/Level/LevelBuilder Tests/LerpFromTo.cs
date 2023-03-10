using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LerpFromTo : MonoBehaviour
{
    public Vector3 posA;
    public Vector3 posB;
    public float speed;
    public float tolerance;

    public UnityEvent onLerpStart_Forwards;
    public UnityEvent onLerpEnd_Forwards;
    
    public UnityEvent onLerpStart_Backwards;
    public UnityEvent onLerpEnd_Backwards;
    
    private bool backwards = false;
    public void StartLerp()
    {
        onLerpStart_Forwards.Invoke();
        StopAllCoroutines();
        StartCoroutine(StartLerpCoroutine());
        onLerpEnd_Forwards.Invoke();
    }

    public void StartLerpBackwards()
    {
        onLerpStart_Backwards.Invoke();
        StopAllCoroutines();
        StartCoroutine(StartLerpCoroutine(true));
        onLerpEnd_Backwards.Invoke();
    }

    public void AlternateLerpDirection()
    {
        if (backwards)
        {
            StartLerpBackwards();
        }
        else
        {
            StartLerp();
        }

        backwards = !backwards;
    }
    
    IEnumerator StartLerpCoroutine(bool backwards = false)
    {
        Vector3 posA = backwards ? this.posB : this.posA;
        Vector3 posB = backwards ? this.posA : this.posB;
        
        transform.localPosition = posA;
        
        while (Vector3.Distance(transform.localPosition, posB) > tolerance)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, posB, speed * Time.deltaTime);
            yield return null;
        }
        
        transform.localPosition = posB;
    }
}
