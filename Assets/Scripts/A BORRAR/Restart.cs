using UnityEngine;
using UnityEngine.SceneManagement;
using Unity;

public class Restart : MonoBehaviour
{
    private void Update()
    {
        if (PauseMenu.Paused) return;
        
        if (Input.GetKeyDown(KeyCode.F2))
            LoadSCene(0);
    }

    public void LoadSCene(int number)
    {
        SceneManager.LoadScene(number);
    }
}
