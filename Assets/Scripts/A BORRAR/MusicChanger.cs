using Enderlook.Unity.AudioManager;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.Level.Triggers
{
    public class MusicChanger : MonoBehaviour
    {
        public void ChangeMusic()
        {
            BackgroundMusic.StopOtherPlay();
        }

        public void PlayAudio(AudioUnit audio)
        {
            PlayAudio(audio);
        }
    }
}
