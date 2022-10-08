using Enderlook.Unity.AudioManager;

using Game.Player;
using Game.Utility;

using UnityEngine;

namespace Game.Level
{
    [DefaultExecutionOrder(1)]
    public sealed class BackgroundMusic : MonoBehaviour
    {
        [SerializeField, Tooltip("Initial background sound.")]
        private AudioFile backgroundSound;

        private AudioPlay backgroundSoundPlay;
        private AudioPlay otherPlay;

        private static BackgroundMusic instance;

        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogError($"{nameof(BackgroundMusic)} is a singlenton.");
                Destroy(this);
            }

            instance = this;
            Try.PlayLoop(PlayerBody.Player.transform, backgroundSound, out backgroundSoundPlay, "backgroundSound", "player");
        }

        private void Update()
        {
            if (!otherPlay.IsDefault && !otherPlay.IsPlaying && !backgroundSoundPlay.IsDefault && !backgroundSoundPlay.IsPlaying)
                backgroundSoundPlay.Play();
        }

        public static void PlayLoop(AudioFile audio, string name)
        {
            if (Try.PlayLoop(PlayerBody.Player.transform, audio, out instance.otherPlay, name, "player") && !instance.backgroundSoundPlay.IsDefault)
                instance.backgroundSoundPlay.Stop();
        }

        public static void PlayOneShoot(AudioFile audio, string name)
        {
            if (Try.PlayOneShoot(PlayerBody.Player.transform, audio, out instance.otherPlay, name, "player") && !instance.backgroundSoundPlay.IsDefault)
                instance.backgroundSoundPlay.Stop();
        }

        public static void StopOtherPlay()
        {
            if (!instance.otherPlay.IsDefault && instance.otherPlay.IsPlaying)
                instance.otherPlay.Stop();
            if (!instance.backgroundSoundPlay.IsDefault && !instance.backgroundSoundPlay.IsPlaying)
                instance.backgroundSoundPlay.Play();
        }
    }
}