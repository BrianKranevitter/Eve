using Game.Utility;

using System;

using UnityEngine;

namespace Game.Level.Triggers
{
    [Serializable]
    public sealed class TriggerAnimatorPlayerTriggerAction : PlayerTriggerAction
    {
        [SerializeField, Tooltip("Animators to trigger.")]
        private Pack[] animators;

        [Serializable]
        private struct Pack
        {
            [Tooltip("Animator which triggers will be triggered.")]
            public Animator animator;

            [Tooltip("Name of the animation trigger that is triggered on enter.")]
            public string enterAnimationTrigger;

            [Tooltip("Name of the animation trigger that is triggered on exit.")]
            public string exitAnimationTrigger;
        }

        public override void OnTriggerEnter()
        {
            for (int i = 0; i < animators.Length; i++)
            {
                Pack pack = animators[i];

                Try.SetAnimationTrigger(pack.animator, pack.enterAnimationTrigger, "enter animation trigger", "animation");
            }
        }

        public override void OnTriggerExit()
        {
            for (int i = 0; i < animators.Length; i++)
            {
                Pack pack = animators[i];

                Try.SetAnimationTrigger(pack.animator, pack.exitAnimationTrigger, "exit animation trigger", "animation");
            }
        }
    }
}