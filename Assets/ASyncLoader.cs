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
    private static ASyncLoader instance;

    [Header("Settings")]
    public bool destroyOnLoad = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
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

        // Load with Single mode to prevent duplicate scenes
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad, LoadSceneMode.Single);
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

        // Optional extra wait
        yield return null;

        // Disable loading screen
        if (loadingscreen != null)
        {
            loadingscreen.SetActive(false);
        }

        // Destroy loader object if necessary
        if (destroyOnLoad)
        {
            Destroy(this.gameObject);
        }
    }

}