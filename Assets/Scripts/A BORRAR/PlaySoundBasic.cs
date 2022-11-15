using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlaySoundBasic : MonoBehaviour
{
    [SerializeField]
    ClipArray[] clipsArrays;

    [SerializeField]
    SoundObject prefab;

    [SerializeField] private AudioMixerGroup group;

    public void PlayAudio(int index)
    {
        SoundObject so = Instantiate(prefab);

        so.transform.position = transform.position;
        so.SetAudio(clipsArrays[index].clips[Random.Range(0, clipsArrays[index].clips.Length)],
            clipsArrays[index].volume, clipsArrays[index].minDistance, clipsArrays[index].maxDistance, group);
    }
}