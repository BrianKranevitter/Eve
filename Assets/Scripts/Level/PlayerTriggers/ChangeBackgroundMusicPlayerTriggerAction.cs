using Enderlook.Unity.AudioManager;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Level.Triggers
{
    public sealed class ChangeBackgroundMusicPlayerTriggerAction : PlayerTriggerAction
    {
        [SerializeField, Tooltip("Audio to play in background.")]
        private AudioFile audio;

        [SerializeField, Tooltip("If true, the audio will loop.")]
        private bool loop;

        [SerializeField, Tooltip("If true, the background music will change to default when player get out of trigger.")]
        private bool stopWhenGetOut;

        public override void OnTriggerEnter()
        {
            if (loop)
                BackgroundMusic.PlayLoop(audio, "audio");
            else
                BackgroundMusic.PlayOneShoot(audio, "audio");
        }

        public override void OnTriggerExit()
        {
            if (stopWhenGetOut)
                BackgroundMusic.StopOtherPlay();
        }
    }
}