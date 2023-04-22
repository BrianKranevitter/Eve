using System;
using System.Collections;
using System.Collections.Generic;
using Enderlook.Unity.AudioManager;
using Enderlook.Unity.Toolset.Attributes;
using Enderlook.Unity.Toolset.Checking;
using UnityEngine;
using UnityEngine.Events;

public class CheatsManager : MonoBehaviour
{
    [Serializable]
    public class Cheat
    {
        public string name;
        public Type cheatType;
        [ShowIf(nameof(cheatType), typeof(Type),Type.Automatic, false)]
        public KeyCode key;
        public UnityEvent onTurnOn;
        public UnityEvent onTurnOff;

        [HideInInspector]
        public bool activated;

        public enum Type
        {
            Toggleable, Repeatable, Automatic
        }
    }
    
    public static bool cheatsActivated;

    public List<Cheat> extraCheats;
    
    public UnityEvent onTurnOn;
    public UnityEvent onTurnOff;
    
    private Action onUpdate = () => {};
    private void Update()
    {
        onUpdate.Invoke();
    }

    private void CheatsUpdate()
    {
        foreach (var cheat in extraCheats)
        {
            if (cheat.cheatType != Cheat.Type.Automatic)
            {
                if (Input.GetKeyDown(cheat.key))
                {
                    switch (cheat.cheatType)
                    {
                        case Cheat.Type.Toggleable:
                            if (cheat.activated)
                            {
                                cheat.onTurnOff.Invoke();
                                cheat.activated = false;
                            }
                            else
                            {
                                cheat.onTurnOn.Invoke();
                                cheat.activated = true;
                            }
                            break;
                    
                        case Cheat.Type.Repeatable:
                            cheat.onTurnOn.Invoke();
                            cheat.activated = true;
                            break;
                    }
                }
            }
        }
    }

    public void ToggleCheats()
    {
        if (cheatsActivated)
        {
            TurnOffCheats();
        }
        else
        {
            TurnOnCheats();
        }
    }
    
    public void TurnOnCheats()
    {
        onTurnOn.Invoke();
        cheatsActivated = true;
        onUpdate = CheatsUpdate;
        
        foreach (var cheat in extraCheats)
        {
            if (cheat.cheatType == Cheat.Type.Automatic)
            {
                cheat.onTurnOn.Invoke();
                cheat.activated = true;
            }
        }
    }
    
    public void TurnOffCheats()
    {
        onTurnOff.Invoke();
        cheatsActivated = false;
        onUpdate = delegate {  };

        foreach (var cheat in extraCheats)
        {
            cheat.onTurnOff.Invoke();
            cheat.activated = false;
        }
    }
}
