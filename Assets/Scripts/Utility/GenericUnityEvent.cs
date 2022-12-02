using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class GenericUnityEvent : MonoBehaviour
{
      [Tooltip("This name literally does nothing, it is just to identify or describe what the script is for in each case.")]
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
      public string name;
      [FormerlySerializedAs("OnSkip")] public UnityEvent OnEventTrigger;
}
