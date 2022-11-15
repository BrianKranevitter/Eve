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

        public static bool PlayOneShoot(Transform transform, AudioUnit file, string fileName)
        {
            if (file == null)
            {
                Debug.LogWarning($"Missing {fileName} sound.");
                return false;
            }

            KamAudioManager.instance.PlaySFX_AudioUnit(file,transform.position);
            //AudioController.PlayOneShoot(file, transform);
            return true;
        }

        public static bool PlayOneShoot(Transform transform, AudioUnit file, out AudioPlay audioPlay, string fileName, string transformName)
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

            
            KamAudioManager.instance.PlaySFX_AudioUnit(file, transform.position);
            audioPlay = default;
            //audioPlay = AudioController.PlayOneShoot(file, transform);
            return true;
        }

        public static bool PlayLoop(Transform transform, AudioUnit file, out AudioPlay audioPlay, string fileName, string transformName)
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

            KamAudioManager.instance.PlaySFX_AudioUnit(file, transform.position, true);
            //audioPlay = AudioController.PlayLoop(file, transform);
            audioPlay = default;
            return true;
        }

        public static bool PlayOneShoot(Vector3 position, AudioUnit file, string fileName)
        {
            if (file == null)
            {
                Debug.LogWarning($"Missing {fileName} sound.");
                return false;
            }
            else
            {
                KamAudioManager.instance.PlaySFX_AudioUnit(file, position);
                //AudioController.PlayOneShoot(file, position);
                return true;
            }
        }
    }
}