using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generic_PlayMusic : MonoBehaviour
{
    public Music song;
    private void Start()
    {
        KamAudioManager.instance.PlayMusic(KamAssetDatabase.i.GetSong(song));
    }
}