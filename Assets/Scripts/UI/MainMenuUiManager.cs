using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUiManager : MonoBehaviour
{
    public Button playButton;
    public Button creditsButton;
    public Button quitButton;
    public Button backButton;

    public Canvas mainMenuUI;
    public Canvas creditsUI;

    void Start()
    {
        playButton.onClick.AddListener(PlayGame);
        creditsButton.onClick.AddListener(OpenCredits);
        quitButton.onClick.AddListener(QuitGame);
        backButton.onClick.AddListener(CloseCredits);
    }

    void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
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

    void QuitGame()
    {
        Debug.Log("Game Quit");
        Application.Quit();
    }
}
