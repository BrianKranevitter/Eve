using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class KamSetAudioVolume : MonoBehaviour
{
    public AudioMixer MasterMixer;
    public AnimationCurve volumeCurve;
    public List<Volumes> volumes = new List<Volumes>();
    
    [System.Serializable]
    public struct Volumes
    {
        [Tooltip("Name of the property that changes the volume for its group.")]
        public string name;
        public Slider slider;
    }

    private void Awake()
    {
        foreach (var volume in volumes)
        {
            if (volume.slider != null)
                volume.slider.value = SettingsManager.FindFloatByName(volume.name);
        }
    }
    
    public void SetVolume(string volumeName)
    {
        Volumes volume = volumes.First(x => x.name == volumeName);

        if (volume.slider == null) return;

        float value = -80f + (80 * volumeCurve.Evaluate(volume.slider.value));
        MasterMixer.SetFloat(volume.name, value);

        SettingsManager.FindFloatByName(volume.name) = volume.slider.value;
    }
}
