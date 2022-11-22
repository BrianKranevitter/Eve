using Enderlook.Unity.AudioManager;

using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Utility
{
    public sealed class SoundPlayer : MonoBehaviour
    {
        private bool overrideMaxDistance = false;
        private float newMaxDistance = 50;
        public void PlayAudioOneShootBag(AudioFile audio)
        {
            AudioUnit audioFile;
            AudioSource source;
            if (audio is AudioBag)
            {
                AudioBag audioBag = (AudioBag)audio;
                audioFile = audioBag.files[Random.Range(0, audioBag.files.Length)];
                
                source = KamAudioManager.instance.PlaySFX_AudioUnit(audioFile, transform.position);
                source.maxDistance = overrideMaxDistance ? newMaxDistance : source.maxDistance;
            }
            else if(audio is AudioUnit)
            {
                audioFile = (AudioUnit)audio;
                source = KamAudioManager.instance.PlaySFX_AudioUnit(audioFile, transform.position);
                source.maxDistance = overrideMaxDistance ? newMaxDistance : source.maxDistance;
            }
            
            
            //AudioController.PlayOneShoot(audioFile, transform.position);
        }

        public void OverrideMaxDistance(float newMaxDistance)
        {
            overrideMaxDistance = true;
            this.newMaxDistance = newMaxDistance;
        }

        /*[Obsolete]
        public void PlayAudioOneShoot(AudioBag audio) => AudioController.PlayOneShoot(audio, transform.position);

        [Obsolete]
        public void PlayAudioOneShootUnit(AudioUnit audio) => AudioController.PlayOneShoot(audio, transform.position);*/
    }
}