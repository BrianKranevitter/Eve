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


    public List<LutSliders> lutSliders = new List<LutSliders>();
    [Serializable]
    public struct LutSliders
    {
        public string name;
        public Slider slider;
        public Utilities_DividedSlider dividedSlider;
    }
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
    
    
    
    
    
    public void SetSaturation(Utilities_DividedSlider slider)
    {
        float value = slider.CurrentValue;
        SettingsManager.Saturation = value;
        lutManager.Saturation = value;
    }
    public void SetBrightness(Utilities_DividedSlider slider)
    {
        float value = slider.CurrentValue;
        SettingsManager.Brightness = value;
        lutManager.Brightness = value;
    }
    public void SetContrast(Utilities_DividedSlider slider)
    {
        float value = slider.CurrentValue;
        SettingsManager.Contrast = value;
        lutManager.Contrast = value;
    }
    public void SetSharpness(Utilities_DividedSlider slider)
    {
        float value = slider.CurrentValue;
        SettingsManager.Sharpness = value;
        lutManager.Sharpness = value;
    }

    public void SetBackToDefault()
    {
        foreach (var lut in lutSliders)
        {
            float value = SettingsManager.FindFloatByName_Default(lut.name);
            if (lut.slider != null)
            {
                lut.slider.value = value;
            }
            
            if (lut.dividedSlider != null)
            {
                lut.dividedSlider.CurrentValue = value;
            }
        }
    }
}
