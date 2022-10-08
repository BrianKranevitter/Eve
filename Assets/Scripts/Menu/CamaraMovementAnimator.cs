using UnityEngine;

namespace Game.Menu
{
    [RequireComponent(typeof(Animator))]
    public sealed class CamaraMovementAnimator : MonoBehaviour
    {
        private const string up = "Up";
        private const string down = "Down";

        private Animator animator;

        private void Awake() => animator = GetComponent<Animator>();

        public void Up()
        {
            animator.SetBool(up, true);
            animator.SetBool(down, false);
        }

        public void Down()
        {
            animator.SetBool(up, false);
            animator.SetBool(down, true);
        }
    }
}