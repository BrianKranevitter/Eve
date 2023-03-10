#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FloorBuilder : EditorWindow
{
    private Vector3 pos;
    private PlaceableObjectSO startingfloorObj;
    
    [MenuItem("Custom/KamFloorBuilder")]
    public static void ShowWindow()
    {
        GetWindow(typeof(FloorBuilder));
    }

    private void OnGUI()
    {
        GUILayout.Label("Tis but a test", EditorStyles.boldLabel);
        startingfloorObj = EditorGUILayout.ObjectField("Test", startingfloorObj, typeof(PlaceableObjectSO), false) as PlaceableObjectSO;

        if (GUILayout.Button("Testing grid thingy"))
        {
            CustomGrid<int> grid = new CustomGrid<int>(6, 12, 10f, default(Vector3), (grid, x, y) => 0);
        }
    }
}
#endif
