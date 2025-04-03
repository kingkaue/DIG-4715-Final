using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuUIManager : MonoBehaviour
{
    public static bool isPaused = false;
    [SerializeField] GameObject pauseMenuUI;

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("hit pause button");
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        isPaused = false;
    }

    public void Settings()
    {

    }
}
