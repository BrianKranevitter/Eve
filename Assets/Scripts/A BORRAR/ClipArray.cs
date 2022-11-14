using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables")]
public class ClipArray : ScriptableObject
{
    public AudioClip[] clips;

    public float volume, minDistance, maxDistance;
}