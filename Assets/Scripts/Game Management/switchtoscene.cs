using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class switchtoscene : MonoBehaviour
{
    [SerializeField] private string scene;
    [SerializeField] private bool isReturnToHub = false;
    [SerializeField] private bool gameStart = false;

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
            SceneManager.LoadScene("Cabin Scene");
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
            player.GetComponent<PlayerPickUpDrop>().canPutInInventory = true;
        }

        // Load the new scene additively
        yield return SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);

        Scene newScene = SceneManager.GetSceneByName(scene);

        // Gets spawnpoint gameobject in scene
        Transform spawnPoint = FindSpawnPoint(newScene, "Spawnpoint");

        // Moves player to spawnpoint
        if (spawnPoint != null)
        {
            MovePlayerToScene(player, newScene, spawnPoint);
        }

        SceneManager.SetActiveScene(newScene);
    }

    private IEnumerator ReturnToHub(GameObject player)
    {
        player.GetComponent<PlayerPickUpDrop>().canPutInInventory = false;
        Scene hubScene = SceneManager.GetSceneByName("Cabin Scene");

        // Loads in hub scene if not already loaded in
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

        // Unload current scene if it's not the hub
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name != "Cabin Scene")
        {
            Debug.Log("Unloading Scene");
            yield return SceneManager.UnloadSceneAsync(currentScene);
        }

        SceneManager.SetActiveScene(hubScene);
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