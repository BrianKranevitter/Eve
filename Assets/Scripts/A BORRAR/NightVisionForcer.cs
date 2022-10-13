using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class NightVisionForcer : MonoBehaviour
{
    public Color nightVisionColor;

    [SerializeField]
    private PostProcessVolume ppVolume;

    private NightVisionPPSSettings nightVisionShader;

    private void Awake()
    {
        ppVolume.profile.TryGetSettings(out nightVisionShader);
    }

    private void Update()
    {
        nightVisionShader._Color.value = nightVisionColor;
    }
}