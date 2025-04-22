using System.Collections;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlowerBasket : MonoBehaviour
{
    [SerializeField] private Transform objectPickupTransform;
    [SerializeField] private Transform objectgrabpointtransform;
    [SerializeField] Transform flowerSpawnPoint;
    [SerializeField] GameObject[] flowerPrefabs;
    private GameManager gameManagerScript;
    private GameObject player;

    public void OnInteract(InputAction.CallbackContext context)
    {
        // When interacting with basket will try to spawn flower
        if (context.performed)
        {
            // Checks if player is close enough to basket to interact
            if (Vector3.Distance(gameObject.transform.position, player.transform.position) <= 2)
            {
                Debug.Log("Trying to get out of basket");
                TrySpawnFlower();
            }
        }
    }

    void Start()
    {
        // Assigns all the gameobjects
        player = GameObject.FindGameObjectWithTag("Player");
        gameManagerScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        objectgrabpointtransform = player.transform.Find("riggedplayermodel/root/pelvis/CC_Base_Pelvis/ObjectGradPointArm");
        objectPickupTransform = player.transform.Find("ObjectPickupRay");
    }

    void TrySpawnFlower()
    {
        // For each flower in flower dictionary
        foreach (var flower in gameManagerScript.flowers)
        {
            // Spawns flower if there are any
            if (flower.Value > 0)
            {
                SpawnFlower(flower.Key);
                return;
            }
        }

        Debug.Log("No more flowers");
    }

    void SpawnFlower(string flowerType)
    {
        GameObject prefab = FindFlowerPrefab(flowerType);

        // Spawns flower prefab on top of basket and immediately starts moving it to the player's hand
        if (prefab != null)
        {
            GameObject spawnedFlower = Instantiate(prefab, flowerSpawnPoint.position, Quaternion.identity);
            gameManagerScript.flowers[flowerType]--;
            spawnedFlower.GetComponent<ObjectGrabable>().Grab(objectgrabpointtransform);
            player.GetComponent<PlayerPickUpDrop>().objectgrabable = spawnedFlower.GetComponent<ObjectGrabable>();
            player.GetComponent<PlayerPickUpDrop>().animator.SetBool("IsCarrying", true);
        }
    }

    private GameObject FindFlowerPrefab(string flower)
    {
        // Checks dictionary name to see if there are any prefabs that contain the flower name
        foreach (GameObject prefab in flowerPrefabs)
        {
            if (prefab.name.Contains(flower))
            {
                return prefab;
            }
        }
        return null;
    }
}
