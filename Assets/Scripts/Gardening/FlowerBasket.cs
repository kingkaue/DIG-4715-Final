using System.Collections;
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
            Debug.Log("[FlowerBasket] Interaction input detected");

            // Checks if player is close enough to basket to interact
            float distance = Vector3.Distance(gameObject.transform.position, player.transform.position);
            Debug.Log($"[FlowerBasket] Player distance: {distance}");

            if (distance <= 2)
            {
                Debug.Log("[FlowerBasket] Player is within range - attempting to spawn flower");
                TrySpawnFlower();
            }
            else
            {
                Debug.LogWarning("[FlowerBasket] Player too far away to interact");
            }
        }
    }

    void Start()
    {
        Debug.Log("[FlowerBasket] Initializing basket");

        // Assigns all the gameobjects
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) Debug.LogError("[FlowerBasket] Player not found!");

        gameManagerScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        if (gameManagerScript == null) Debug.LogError("[FlowerBasket] GameManager not found!");

        objectgrabpointtransform = player.transform.Find("riggedplayermodel/root/pelvis/CC_Base_Pelvis/ObjectGradPointArm");
        if (objectgrabpointtransform == null) Debug.LogError("[FlowerBasket] ObjectGrabPoint not found!");

        objectPickupTransform = player.transform.Find("ObjectPickupRay");
        if (objectPickupTransform == null) Debug.LogError("[FlowerBasket] ObjectPickupRay not found!");

        Debug.Log($"[FlowerBasket] Initialized with {flowerPrefabs.Length} flower prefabs");
    }

    void TrySpawnFlower()
    {
        Debug.Log("[FlowerBasket] Attempting to spawn flower");

        bool hasFlowers = false;

        // For each flower in flower dictionary
        foreach (var flower in gameManagerScript.flowers)
        {
            Debug.Log($"[FlowerBasket] Checking {flower.Key} - Count: {flower.Value}");

            // Spawns flower if there are any
            if (flower.Value > 0)
            {
                hasFlowers = true;
                Debug.Log($"[FlowerBasket] Found available {flower.Key} - spawning");
                SpawnFlower(flower.Key);
                return;
            }
        }

        if (!hasFlowers)
        {
            Debug.LogWarning("[FlowerBasket] No more flowers available in inventory");
        }
    }

    void SpawnFlower(string flowerType)
    {
        Debug.Log($"[FlowerBasket] Attempting to spawn {flowerType}");

        GameObject prefab = FindFlowerPrefab(flowerType);

        if (prefab == null)
        {
            Debug.LogError($"[FlowerBasket] Could not find prefab for {flowerType}");
            return;
        }

        Debug.Log($"[FlowerBasket] Found prefab: {prefab.name}");

        // Spawns flower prefab on top of basket
        GameObject spawnedFlower = Instantiate(prefab, flowerSpawnPoint.position, Quaternion.identity);
        Debug.Log($"[FlowerBasket] Spawned {flowerType} at {flowerSpawnPoint.position}");

        // Update inventory
        gameManagerScript.flowers[flowerType]--;
        Debug.Log($"[FlowerBasket] Updated inventory - {flowerType} count now {gameManagerScript.flowers[flowerType]}");

        // Set up grabable component
        ObjectGrabable grabable = spawnedFlower.GetComponent<ObjectGrabable>();
        if (grabable == null)
        {
            Debug.LogError($"[FlowerBasket] No ObjectGrabable component on {spawnedFlower.name}");
            return;
        }

        Debug.Log($"[FlowerBasket] Attempting to grab flower");
        grabable.Grab(objectgrabpointtransform);

        // Set player's current grabable
        PlayerPickUpDrop playerPickup = player.GetComponent<PlayerPickUpDrop>();
        if (playerPickup == null)
        {
            Debug.LogError("[FlowerBasket] PlayerPickUpDrop component missing from player");
            return;
        }

        playerPickup.objectgrabable = grabable;
        playerPickup.animator.SetBool("IsCarrying", true);
        Debug.Log("[FlowerBasket] Successfully gave flower to player");
    }

    private GameObject FindFlowerPrefab(string flower)
    {
        Debug.Log($"[FlowerBasket] Searching for prefab matching: {flower}");

        // Checks dictionary name to see if there are any prefabs that contain the flower name
        foreach (GameObject prefab in flowerPrefabs)
        {
            Debug.Log($"[FlowerBasket] Checking prefab: {prefab.name}");

            if (prefab.name.Contains(flower))
            {
                Debug.Log($"[FlowerBasket] Found matching prefab: {prefab.name}");
                return prefab;
            }
        }

        Debug.LogWarning($"[FlowerBasket] No prefab found for {flower}");
        return null;
    }
}