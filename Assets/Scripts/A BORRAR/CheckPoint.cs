using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public GameObject[] door;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            for (int i = 0; i < door.Length; i++)
            {
            door[i].SetActive(false);

            }
    }
}
