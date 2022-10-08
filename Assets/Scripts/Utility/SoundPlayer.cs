using Enderlook.Unity.AudioManager;

using System;

using UnityEngine;

namespace Game.Utility
{
    public sealed class SoundPlayer : MonoBehaviour
    {
        public void PlayAudioOneShootBag(AudioFile audio) => AudioController.PlayOneShoot(audio, transform.position);

        /*[Obsolete]
        public void PlayAudioOneShoot(AudioBag audio) => AudioController.PlayOneShoot(audio, transform.position);

        [Obsolete]
        public void PlayAudioOneShootUnit(AudioUnit audio) => AudioController.PlayOneShoot(audio, transform.position);*/
    }
}