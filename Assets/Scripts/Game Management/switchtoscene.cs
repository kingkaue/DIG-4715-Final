using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class switchtoscene : MonoBehaviour
{
    [SerializeField] private string scene;
    [SerializeField] private bool isReturnToHub = false;

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
        else
        {
            yield return LoadNewScene(player);
        }
    }

    private IEnumerator LoadNewScene(GameObject player)
    {
        player.GetComponent<PlayerPickUpDrop>().canPutInInventory = true;

        // Load the new scene additively
        yield return SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);

        Scene newScene = SceneManager.GetSceneByName(scene);
        Transform spawnPoint = FindSpawnPoint(newScene, "Spawnpoint");

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

        GameObject cameraHandler = GameObject.FindGameObjectWithTag("Camera Handler");
        GameObject playerCam = GameObject.FindGameObjectWithTag("MainCamera");
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameController");

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
