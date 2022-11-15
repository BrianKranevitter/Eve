using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundObject : MonoBehaviour
{
    AudioSource _as;

    [SerializeField]
    float timeChecker;

    float currentTime;

    void Awake()
    {
        _as = GetComponent<AudioSource>();
    }


    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime >= timeChecker)
        {
            if (!_as.isPlaying)
                Destroy(gameObject);
        }
    }

    public void SetAudio(AudioClip clip, float volume, float minRange, float maxRange, AudioMixerGroup group)
    {
        _as.clip = clip;
        _as.volume = volume;
        _as.minDistance = minRange;
        _as.maxDistance = maxRange;
        _as.outputAudioMixerGroup = group;

        _as.Play();
    }
}