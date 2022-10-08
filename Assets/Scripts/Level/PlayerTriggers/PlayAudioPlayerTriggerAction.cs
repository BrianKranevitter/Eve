using Enderlook.Unity.AudioManager;

using Game.Utility;

using System;

using UnityEngine;

namespace Game.Level.Triggers
{
    [Serializable]
    public sealed class PlayAudioPlayerTriggerAction : PlayerTriggerAction
    {
        [SerializeField, Tooltip("Audios to play.")]
        private Pack[] audios;

        private AudioPlay[] stored;

        [Serializable]
        private struct Pack
        {
            [Tooltip("Audio file to play.")]
            public AudioFile audio;

            [Tooltip("Center position where sound will be played.")]
            public Transform center;

            [Tooltip("If it should loop if the player stays in the trigger.")]
            public bool loop;

            [Tooltip("If it should stop when the player get out of the trigger.")]
            public bool stopWhenGetOut;
        }

        public override void OnTriggerEnter()
        {
            if (stored is null)
                stored = new AudioPlay[audios.Length];

            for (int i = 0; i < audios.Length; i++)
            {
                Pack pack = audios[i];

                if (pack.loop)
                {
                    Try.PlayLoop(pack.center, pack.audio, out AudioPlay audioPlay, "audio", "center");
                    stored[i] = audioPlay;
                }
                else
                {
                    Try.PlayOneShoot(pack.center, pack.audio, out AudioPlay audioPlay, "audio", "center");
                    stored[i] = audioPlay;
                }
            }
        }

        public override void OnTriggerExit()
        {
            for (int i = 0; i < audios.Length; i++)
            {
                Pack pack = audios[i];

                if (pack.audio == null)
                    continue;

                if (pack.center == null)
                    continue;

                if (pack.stopWhenGetOut)
                    stored[i].Stop();

                stored[i] = default;
            }
        }
    }
}