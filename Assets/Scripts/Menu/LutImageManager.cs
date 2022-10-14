using System;
using System.Collections;
using System.Collections.Generic;
using DigitalRuby.SimpleLUT;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LutImageManager : MonoBehaviour
{
    public SimpleLUT lutManager;
    private void Start()
    {
        lutManager.Saturation = SettingsManager.Saturation;
        lutManager.Brightness = SettingsManager.Brightness;
        lutManager.Contrast = SettingsManager.Contrast;
        lutManager.Sharpness = SettingsManager.Sharpness;
    }

    public void SetSaturation(Slider slider)
    {
        float value = slider.value;
        SettingsManager.Saturation = value;
        lutManager.Saturation = value;
    }
    public void SetBrightness(Slider slider)
    {
        float value = slider.value;
        SettingsManager.Brightness = value;
        lutManager.Brightness = value;
    }
    public void SetContrast(Slider slider)
    {
        float value = slider.value;
        SettingsManager.Contrast = value;
        lutManager.Contrast = value;
    }
    public void SetSharpness(Slider slider)
    {
        float value = slider.value;
        SettingsManager.Sharpness = value;
        lutManager.Sharpness = value;
    }
}
