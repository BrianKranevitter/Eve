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
        GoToStateCheck(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        GoToStateCheck(other.gameObject);
    }

    void GoToStateCheck(GameObject other)
    {
        if (mask.CheckLayer(other.layer))
        {
            Enemy enemy = other.GetComponentInChildren<Enemy>() ?? other.GetComponentInParent<Enemy>();
            
            enemy.SendInput(stateWhenCollidedWith);

            if (onlyOnce)
            {
                Destroy(gameObject);
            }
        }
    }
}
