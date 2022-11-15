using Enderlook.Unity.AudioManager;
using Enderlook.Unity.Utils;

using UnityEngine;

namespace Game.Player.Weapons
{
    public sealed class BulletShell : MonoBehaviour
    {
        [SerializeField, Tooltip("Sound to play when colliding with floor.")]
        private AudioUnit onHitGroundSound;

        [SerializeField, Tooltip("Layer of ground.")]
        private LayerMask groundLayer;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.IsContainedIn(groundLayer))
            {
                KamAudioManager.instance.PlaySFX_AudioUnit(onHitGroundSound, transform.position);
                //AudioController.PlayOneShoot(onHitGroundSound, transform.position);
                Destroy(this);
            }
        }
    }
}
