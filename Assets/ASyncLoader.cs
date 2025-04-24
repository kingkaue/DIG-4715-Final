using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ASyncLoader : MonoBehaviour
{
    [Header("UI References")]
    public GameObject loadingscreen;
    public GameObject mainMenu;
    public Slider loadingSlider;

    [Header("Settings")]
    public bool destroyOnLoad = false;

    private void Awake()
    {
        if (!destroyOnLoad)
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void LoadLevelBtn(string levelToLoad)
    {
        mainMenu?.SetActive(false);
        loadingscreen?.SetActive(true);

        StartCoroutine(LoadLevelASync(levelToLoad));
    }

    public IEnumerator LoadLevelASync(string levelToLoad)
    {
        // Reset slider
        loadingSlider.value = 0f;

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad);
        loadOperation.allowSceneActivation = false;

        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            loadingSlider.value = progressValue;

            if (progressValue >= 0.9f)
            {
                // Wait one extra frame to ensure everything is loaded
                yield return null;

                loadOperation.allowSceneActivation = true;
            }

            yield return null;
        }

        // Additional wait to ensure new scene is fully loaded
        yield return null;

        // Disable loading screen
        if (loadingscreen != null)
        {
            loadingscreen.SetActive(false);
        }

        // Destroy this object if marked to do so
        if (destroyOnLoad)
        {
            Destroy(this.gameObject);
        }
    }
}