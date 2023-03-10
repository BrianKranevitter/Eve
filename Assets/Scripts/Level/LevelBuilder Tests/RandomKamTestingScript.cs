using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RandomKamTestingScript : MonoBehaviour
{
    [SerializeField] private RawImage image;
    public UnityEvent<bool> test;
    private void Start() {

        RefreshVisual();
    }

    private void Instance_OnSelectedChanged(PlaceableObjectSO selected) 
    {
        RefreshVisual();
    }

    private void RefreshVisual()
    {
        PlaceableObjectSO placedObjectTypeSO = CustomGridBuilderManager.Instance.GetSelectedObject();
        image.texture = AssetPreview.GetAssetPreview(placedObjectTypeSO.visual);
    }
}
