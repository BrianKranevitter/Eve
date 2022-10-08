using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlerStatic : MonoBehaviour
{
    Animator _anim;
    public Animator mouthAnimator;
    public float currentTime;
    public int index, timeToWalk, timeToRight, timeToIdle, timeToHurt, timeToEating, timeToEatingLow, timeToDeath
        , timeToAttack, timeToLeft, timeToBreath;
    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime > timeToWalk && index == 0)
        {
            index++;
            _anim.SetTrigger("GoToWalk");
        }
        else if (currentTime > timeToRight && index == 1)
        {
            index++;
            _anim.SetTrigger("GoToRight");
        }
        else if (currentTime > timeToIdle && index == 2)
        {
            index++;
            _anim.SetTrigger("GoToIdle");
        }
        else if (currentTime > timeToHurt && index == 3)
        {
            index++;
            currentTime = timeToEating;
        }
        else if (currentTime > timeToEating && index == 4)
        {
            index++;
            _anim.SetTrigger("GoToEating");
        }
        else if (currentTime > timeToEatingLow && index == 5)
        {
            index++;
            currentTime = timeToDeath;
        }
        else if (currentTime > timeToDeath && index == 6)
        {
            index++;
            _anim.SetTrigger("GoToDeath");
        }
        else if (currentTime > timeToAttack && index == 7)
        {
            index++;
            _anim.SetTrigger("GoToAttack");
            mouthAnimator.SetTrigger("Puke");
        }
        else if (currentTime > timeToLeft && index == 8)
        {
            index++;
            _anim.SetTrigger("GoToLeft");
        }
        else if (currentTime > timeToBreath && index == 9)
        {
            index = 0;
            _anim.SetTrigger("GoToBreath");
            currentTime = 0;
        }
    }
}
