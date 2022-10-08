using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggerStatic : MonoBehaviour
{
    Animator _anim;
    public float currentTime;
    public int index, timeToAttack, timeToWalk, timeToIdle, timeToCombat, timeToRun;
    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime > timeToAttack && index == 0)
        {
            index++;
            _anim.SetTrigger("GoToAttack");
        }
        else if (currentTime > timeToWalk && index == 1)
        {
            index++;
            _anim.SetTrigger("GoToWalk");
        }
        else if (currentTime > timeToIdle && index == 2)
        {
            index++;
            _anim.SetTrigger("GoToCalm");
        }
        else if (currentTime > timeToCombat && index == 3)
        {
            index++;
            _anim.SetTrigger("GoToCombat");
        }
        else if (currentTime > timeToRun && index == 4)
        {
            index = 0;
            _anim.SetTrigger("GoToRun");
            currentTime = 0;
        }
    }
}
