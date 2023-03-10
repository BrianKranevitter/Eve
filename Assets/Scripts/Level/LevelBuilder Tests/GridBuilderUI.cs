using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridBuilderUI : MonoBehaviour
{
    private CustomGridBuilder builder;
    public TextMeshProUGUI text;
    private CustomGridBuilderManager manager;
    public Image disabledOverlay;
    
    public void LoadItem(CustomGridBuilder builder, CustomGridBuilderManager manager)
    {
        this.builder = builder;
        this.manager = manager;
        text.text = builder.gameObject.name;
    }

    public CustomGridBuilder GetBuilder()
    {
        return builder;
    }

    public void SetDisabledOverlay(bool state)
    {
        disabledOverlay.gameObject.SetActive(state);
    }
    public void OnClick()
    {
        manager.SelectGrid(builder);
    }
}
