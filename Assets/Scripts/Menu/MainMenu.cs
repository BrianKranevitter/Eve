using System;
using Name.Menu;
using TMPro.SpriteAssetUtilities;
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

        private void Awake()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

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