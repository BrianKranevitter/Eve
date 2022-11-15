using Enderlook.Unity.AudioManager;

using System;

using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Utility
{
    public sealed class SoundPlayer : MonoBehaviour
    {
        public void PlayAudioOneShootBag(AudioFile audio)
        {
            AudioUnit audioFile;
            if (audio is AudioBag)
            {
                AudioBag audioBag = (AudioBag)audio;
                audioFile = audioBag.files[Random.Range(0, audioBag.files.Length)];
                KamAudioManager.instance.PlaySFX_AudioUnit(audioFile, transform.position);
            }
            else if(audio is AudioUnit)
            {
                audioFile = (AudioUnit)audio;
                KamAudioManager.instance.PlaySFX_AudioUnit(audioFile, transform.position);
            }
            
            
            //AudioController.PlayOneShoot(audioFile, transform.position);
        }

        /*[Obsolete]
        public void PlayAudioOneShoot(AudioBag audio) => AudioController.PlayOneShoot(audio, transform.position);

        [Obsolete]
        public void PlayAudioOneShootUnit(AudioUnit audio) => AudioController.PlayOneShoot(audio, transform.position);*/
    }
}