using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enderlook.Unity.Toolset.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    public AudioMixer masterMixer;

    public List<VolumeSetting> settings = new List<VolumeSetting>();

    public AnimationCurve volumeCurve;

    [System.Serializable]
    public struct VolumeSetting
    {
        public string name;
        public Slider slider;
        public Utilities_DividedSlider dividedSlider;
        
        public TextMeshProUGUI text;
    }
    
    public void SetVolume(string volumeName)
    {
        VolumeSetting setting = settings.First(x => x.name == volumeName);

        bool sliderNull = setting.slider == null;
        bool dividedSliderNull = setting.dividedSlider == null;
        if (sliderNull && dividedSliderNull) return;
        
        float value = sliderNull ? setting.dividedSlider.CurrentValue : setting.slider.value;
        
        masterMixer.SetFloat(setting.name, -80f + (80 * volumeCurve.Evaluate(value)));

        if (setting.text != null)
        {
            setting.text.text = Mathf.RoundToInt(value * 100) + "%";
        }
    }

    public void SaveVolume()
    {
        foreach (var setting in settings)
        {
            float value = setting.slider == null ? setting.dividedSlider.CurrentValue : setting.slider.value;
            PlayerPrefs.SetFloat(setting.name, value);
            //Debug.Log("Saving Volume: " + $"{setting.name} = {value}");
        }
    }
    public void LoadVolume()
    {
        foreach (var setting in settings)
        {
            if(PlayerPrefs.HasKey(setting.name))
            {
                float value = PlayerPrefs.GetFloat(setting.name);
                if (setting.dividedSlider != null)
                {
                    setting.dividedSlider.CurrentValue = value;
                }

                if (setting.slider != null)
                {
                    setting.slider.value = value;
                }
                
                //Debug.Log("Loading Volume: " + $"{setting.name} = {value}");
            }
            else
            {
                setting.slider.value = SettingsManager.FindFloatByName_Default(setting.name);
            }
            
            SetVolume(setting.name);
        }
    }
    
    private void OnApplicationQuit()
    {
        SaveVolume();
    }

    private void Awake()
    {
        LoadVolume();
        
        Debug.Log("LOAD VOLUME");
    }

    public void SetToDefaults()
    {
        foreach (var setting in settings)
        {
            float value = SettingsManager.FindFloatByName_Default(setting.name);
            if (setting.slider != null)
            {
                setting.slider.value = value;
            }
            
            if (setting.dividedSlider != null)
            {
                setting.dividedSlider.CurrentValue = value;
            }
        }
    }
}
