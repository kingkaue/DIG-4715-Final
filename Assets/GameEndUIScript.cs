using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEndUIScript : MonoBehaviour
{
    public Button mainMenuButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainMenuButton.onClick.AddListener(BackToMenu);
    }

    // Update is called once per frame
    void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
