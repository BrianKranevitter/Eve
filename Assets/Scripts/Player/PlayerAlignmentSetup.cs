using System.Collections;
using System.Collections.Generic;
using Enderlook.Unity.Toolset.Attributes;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAlignmentSetup : MonoBehaviour
{
    public Transform targetFeet;
    public Transform targetBody;
    public Transform targetHead;
    
    public Transform objToLookAtWhileAligning;
    public UnityEvent onFinished;
}
