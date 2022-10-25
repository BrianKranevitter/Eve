using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Menu
{
    public sealed class MainMenu : MonoBehaviour
    {
        private const string up = "Up";
        private const string down = "Down";

        [SerializeField]
        private Animator animator;

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

        public void PlayGame() => SceneManager.LoadScene("Tutorial");

        public void Quit() => Application.Quit();
    }
}