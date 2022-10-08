using Game.Player;

using UnityEngine;

namespace Game.Level.Triggers
{
    public sealed class PlayerTrigger : MonoBehaviour
    {
        [SerializeField, Tooltip("If true, actions will only be triggered once. After that, the gameobject will be destroyed to save memory.")]
        private bool triggerOnce;

        [SerializeReference]
        private PlayerTriggerAction[] actions;

        private bool isIn;

        private void OnTriggerEnter(Collider other)
        {
            if (isIn)
                return;

            isIn = true;

            if (other.transform.GetComponentInParent<PlayerBody>() == null)
                return;

            foreach (PlayerTriggerAction action in actions)
                action.OnTriggerEnter();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!isIn)
                return;

            isIn = false;

            if (other.transform.GetComponentInParent<PlayerBody>() == null)
                return;

            foreach (PlayerTriggerAction action in actions)
                action.OnTriggerExit();

            if (triggerOnce)
                Destroy(gameObject);
        }
    }
}