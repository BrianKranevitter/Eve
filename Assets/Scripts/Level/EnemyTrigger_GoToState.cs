using System;
using System.Collections;
using System.Collections.Generic;
using Game.Enemies;
using UnityEngine;

public class EnemyTrigger_GoToState : MonoBehaviour
{
    public LayerMask mask;
    public bool onlyOnce;
    public Enemy.EnemyState stateWhenCollidedWith;

    private void OnTriggerEnter(Collider other)
    {
        GoToStateCheck(other.gameObject, stateWhenCollidedWith);
    }

    private void OnTriggerStay(Collider other)
    {
        GoToStateCheck(other.gameObject, stateWhenCollidedWith);
    }

    void GoToStateCheck(GameObject other, Enemy.EnemyState stateWhenCollidedWith)
    {
        if (mask.CheckLayer(other.layer))
        {
            Enemy enemy = other.GetComponentInChildren<Enemy>() ?? other.GetComponentInParent<Enemy>();
            
            if (stateWhenCollidedWith == Enemy.EnemyState.Blinded_Player && enemy.currentState != Enemy.EnemyState.Blinded_Player)
            {
                enemy.SendInput(Enemy.EnemyState.Idle);
            }
            enemy.SendInput(stateWhenCollidedWith);

            if (onlyOnce)
            {
                Destroy(gameObject);
            }
        }
    }
}
