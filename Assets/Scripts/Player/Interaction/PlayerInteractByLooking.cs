using System;
using System.Collections;
using System.Collections.Generic;
using Kam.Utils;
using UnityEngine;

public class PlayerInteractByLooking : MonoBehaviour
{
    public float range;
    public float timeLookingBeforeInteract;
    public Camera cam;
    public LayerMask mask;

    private PlayerTriggerByLook last;

    private bool drawgizmos = false;
    private void Awake()
    {
        drawgizmos = true;
    }

    void Update()
    {
        if (PauseMenu.Paused) return;
            
        Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f));
        if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hitInfo, range, mask))
        {
            if (hitInfo.transform.TryGetComponent(out PlayerTriggerByLook interactable))
            {
                Debug.Log("Got component");
                if (last != interactable)
                {
                    last = interactable;
                    StopAllCoroutines();
                    StartCoroutine(KamUtilities.Delay(timeLookingBeforeInteract, delegate { interactable.Trigger(); }));
                }
            }
            else
            {
                Debug.Log("No component");
                last = null;
                StopAllCoroutines();
            }
        }
        else
        {
            last = null;
            StopAllCoroutines();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawgizmos) return;
        
        Gizmos.color = Color.blue;
        Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f));
        Physics.Raycast(cam.transform.position, ray.direction, out RaycastHit hitInfo, range, mask);
        Vector3 pointA = cam.transform.position;
        Vector3 pointB = hitInfo.point == Vector3.zero ? ray.GetPoint(range) : hitInfo.point;
        Gizmos.DrawLine(pointA, pointB);
        
        Gizmos.color = Color.cyan;;
        Gizmos.DrawSphere(pointA, 0.1f);
        
        Gizmos.color = Color.red;;
        Gizmos.DrawSphere(pointB, 0.1f);
    }
}
