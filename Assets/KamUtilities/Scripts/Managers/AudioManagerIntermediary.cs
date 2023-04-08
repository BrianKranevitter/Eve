using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerIntermediary : MonoBehaviour
{
    public void PlaySFX(SFXs effect)
    {
        KamAudioManager.instance.PlaySFX(KamAssetDatabase.i.GetSFX(effect));
    }
    
    public void PlayMusic(Music song)
    {
        KamAudioManager.instance.PlayMusic(KamAssetDatabase.i.GetSong(song));
    }

    public void PauseCurrent()
    {
        KamAudioManager.activeSong.Pause();
    }

    public void UnpauseCurrent()
    {
        KamAudioManager.activeSong.UnPause();
    }
}
