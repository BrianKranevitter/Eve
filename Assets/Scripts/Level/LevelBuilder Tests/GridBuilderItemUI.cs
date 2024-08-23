using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GridBuilderItemUI : MonoBehaviour
{
    public RawImage image;
    public TextMeshProUGUI text;
    private CustomGridBuilder builder;
    private PlaceableObjectSO so;
    
    public void LoadItem(PlaceableObjectSO so, CustomGridBuilder builder)
    {
        this.builder = builder;
        this.so = so;
       // image.texture = so.optionalVisual != null ? so.optionalVisual : AssetPreview.GetAssetPreview(so.visual);
        text.text = $"{so.width}x{so.height}";
    }

    public PlaceableObjectSO GetLoadedSO()
    {
        return so;
    }

    public void OnClick()
    {
        builder.SelectObject(so);
    }
}
