using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalClippingTest : MonoBehaviour
{
    public Renderer renderer;

    private void Update()
    {
        renderer.material.SetVector("_SliceCenter", transform.position);
        renderer.material.SetVector("_SliceNormal", transform.forward);
    }
}
