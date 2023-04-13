using System;
using System.Collections;
using System.Collections.Generic;
using Game.Player;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class PlayerAlignment : MonoBehaviour
{
    public PlayerController player;
    public GameObject playerHead;
    public GameObject playerBody;
    
    public GameObject head;
    public GameObject body;
    public Animator bodyAnimator;
    public float speed;
    public float lerpThreshhold = 0.001f;

    public NavMeshAgent NavAgent;
    
    private Coroutine aligning = null;

    public void Align(PlayerAlignmentSetup setup)
    {
        AlignNavmesh(setup.targetFeet, setup.objToLookAtWhileAligning, setup.targetHead, setup.onFinished);
    }

    private void AlignNavmesh(Transform targetFeet, Transform objToLookAtWhileAligning, Transform targetHead, UnityEvent onAligned)
    {
        player.gameObject.SetActive(false);
        head.SetActive(true);
        body.SetActive(true);
        
        
        transform.position = playerBody.transform.position;
        transform.rotation = playerBody.transform.rotation;
    
        head.transform.position = playerHead.transform.position;
        head.transform.rotation = playerHead.transform.rotation;
        
        

        NavAgent.enabled = true;
        NavAgent.speed = player.walkingSpeed;
        NavAgent.acceleration = player.walkingAcceleration;
            
            
        NavAgent.SetDestination(targetFeet.position);

        if (aligning != null)
        {
            StopCoroutine(aligning);
        }

        aligning = StartCoroutine(WaitingForAlignment(onAligned, objToLookAtWhileAligning, targetHead));
    }

    IEnumerator WaitingForAlignment(UnityEvent onAligned, Transform objToLookAtWhileAligning, Transform targetHead)
    {
        while (!(!NavAgent.pathPending && NavAgent.remainingDistance <= NavAgent.stoppingDistance && (!NavAgent.hasPath || NavAgent.velocity.sqrMagnitude == 0f)))
        {
            //Walking towards destination
            bodyAnimator.SetFloat("PlayerSpeed", NavAgent.velocity.magnitude);
                
            head.transform.LookAt(objToLookAtWhileAligning);
            yield return null;
        }


        while (Vector3.Distance(head.transform.eulerAngles, targetHead.eulerAngles) > lerpThreshhold)
        {
            head.transform.eulerAngles = Vector3.Lerp(head.transform.eulerAngles, targetHead.eulerAngles, speed * Time.deltaTime);
            yield return null;
        }
            
        head.transform.eulerAngles = targetHead.eulerAngles;

        //Arrived
        head.SetActive(false);
        body.SetActive(false);
        
        onAligned.Invoke();
        
        NavAgent.enabled = false;

        aligning = null;
    }
    
    public void InstaAlign(PlayerAlignmentSetup setup)
    {
        InstaAlignBody(setup.targetBody);
        InstaAlignCamera(setup.targetHead);
        
        setup.onFinished.Invoke();
    }
    
    public void InstaAlignBody(Transform target)
    {
        player.transform.position = target.position;
        player.transform.rotation = target.rotation;
    }
    
    public void InstaAlignCamera(Transform target)
    {
        player.head.transform.position = target.position;
        player.head.transform.rotation = target.rotation;
    }
}
