using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public TextMeshProUGUI text;
    }
    
    public void SetVolume(string volumeName)
    {
        VolumeSetting setting = settings.First(x => x.name == volumeName);

        if (setting.slider == null) return;
        
        masterMixer.SetFloat(setting.name, -80f + (80 * volumeCurve.Evaluate(setting.slider.value)));

        if (setting.text != null)
        {
            setting.text.text = Mathf.RoundToInt(setting.slider.value * 100) + "%";
        }
    }

    public void SaveVolume()
    {
        foreach (var setting in settings)
        {
            PlayerPrefs.SetFloat(setting.name, setting.slider.value);
            Debug.Log($"Saving {setting.name} as {setting.slider.value}");
        }
    }
    public void LoadVolume()
    {
        foreach (var setting in settings)
        {
            if(PlayerPrefs.HasKey(setting.name))
            {
                setting.slider.value = PlayerPrefs.GetFloat(setting.name);
            }
            else
            {
                setting.slider.value = 1;
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
}
