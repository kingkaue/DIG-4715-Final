using UnityEngine;
using UnityEngine.InputSystem;

public class FlowerBasket : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform flowerSpawnPoint;
    [SerializeField] private GameObject[] flowerPrefabs;
    [SerializeField] private float interactionDistance = 2f;

    private Transform objectgrabpointtransform;
    private Transform objectPickupTransform;
    private GameManager gameManagerScript;
    private GameObject player;
    private PlayerPickUpDrop playerPickup;
    private Animator playerAnimator;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found!");
            return;
        }

        gameManagerScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        playerPickup = player.GetComponent<PlayerPickUpDrop>();
        playerAnimator = player.GetComponent<Animator>();

        // Find transforms more safely
        objectgrabpointtransform = player.transform.Find("riggedplayermodel/root/pelvis/CC_Base_Pelvis/ObjectGradPointArm");
        objectPickupTransform = player.transform.Find("ObjectPickupRay");

        if (objectgrabpointtransform == null || objectPickupTransform == null)
        {
            Debug.LogError("Required transforms not found on player!");
        }

        if (playerAnimator == null)
        {
            Debug.LogError("Player Animator not found!");
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed && player != null)
        {
            if (Vector3.Distance(transform.position, player.transform.position) <= interactionDistance)
            {
                TrySpawnFlower();
            }
        }
    }

    void TrySpawnFlower()
    {
        foreach (var flower in gameManagerScript.flowers)
        {
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
        if (prefab == null) return;

        GameObject spawnedFlower = Instantiate(prefab, flowerSpawnPoint.position, Quaternion.identity);
        gameManagerScript.flowers[flowerType]--;

        ObjectGrabable grabable = spawnedFlower.GetComponent<ObjectGrabable>();
        if (grabable == null)
        {
            Debug.LogError("Spawned flower has no ObjectGrabable component!");
            Destroy(spawnedFlower);
            return;
        }

        // Handle pickup and animation
        if (playerPickup != null)
        {
            playerPickup.objectgrabable = grabable;
            grabable.Grab(objectgrabpointtransform);

            // Trigger animation
            if (playerAnimator != null)
            {
                playerAnimator.SetBool("IsCarrying", true);
                // Also try triggering a parameter if bool isn't working
                playerAnimator.SetTrigger("PickUp");
            }
        }
    }

    private GameObject FindFlowerPrefab(string flower)
    {
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