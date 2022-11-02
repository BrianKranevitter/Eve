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
    public Transform useDirectionOf;
    public GameObject glowstick;

    [Header("Throw Settings")]
    public KeyCode key;
    public float forwardForce;
    public float upwardsForce;
    
    [Header("Visuals/Effects")]
    public AudioFile throwSFX;
    public bool useAnimationToSpawn;
    public Animator animator;
    public string throwTrigger;
    
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
                if (!useAnimationToSpawn)
                {
                    Throw();
                }
                
                animator.SetTrigger(throwTrigger);
            }
        }
    }

    public void Throw()
    {
        Try.PlayOneShoot(transform, throwSFX,"throwGlowstickSFX");

        GameObject obj = Instantiate(glowstick, firePoint.position, firePoint.rotation);

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        Vector3 forwardForce = useDirectionOf.forward * this.forwardForce;
        Vector3 upwardsForce = useDirectionOf.up * this.upwardsForce;
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
