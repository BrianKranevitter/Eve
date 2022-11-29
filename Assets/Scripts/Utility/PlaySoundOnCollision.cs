using System;
using System.Collections;
using System.Collections.Generic;
using Enderlook.Unity.AudioManager;
using UnityEngine;

public class PlaySoundOnCollision : MonoBehaviour
{
    public AudioUnit file;
    public float minimumVelocity;
    
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.relativeVelocity.magnitude > minimumVelocity)
            KamAudioManager.instance.PlaySFX_AudioUnit(file, transform.position);
    }
}
