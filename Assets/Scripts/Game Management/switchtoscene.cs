using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class switchtoscene : MonoBehaviour
{
    [SerializeField] private string scene;
    [SerializeField] private bool isReturnToHub = false;
    [SerializeField] private bool gameStart = false;
    private ASyncLoader asyncLoader;

    private void Start()
    {
        // Find AsyncLoader more safely
        var loaderObj = GameObject.FindGameObjectWithTag("AsyncLoader");
        if (loaderObj != null)
        {
            asyncLoader = loaderObj.GetComponent<ASyncLoader>();
        }
        else
        {
            Debug.LogWarning("AsyncLoader not found in scene");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(HandleSceneTransition(other.gameObject));
        }
    }

    private IEnumerator HandleSceneTransition(GameObject player)
    {
        if (asyncLoader == null)
        {
            Debug.LogError("AsyncLoader reference is missing!");
            yield break;
        }

        if (isReturnToHub)
        {
            yield return ReturnToHub(player);
        }
        else if (gameStart)
        {
            yield return asyncLoader.LoadLevelASync("Cabin Scene");
        }
        else
        {
            yield return LoadNewScene(player);
        }
    }

    private IEnumerator LoadNewScene(GameObject player)
    {
        if (scene == "Forest Trail")
        {
            var pickup = player.GetComponent<PlayerPickUpDrop>();
            if (pickup != null)
            {
                pickup.canPutInInventory = true;
            }
        }

        // Show loading screen
        if (asyncLoader.loadingscreen != null)
        {
            asyncLoader.loadingscreen.SetActive(true);
        }

        // Start async loading
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        loadOperation.allowSceneActivation = false;

        // Update loading progress
        while (!loadOperation.isDone)
        {
            if (asyncLoader.loadingSlider != null)
            {
                float progress = Mathf.Clamp01(loadOperation.progress / 0.9f);
                asyncLoader.loadingSlider.value = progress;
            }

            if (loadOperation.progress >= 0.9f)
            {
                loadOperation.allowSceneActivation = true;
            }

            yield return null;
        }

        Scene newScene = SceneManager.GetSceneByName(scene);
        if (!newScene.IsValid())
        {
            Debug.LogError("Failed to load new scene: " + scene);
            yield break;
        }

        Transform spawnPoint = FindSpawnPoint(newScene, "Spawnpoint");
        if (spawnPoint != null)
        {
            MovePlayerToScene(player, newScene, spawnPoint);
        }

        SceneManager.SetActiveScene(newScene);

        // Hide loading screen
        if (asyncLoader.loadingscreen != null)
        {
            asyncLoader.loadingscreen.SetActive(false);
        }
    }

    private IEnumerator ReturnToHub(GameObject player)
    {
        var pickup = player.GetComponent<PlayerPickUpDrop>();
        if (pickup != null)
        {
            pickup.canPutInInventory = false;
        }

        Scene hubScene = SceneManager.GetSceneByName("Cabin Scene");
        if (!hubScene.IsValid())
        {
            // Load hub scene if not already loaded
            AsyncOperation loadHub = SceneManager.LoadSceneAsync("Cabin Scene", LoadSceneMode.Additive);
            yield return loadHub;
            hubScene = SceneManager.GetSceneByName("Cabin Scene");
        }

        Transform spawnPoint = FindSpawnPoint(hubScene, "ReturnSpawnpoint");
        if (spawnPoint != null)
        {
            MovePlayerToScene(player, hubScene, spawnPoint);
        }

        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name != "Cabin Scene")
        {
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(currentScene);
            yield return unloadOperation;
        }

        SceneManager.SetActiveScene(hubScene);

        if (asyncLoader.loadingscreen != null)
        {
            asyncLoader.loadingscreen.SetActive(false);
        }
    }

    private Transform FindSpawnPoint(Scene scene, string tag)
    {
        GameObject[] roots = scene.GetRootGameObjects();
        foreach (GameObject root in roots)
        {
            if (root.CompareTag(tag))
            {
                return root.transform;
            }
        }
        return null;
    }

    private void MovePlayerToScene(GameObject player, Scene targetScene, Transform spawnPoint)
    {
        SceneManager.MoveGameObjectToScene(player, targetScene);
        player.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

        // Use try-catch to handle potential missing objects
        try
        {
            GameObject cameraHandler = GameObject.FindGameObjectWithTag("Camera Handler");
            if (cameraHandler != null)
            {
                SceneManager.MoveGameObjectToScene(cameraHandler, targetScene);
            }

            GameObject playerCam = GameObject.FindGameObjectWithTag("MainCamera");
            if (playerCam != null)
            {
                SceneManager.MoveGameObjectToScene(playerCam, targetScene);
            }

            GameObject gameManager = GameObject.FindGameObjectWithTag("GameController");
            if (gameManager != null)
            {
                SceneManager.MoveGameObjectToScene(gameManager, targetScene);
            }
        }
        catch (MissingReferenceException e)
        {
            Debug.LogWarning("Object reference missing during scene transition: " + e.Message);
        }
    }
}