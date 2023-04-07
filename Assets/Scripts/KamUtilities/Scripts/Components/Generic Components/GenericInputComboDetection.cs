using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericInputComboDetection : MonoBehaviour
{
    public float timeSinceFirstInput;
    public KeyCode[] inputCombo;

    private float currentTime;

    private Coroutine detectionCoroutine = null;

    public bool ableToDetect = true;
    bool detecting = false;
    public UnityEvent onInputCombo;
    private void Update()
    {
        if (ableToDetect && !detecting)
        {
            if (Input.GetKeyDown(inputCombo[0]))
            {
                currentTime = timeSinceFirstInput;

                if (detectionCoroutine != null)
                {
                    StopCoroutine(detectionCoroutine);
                }
                
                detectionCoroutine = StartCoroutine(InputDetectionCoroutine());
            }
        }
    }

    IEnumerator InputDetectionCoroutine()
    {
        detecting = true;
        bool success = true;
        
        for (int i = 1; (i < inputCombo.Length && detecting); i++)
        {
            yield return null;
            
            while (!Input.GetKeyDown(inputCombo[i]))
            {
                currentTime -= Time.deltaTime;

                if (currentTime <= 0 || (Input.anyKeyDown && !Input.GetKeyDown(inputCombo[i])))
                {
                    detecting = false;
                    success = false;
                    break;
                }

                yield return null;
            }
        }

        if (success)
        {
            onInputCombo.Invoke();
        }
    }

    public void SetAbleToDetect(bool state)
    {
        ableToDetect = state;
    }
}
