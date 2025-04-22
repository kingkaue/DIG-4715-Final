using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUiManager : MonoBehaviour
{
    public Button playButton;
    public Button creditsButton;
    public Button controlsButton;
    public Button quitButton;
    public Button creditsBackButton;
    public Button controlsBackButton;


    public Canvas mainMenuUI;
    public Canvas creditsUI;
    public Canvas controlsUI;

    void Start()
    {
        playButton.onClick.AddListener(PlayGame);
        controlsButton.onClick.AddListener(OpenControls);
        creditsButton.onClick.AddListener(OpenCredits);
        quitButton.onClick.AddListener(QuitGame);
        creditsBackButton.onClick.AddListener(CloseCredits);
        controlsBackButton.onClick.AddListener(CloseControls);
    }

    void PlayGame()
    {
        SceneManager.LoadScene("Therapy Scene");
    }

    void OpenControls()
    {
        mainMenuUI.gameObject.SetActive(false);
        controlsUI.gameObject.SetActive(true);
    }

    void OpenCredits()
    {
        mainMenuUI.gameObject.SetActive(false);
        creditsUI.gameObject.SetActive(true);
    }

    void CloseCredits()
    {
        creditsUI.gameObject.SetActive(false);
        mainMenuUI.gameObject.SetActive(true);
    }

    void CloseControls()
    {
        controlsUI.gameObject.SetActive(false);
        mainMenuUI.gameObject.SetActive(true);
    }

    void QuitGame()
    {
        Debug.Log("Game Quit");
        Application.Quit();
    }
}
