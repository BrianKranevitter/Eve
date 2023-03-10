using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AlternateBooleansInFunction : MonoBehaviour
{
    [System.Serializable]
    public class AlternatingFunction
    {
        public string customName;
        public UnityEvent<bool> function;
        public bool state;

        public void CallFunctionWithAlternatingBoolean()
        {
            function.Invoke(state);
            state = !state;
        }
    }

    public List<AlternatingFunction> _List = new List<AlternatingFunction>();

    public void CallFunction(string name)
    {
        _List.Find(x => x.customName == name).CallFunctionWithAlternatingBoolean();
    }
}
