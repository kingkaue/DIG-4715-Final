using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class switchtoscene : MonoBehaviour
{
    [SerializeField] private string scene;
    [SerializeField] private bool isReturnToHub = false;
    [SerializeField] private bool gameStart = false;
    [SerializeField] private ASyncLoader asyncLoader;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(HandleSceneTransition(other.gameObject));
        }
    }

    private IEnumerator HandleSceneTransition(GameObject player)
    {
        if (isReturnToHub)
        {
            yield return ReturnToHub(player);
        }
        else if (gameStart)
        {
            asyncLoader.LoadLevelBtn("Cabin Scene"); // Show loading screen
            yield return asyncLoader.LoadLevelASync("Cabin Scene"); // Wait for async load (FIXED)
        }
        else
        {
            asyncLoader.LoadLevelBtn(scene); // Activate loading screen
            yield return LoadNewScene(player); // Load new scene with loading screen
        }
    }

    private IEnumerator LoadNewScene(GameObject player)
    {
        if (scene == "Forest Trail")
        {
            player.GetComponent<PlayerPickUpDrop>().canPutInInventory = true;
        }

        // Start async loading
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        loadOperation.allowSceneActivation = false; // Prevent auto-switch

        // Update loading progress
        while (!loadOperation.isDone)
        {
            float progress = Mathf.Clamp01(loadOperation.progress / 0.9f);
            asyncLoader.loadingSlider.value = progress;

            if (loadOperation.progress >= 0.9f)
            {
                loadOperation.allowSceneActivation = true; // Allow scene switch
            }

            yield return null;
        }

        Scene newScene = SceneManager.GetSceneByName(scene);
        Transform spawnPoint = FindSpawnPoint(newScene, "Spawnpoint");

        if (spawnPoint != null)
        {
            MovePlayerToScene(player, newScene, spawnPoint);
        }

        SceneManager.SetActiveScene(newScene);
        asyncLoader.loadingscreen.SetActive(false); // Hide loading screen
    }

    private IEnumerator ReturnToHub(GameObject player)
    {
        player.GetComponent<PlayerPickUpDrop>().canPutInInventory = false;
        Scene hubScene = SceneManager.GetSceneByName("Cabin Scene");

        if (!hubScene.IsValid())
        {
            Debug.LogError("Hub scene not found!");
            yield break;
        }

        Transform spawnPoint = FindSpawnPoint(hubScene, "ReturnSpawnpoint");
        if (spawnPoint != null)
        {
            MovePlayerToScene(player, hubScene, spawnPoint);
        }

        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name != "Cabin Scene")
        {
            asyncLoader.LoadLevelBtn("Cabin Scene"); // Show loading screen
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(currentScene);

            while (!unloadOperation.isDone)
            {
                yield return null;
            }
        }

        SceneManager.SetActiveScene(hubScene);
        asyncLoader.loadingscreen.SetActive(false); // Hide loading screen
    }

    private Transform FindSpawnPoint(Scene scene, string tag)
    {
        // Sets all root gameobjects in an array
        GameObject[] roots = scene.GetRootGameObjects();

        // Iterates through array to find object with spawnpoint tag
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
        // Moves player game object to other scene and positions them on spawnpoint
        SceneManager.MoveGameObjectToScene(player, targetScene);
        player.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

        // Finds important game objects
        GameObject cameraHandler = GameObject.FindGameObjectWithTag("Camera Handler");
        GameObject playerCam = GameObject.FindGameObjectWithTag("MainCamera");
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameController");

        // Moves other objects to scene
        if (cameraHandler != null)
        {
            SceneManager.MoveGameObjectToScene(cameraHandler, targetScene);
            Debug.Log("Found camera handler");
        }
        else
        {
            Debug.Log("Could not find camera handler");
        }

        if (playerCam != null)
        {
            SceneManager.MoveGameObjectToScene(playerCam, targetScene);
            Debug.Log("Found player cam");
        }
        else
        {
            Debug.Log("Could not find player cam");
        }

        if (gameManager != null)
        {
            SceneManager.MoveGameObjectToScene(gameManager, targetScene);
            Debug.Log("Found gameManager");
        }
        else
        {
            Debug.Log("Could not find game manager");
        }
    }
}