using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToCabinScene : MonoBehaviour
{
    [SerializeField] private bool isFlowerLevel;
    [SerializeField] private bool isAnimalLevel;
    [SerializeField] private bool isBugLevel;
    [SerializeField] private GameObject player;


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            player = other.gameObject;
            StartCoroutine(ReturnToCabin());
        }
    }

    private IEnumerator ReturnToCabin()
    {
        Scene cabinScene = SceneManager.GetSceneByName("Cabin Scene");

        GameObject[] cabinGameObjects = cabinScene.GetRootGameObjects();
        Transform cabinSpawn = null;

        foreach (GameObject root in cabinGameObjects)
        {
            if (isFlowerLevel)
            {
                if (root.CompareTag("Flower_CabinSpawnpoint"))
                {
                    cabinSpawn = root.transform;
                    break;
                }
            }
            else if (isAnimalLevel)
            {
                if (root.CompareTag("Animal_CabinSpawnpoint"))
                {
                    cabinSpawn = root.transform;
                }
            }
            else if (isBugLevel)
            {
                if (root.CompareTag("Bug_CabinSpawnpoint"))
                {
                    cabinSpawn = root.transform;
                }
            }
        }

        if (cabinSpawn != null)
        {
            SceneManager.MoveGameObjectToScene(player, cabinScene);
            player.transform.position = cabinSpawn.position;
        }

        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

        player.transform.position = (cabinSpawn.transform.position);
    }
}
