using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class NightVisionForcer : MonoBehaviour
{
    public Color nightVisionColor;

    public float remapMin, remapMax, noiseStrenght, nightVisionLightRange, grayScaleValue, nightVisionBloomValue;

    public Light nightVisionLight;

    [SerializeField]
    private PostProcessVolume ppVolume;

    private NightVisionPPSSettings nightVisionShader;

    private Bloom bloomShader;

    private Animator _anim;

    private bool nightVisionTurnedOn;

    private void Awake()
    {
        ppVolume.profile.TryGetSettings(out nightVisionShader);
        ppVolume.profile.TryGetSettings(out bloomShader);

        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        nightVisionShader._Color.value = nightVisionColor;
        nightVisionShader._Remap_Min.value = remapMin;
        nightVisionShader._Remap_Max.value = remapMax;
        nightVisionShader._Noise_Strenght.value = noiseStrenght;
        nightVisionLight.range = nightVisionLightRange;
        nightVisionShader._GrayscaleValue.value = grayScaleValue;
        bloomShader.intensity.value = nightVisionBloomValue;

        if (Input.GetKeyDown(KeyCode.N))
        {
            if (!nightVisionTurnedOn)
            {
                _anim.SetTrigger("TurnOn");
                nightVisionTurnedOn = true;
            }
            else
            {
                _anim.SetTrigger("TurnOff");
                nightVisionTurnedOn = false;
            }
        }
    }
}