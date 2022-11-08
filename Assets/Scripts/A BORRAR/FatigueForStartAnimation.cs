using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class FatigueForStartAnimation : MonoBehaviour
{
    public float globalIntensity, blurIntensity,blackScreen;

    [SerializeField]
    private PostProcessVolume ppVolume;

    private FatiguePPSSettings fatigueShader;

    private void Awake()
    {
        ppVolume.profile.TryGetSettings(out fatigueShader);
    }

    private void Update()
    {
        fatigueShader._GlobalIntensity.value = globalIntensity;
        fatigueShader._ExplosionBlurIntensity.value = blurIntensity;
        fatigueShader._BlackScreen_Value.value = blackScreen;
    }

    public void TurnOffForcer()
    {
        this.enabled = false;
    }
}