using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ShaderFloatLerper : MonoBehaviour
{
    public new List<Renderer> renderer;
    public bool playOnAwake = true;
    private bool started;
    public float floatingPointTolerance = 0.01f;
    [Serializable]
    public class FloatParameter
    {
        public string name;
        public string propertyName;
        public float startValue;
        public float endValue;
        [FormerlySerializedAs("incrementBy")] public float speed;
        private float _current;
        private bool _positive;
        private bool _done;
        
        
        public float current { get => _current; set => _current = value; }
        public bool positive { get => _positive; set => _positive = value; }
        public bool done { get => _done; set => _done = value; }

        public UnityEvent onFinished;
    }

    public List<FloatParameter> parameters;

    public UnityEvent onStart;
    public UnityEvent onFinish;

    private void Start()
    {
        if (playOnAwake)
        {
            Debug.Log("Play on awake");
            StartEffect();
        }
    }

    public void StartEffect()
    {
        onStart.Invoke();
        started = true;
        foreach (var parameter in parameters)
        {
            parameter.current = parameter.startValue;
            parameter.positive = parameter.endValue >= parameter.startValue;
            parameter.done = false;
        }
    }
    private void FixedUpdate()
    {
        if (started)
        {
            foreach (var parameter in parameters)
            {
                if (!parameter.done)
                {
                    parameter.current = Mathf.Lerp(parameter.current,parameter.endValue,parameter.speed * Time.deltaTime);
                    
                    bool condition = parameter.positive
                        ? parameter.endValue - parameter.current <= floatingPointTolerance
                        : parameter.current - parameter.endValue <= floatingPointTolerance;

                    bool paramWasDone = false;
                    if (condition)
                    {
                        parameter.done = true;
                        parameter.current = parameter.endValue;
                        parameter.onFinished.Invoke();
                        paramWasDone = true;
                    }
                    
                    foreach (var r in renderer)
                    {
                        r.material.SetFloat(parameter.propertyName.StartsWith("_") ? parameter.propertyName : "_"+parameter.propertyName, parameter.current);
                    }

                    if (paramWasDone)
                    {
                        if (parameters.Count(x => x.done) == parameters.Count)
                        {
                            started = false;
                            onFinish.Invoke();
                        }
                    }
                    
                }
            }
        }
    }
}

