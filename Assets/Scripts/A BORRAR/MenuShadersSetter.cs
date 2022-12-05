using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class MenuShadersSetter : MonoBehaviour
{
    DepthOfField depthShader;
    ColorGrading colorShader;

    [SerializeField]
    private PostProcessVolume ppVolume;

    public float blurValue;
    public Color colorFilter;

    void Start()
    {
        ppVolume.profile.TryGetSettings(out depthShader);
        ppVolume.profile.TryGetSettings(out colorShader);
    }

    void Update()
    {
        depthShader.focalLength.value = blurValue;
        colorShader.colorFilter.value = colorFilter;
    }
}