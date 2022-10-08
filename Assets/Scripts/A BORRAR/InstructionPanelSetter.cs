using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionPanelSetter : MonoBehaviour
{
    public Renderer panelShader;
    public GameObject center;
    
    // Update is called once per frame
    void Update()
    {
        panelShader.material.SetVector("_PanelPosition", center.transform.position);    
    }
}
