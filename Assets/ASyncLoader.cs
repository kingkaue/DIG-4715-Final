using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ASyncLoader : MonoBehaviour
{
    public  GameObject loadingscreen;
    public  GameObject mainMenu;
    public Slider loadingSlider;

    public void LoadLevelBtn(string levelToLoad)
    {
        mainMenu?.SetActive(false);
        loadingscreen?.SetActive(true);
        StartCoroutine(LoadLevelASync(levelToLoad));
    }

    public IEnumerator LoadLevelASync(string levelToLoad)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad);
        loadOperation.allowSceneActivation = false; // Prevent auto-switch

        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            loadingSlider.value = progressValue;

            if (loadOperation.progress >= 0.9f)
            {
                loadOperation.allowSceneActivation = true; // Allow scene activation
            }

            yield return null;
        }

        loadingscreen?.SetActive(false); // Hide loading screen when done
    }
}