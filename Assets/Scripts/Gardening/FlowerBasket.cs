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
        if (context.performed)
        {
            if (Vector3.Distance(gameObject.transform.position, player.transform.position) <= 2)
            {
                TrySpawnFlower();
            }
        }
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gameManagerScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        objectgrabpointtransform = player.transform.Find("ObjectGradPointArm");
        objectPickupTransform = player.transform.Find("ObjectPickupRay");
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
        foreach (GameObject prefab in flowerPrefabs)
        {
            if (prefab.name.Contains(flower))
            {
                return prefab;
            }
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
