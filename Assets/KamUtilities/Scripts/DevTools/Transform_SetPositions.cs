using System;
using System.Collections;
using System.Collections.Generic;
using Kam.Utils;
using UnityEngine;
using UnityEngine.Events;

public class Transform_SetPositions : MonoBehaviour
{
    [System.Serializable]
    public struct Position
    {
        public string name;
        public Transform position;
        public UnityEvent onMove;
    }

    public List<Position> positions = new List<Position>();
    public KeyCode movePositionKey;
    
    private int current = 0;
    private void Update()
    {
        if (Input.GetKeyDown(movePositionKey))
        {
            transform.position = positions[current].position.position;
            positions[current].onMove.Invoke();

            current++;
            if(current >= positions.Count)
            {
                current = 0;
            }
        }
    }
}
