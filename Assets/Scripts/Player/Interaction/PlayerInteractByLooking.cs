using System.Collections;
using System.Collections.Generic;
using Kam.Utils;
using UnityEngine;

public class PlayerInteractByLooking : MonoBehaviour
{
    public float range;
    public float timeLookingBeforeInteract;
    public Camera cam;

    private PlayerTriggerByLook last;
    void Update()
    {
        if (PauseMenu.Paused) return;
            
        Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f));
        if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hitInfo, range))
        {
            if (hitInfo.transform.TryGetComponent(out PlayerTriggerByLook interactable))
            {
                if (last != interactable)
                {
                    last = interactable;
                    StopAllCoroutines();
                    StartCoroutine(KamUtilities.Delay(timeLookingBeforeInteract, delegate { interactable.Trigger(); }));
                }
            }
            else
            {
                StopAllCoroutines();
            }
        }
        else
        {
            StopAllCoroutines();
        }
    }
}
