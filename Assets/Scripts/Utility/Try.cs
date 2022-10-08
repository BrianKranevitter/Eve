using Enderlook.Unity.AudioManager;

using UnityEngine;

namespace Game.Utility
{
    public static class Try
    {
        public static bool SetAnimationTrigger(Animator animator, string triggerName, string metaTriggerName, string animatorName = "")
        {
            if (animator == null)
            {
                if (string.IsNullOrEmpty(animatorName))
                    Debug.LogWarning($"Missing animator.");
                else
                    Debug.LogWarning($"Missing {animatorName} animator.");
                return false;
            }
            if (string.IsNullOrEmpty(triggerName))
            {
                Debug.LogWarning($"Missing {metaTriggerName} animation trigger.");
                return false;
            }
            else
            {
                animator.SetTrigger(triggerName);
                return true;
            }
        }

        public static bool PlayOneShoot(Transform transform, AudioFile file, string fileName)
        {
            if (file == null)
            {
                Debug.LogWarning($"Missing {fileName} sound.");
                return false;
            }

            AudioController.PlayOneShoot(file, transform);
            return true;
        }

        public static bool PlayOneShoot(Transform transform, AudioFile file, out AudioPlay audioPlay, string fileName, string transformName)
        {
            if (file == null)
            {
                Debug.LogWarning($"Missing {fileName} sound.");
                audioPlay = default;
                return false;
            }

            if (transform == null)
            {
                Debug.LogWarning($"Missing {transformName} transform.");
                audioPlay = default;
                return false;
            }

            audioPlay = AudioController.PlayOneShoot(file, transform);
            return true;
        }

        public static bool PlayLoop(Transform transform, AudioFile file, out AudioPlay audioPlay, string fileName, string transformName)
        {
            if (file == null)
            {
                Debug.LogWarning($"Missing {fileName} sound.");
                audioPlay = default;
                return false;
            }

            if (transform == null)
            {
                Debug.LogWarning($"Missing {transformName} transform.");
                audioPlay = default;
                return false;
            }

            audioPlay = AudioController.PlayLoop(file, transform);
            return true;
        }

        public static bool PlayOneShoot(Vector3 position, AudioFile file, string fileName)
        {
            if (file == null)
            {
                Debug.LogWarning($"Missing {fileName} sound.");
                return false;
            }
            else
            {
                AudioController.PlayOneShoot(file, position);
                return true;
            }
        }
    }
}