using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BracerHandler : MonoBehaviour
{
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            anim.SetTrigger("KeyPressed");
        if (Input.GetKeyDown(KeyCode.Alpha2))
            anim.SetTrigger("GoPlay");
        if (Input.GetKeyDown(KeyCode.Alpha3))
            anim.SetTrigger("Back");
        if (Input.GetKeyDown(KeyCode.Alpha4))
            anim.SetTrigger("GoSettings");
        if (Input.GetKeyDown(KeyCode.Alpha5))
            anim.SetTrigger("GoCredits");
        if (Input.GetKeyDown(KeyCode.Alpha6))
            anim.SetTrigger("GoExit");
    }
}
