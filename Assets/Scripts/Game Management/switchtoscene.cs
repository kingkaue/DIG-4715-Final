using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchToScene : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string scene;
    [SerializeField] private bool isReturnToHub = false;
    [SerializeField] private bool gameStart = false;

    [Header("References")]
    private ASyncLoader asyncLoader;
    private bool isTransitioning = false;

    private void Start()
    {
        FindAsyncLoader();
    }

    private void FindAsyncLoader()
    {
        GameObject loaderObj = GameObject.FindGameObjectWithTag("AsyncLoader");
        if (loaderObj != null)
        {
            asyncLoader = loaderObj.GetComponent<ASyncLoader>();
            if (asyncLoader == null)
            {
                Debug.LogError("AsyncLoader component missing on tagged object");
            }
        }
        else
        {
            Debug.LogWarning("AsyncLoader not found in scene");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            StartCoroutine(HandleSceneTransition(other.gameObject));
        }
    }

    private IEnumerator HandleSceneTransition(GameObject player)
    {
        isTransitioning = true;

        if (asyncLoader == null)
        {
            FindAsyncLoader();
            if (asyncLoader == null)
            {
                Debug.LogError("AsyncLoader reference missing! Cannot transition scenes.");
                isTransitioning = false;
                yield break;
            }
        }

        if (isReturnToHub)
        {
            yield return ReturnToHub(player);
        }
        else if (gameStart)
        {
            yield return StartGame();
        }
        else
        {
            yield return LoadNewScene(player);
        }

        isTransitioning = false;
    }

    private IEnumerator StartGame()
    {
        if (asyncLoader.loadingscreen != null)
        {
            asyncLoader.loadingscreen.SetActive(true);
        }

        yield return asyncLoader.LoadLevelASync("Cabin Scene");

        if (asyncLoader.loadingscreen != null)
        {
            asyncLoader.loadingscreen.SetActive(false);
        }
    }

    private IEnumerator LoadNewScene(GameObject player)
    {
        // Show loading screen
        if (asyncLoader.loadingscreen != null)
        {
            asyncLoader.loadingscreen.SetActive(true);
        }

        // Set inventory flag if going to Forest Trail
        if (scene == "Forest Trail")
        {
            var pickup = player.GetComponent<PlayerPickUpDrop>();
            if (pickup != null) pickup.canPutInInventory = true;
        }

        // Load scene additively
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

        // Get the newly loaded scene
        Scene newScene = SceneManager.GetSceneByName(scene);
        if (!newScene.IsValid())
        {
            Debug.LogError("Failed to load scene: " + scene);
            yield break;
        }

        // Set as active scene first
        SceneManager.SetActiveScene(newScene);

        // Handle cameras
        Camera[] newSceneCameras = GetCamerasInScene(newScene);
        foreach (Camera cam in newSceneCameras)
        {
            cam.gameObject.SetActive(false);
        }

        // Move player and find spawn point
        Transform spawnPoint = FindSpawnPoint(newScene, "Spawnpoint");
        if (spawnPoint != null)
        {
            MovePlayerToScene(player, newScene, spawnPoint);
        }

        

        // Hide loading screen
        if (asyncLoader.loadingscreen != null)
        {
            asyncLoader.loadingscreen.SetActive(false);
        }
    }

    private IEnumerator ReturnToHub(GameObject player)
    {
        // Show loading screen
        if (asyncLoader.loadingscreen != null)
        {
            asyncLoader.loadingscreen.SetActive(true);
        }

        // Disable inventory
        var pickup = player.GetComponent<PlayerPickUpDrop>();
        if (pickup != null) pickup.canPutInInventory = false;

        // Load hub if not already loaded
        Scene hubScene = SceneManager.GetSceneByName("Cabin Scene");
        if (!hubScene.IsValid())
        {
            AsyncOperation loadHub = SceneManager.LoadSceneAsync("Cabin Scene", LoadSceneMode.Additive);
            yield return loadHub;
            hubScene = SceneManager.GetSceneByName("Cabin Scene");
        }

        // Set as active scene
        SceneManager.SetActiveScene(hubScene);

        // Handle cameras in hub
        Camera[] hubCameras = GetCamerasInScene(hubScene);
        foreach (Camera cam in hubCameras)
        {
            cam.gameObject.SetActive(false);
        }

        // Move player to hub
        Transform spawnPoint = FindSpawnPoint(hubScene, "ReturnSpawnpoint");
        if (spawnPoint != null)
        {
            MovePlayerToScene(player, hubScene, spawnPoint);
        }

        // Activate player camera
        Camera playerCam = player.GetComponentInChildren<Camera>(true);
        if (playerCam != null)
        {
            playerCam.gameObject.SetActive(true);
            playerCam.tag = "MainCamera";
        }

        // Unload current scene if not hub
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name != "Cabin Scene")
        {
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(currentScene);
            yield return unloadOperation;
        }

        // Hide loading screen
        if (asyncLoader.loadingscreen != null)
        {
            asyncLoader.loadingscreen.SetActive(false);
        }
    }

    private Camera[] GetCamerasInScene(Scene scene)
    {
        List<Camera> cameras = new List<Camera>();
        GameObject[] rootObjects = scene.GetRootGameObjects();

        foreach (GameObject root in rootObjects)
        {
            cameras.AddRange(root.GetComponentsInChildren<Camera>(true));
        }

        return cameras.ToArray();
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

            Transform child = root.transform.Find(tag);
            if (child != null) return child;
        }
        return null;
    }

    private void MovePlayerToScene(GameObject player, Scene targetScene, Transform spawnPoint)
    {
        // Move player first
        SceneManager.MoveGameObjectToScene(player, targetScene);
        player.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
         
        // Move essential objects
        MoveObjectToSceneByTag("Camera Handler", targetScene);
        MoveObjectToSceneByTag("MainCamera", targetScene);
        MoveObjectToSceneByTag("GameController", targetScene);
    }

    private void MoveObjectToSceneByTag(string tag, Scene targetScene)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject obj in objects)
        {
            if (obj.scene != targetScene)
            {
                SceneManager.MoveGameObjectToScene(obj, targetScene);
            }
        }
    }
}