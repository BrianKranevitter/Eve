using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAlignment : MonoBehaviour
{
    public GameObject player;
    public GameObject playerHead;
    public GameObject playerBody;
    
    public GameObject head;
    public GameObject body;
    public float speed;
    public float lerpThreshhold = 0.001f;
    
    private GameObject targetHead;
    private GameObject targetBody;
    private GenericUnityEvent onFinish;

    public void SetTargetBody(GameObject body)
    {
        targetBody = body;
    }

    public void SetTargetCamera(GameObject camera)
    {
        targetHead = camera;
    }
    
    public void SetOnFinish(GenericUnityEvent script)
    {
        onFinish = script;
    }
    
    public void Align()
    {
        Align(targetBody.transform, targetHead.transform, speed, onFinish.OnEventTrigger.Invoke);
    }

    private Coroutine aligning = null;
    public void Align(Transform targetCamera, Transform targetBody, float alignmentSpeed, Action callback = null)
    {
        if (aligning != null)
        {
            StopCoroutine(aligning);
        }

        aligning = StartCoroutine(AlignCoroutine(head.transform, body.transform, targetCamera, targetBody, alignmentSpeed,
            delegate { callback?.Invoke(); aligning = null; }));
    }
        
    private IEnumerator AlignCoroutine(Transform currentCamera, Transform currentBody, Transform targetCamera, Transform targetBody, float alignmentSpeed, Action callback = null)
    {
        player.SetActive(false);
        head.SetActive(true);
        body.SetActive(true);
        
        InstaAlign(playerBody.transform, playerHead.transform);
        
        while (currentCamera.eulerAngles != targetCamera.eulerAngles || currentCamera.position != targetCamera.position || 
               currentBody.eulerAngles != targetBody.eulerAngles  || currentBody.position != targetBody.position)
        {
            Debug.Log("test");
            currentCamera.eulerAngles = Vector3.Lerp(currentCamera.eulerAngles, targetCamera.eulerAngles, alignmentSpeed * Time.deltaTime);
            if (Vector3.Distance(currentCamera.eulerAngles, targetCamera.eulerAngles) < lerpThreshhold)
            {
                currentCamera.eulerAngles = targetCamera.eulerAngles;
            }
            
            currentCamera.position = Vector3.Lerp(currentCamera.position, targetCamera.position, alignmentSpeed * Time.deltaTime);
            if (Vector3.Distance(currentCamera.position, targetCamera.position) < lerpThreshhold)
            {
                currentCamera.position = targetCamera.position;
            }
            
            currentBody.eulerAngles = Vector3.Lerp(currentBody.eulerAngles, targetBody.eulerAngles, alignmentSpeed * Time.deltaTime);
            if (Vector3.Distance(currentBody.eulerAngles, targetBody.eulerAngles) < lerpThreshhold)
            {
                currentBody.eulerAngles = targetBody.eulerAngles;
            }
            
            currentBody.position = Vector3.Lerp(currentBody.position, targetBody.position, alignmentSpeed * Time.deltaTime);
            if (Vector3.Distance(currentBody.position, targetBody.position) < lerpThreshhold)
            {
                currentBody.position = targetBody.position;
            }

            yield return null;
        }
        
        head.SetActive(false);
        body.SetActive(false);
        callback?.Invoke();
    }

    
    public void InstaAlign(Transform targetBody, Transform targetCamera)
    {
        InstaAlignBody(targetBody);
        InstaAlignCamera(targetCamera);
    }
    
    public void InstaAlignBody(Transform target)
    {
        body.transform.position = target.position;
        body.transform.rotation = target.rotation;
    }
    
    public void InstaAlignCamera(Transform target)
    {
        head.transform.position = target.position;
        head.transform.rotation = target.rotation;
    }
}
