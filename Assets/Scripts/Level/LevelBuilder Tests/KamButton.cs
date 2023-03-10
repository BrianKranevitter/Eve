using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KamButton : Button
{
    public UnityEvent onSelect;
    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        
        onSelect.Invoke();
    }
}
