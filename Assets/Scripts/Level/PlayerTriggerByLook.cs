using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTriggerByLook : MonoBehaviour
{
    public UnityEvent OnTrigger;
    
    public void Trigger()
    {
        OnTrigger.Invoke();
    }
}
