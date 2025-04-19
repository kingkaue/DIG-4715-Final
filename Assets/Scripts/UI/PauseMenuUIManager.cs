using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuUIManager : MonoBehaviour
{
    public static bool isPaused = false;
    public Button resumeButton;
    public Button quitButton;
    public Button settingsButton;

    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] GameObject settingsMenuUI;

    void Start()
    {
        resumeButton.onClick.AddListener(Resume);
        settingsButton.onClick.AddListener(Settings);
        quitButton.onClick.AddListener(QuitToMenu);
        Cursor.lockState = CursorLockMode.Locked;
    }

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
        Debug.Log("resumed");
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void QuitToMenu()
    {
        Debug.Log("mainmenu");
        Time.timeScale = 1f;

        if (SceneManager.GetActiveScene().name != "Therapy Scene" && SceneManager.GetActiveScene().name != "tutorial")
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<DontDestroy>().DestroyObject();
            GameObject.FindGameObjectWithTag("GameController").GetComponent<DontDestroy>().DestroyObject();
            GameObject.FindGameObjectWithTag("Camera Handler").GetComponent<DontDestroy>().DestroyObject();
            GameObject.FindGameObjectWithTag("UICanvas").GetComponent<DontDestroy>().DestroyObject();
            GameObject.Find("PlayerCam").GetComponent<DontDestroy>().DestroyObject();
        }

        SceneManager.LoadScene("MainMenu");
        Destroy(this.gameObject);
        isPaused = false;
    }

    public void Settings()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(true);
    }
}
