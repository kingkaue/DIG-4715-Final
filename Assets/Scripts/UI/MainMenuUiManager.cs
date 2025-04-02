using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUiManager : MonoBehaviour
{
    public Button gameplayButton;
    public Button artButton;
    public Button creditsButton;
    public Button quitButton;

    void Start()
    {
        gameplayButton.onClick.AddListener(PlayGameP);
        artButton.onClick.AddListener(PlayArt);
        creditsButton.onClick.AddListener(OpenCredits);
        quitButton.onClick.AddListener(QuitGame);
    }

    void PlayGameP()
    {
        SceneManager.LoadScene("ProgrammingTest");
    }

    void PlayArt()
    {
        SceneManager.LoadScene("Prototype Scene");
    }

    void OpenCredits()
    {

    }

    void QuitGame()
    {
        Debug.Log("Game Quit");
        Application.Quit();
    }
}
