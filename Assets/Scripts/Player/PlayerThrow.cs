using System;
using System.Collections;
using System.Collections.Generic;
using Enderlook.Unity.AudioManager;
using Game.Utility;
using UnityEngine;

public class PlayerThrow : MonoBehaviour
{
    [Header("Cooldown")]
    public float cooldown = 3f;
    private float _currentCooldown;
    
    [Header("Assignables")]
    public Transform firePoint;
    public GameObject glowstick;

    [Header("Throw Settings")]
    public KeyCode key;
    public AudioFile throwSFX;
    public float forwardForce;
    public float upwardsForce;
    
    private bool readyToThrow;

    public Action<GameObject> onThrow = delegate(GameObject o) {  };
    
    private void Start()
    {
        readyToThrow = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(key))
        {
            if (!readyToThrow)
            {
                //Still on cd
            }
            else
            {
                Throw();
            }
        }
    }

    public void Throw()
    {
        Try.PlayOneShoot(transform, throwSFX,"throwGlowstickSFX");

        GameObject obj = Instantiate(glowstick, firePoint.position, firePoint.rotation);

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        Vector3 forwardForce = firePoint.forward * this.forwardForce;
        Vector3 upwardsForce = firePoint.up * this.upwardsForce;
        rb.AddForceAtPosition(forwardForce + upwardsForce, obj.transform.position + (Vector3.down * 0.4f), ForceMode.Impulse);
        
        readyToThrow = false;
        Invoke(nameof(ResetThrow), cooldown);
        
        onThrow.Invoke(obj);
    }
    
    void ResetThrow()
    {
        readyToThrow = true;
    }

}
