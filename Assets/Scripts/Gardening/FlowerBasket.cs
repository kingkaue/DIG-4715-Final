using UnityEngine;
using UnityEngine.InputSystem;

public class FlowerBasket : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private Transform flowerSpawnPoint;
    [SerializeField] private GameObject[] flowerPrefabs;

    private Transform _playerGrabPoint;
    private PlayerPickUpDrop _playerPickup;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerPickup = player.GetComponent<PlayerPickUpDrop>();
            _playerGrabPoint = player.transform.Find("riggedplayermodel/root/pelvis/CC_Base_Pelvis/ObjectGrabPointArm");
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed && IsPlayerInRange())
        {
            TrySpawnFlower();
        }
    }

    private bool IsPlayerInRange()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        return player != null &&
               Vector3.Distance(transform.position, player.transform.position) <= interactionDistance;
    }

    private void TrySpawnFlower()
    {
        foreach (var flower in GameManager.Instance.flowers)
        {
            if (flower.Value > 0)
            {
                SpawnFlower(flower.Key);
                return;
            }
        }
    }

    private void SpawnFlower(string flowerType)
    {
        GameObject prefab = FindFlowerPrefab(flowerType);
        if (prefab == null) return;

        GameObject flower = Instantiate(prefab, flowerSpawnPoint.position, Quaternion.identity);
        GameManager.Instance.flowers[flowerType]--;

        if (flower.TryGetComponent(out ObjectGrabable grabable))
        {
            grabable.isInventoryItem = _playerPickup.canPutInInventory;

            if (!grabable.isInventoryItem && _playerPickup != null)
            {
                grabable.Grab(_playerGrabPoint);
                _playerPickup.currentGrabbedObject = grabable;

                if (_playerPickup.animator != null)
                {
                    _playerPickup.animator.SetBool("IsCarrying", true);
                }
            }
        }
    }

    private GameObject FindFlowerPrefab(string flowerName)
    {
        foreach (GameObject prefab in flowerPrefabs)
        {
            if (prefab.name.Contains(flowerName))
            {
                return prefab;
            }
        }
        return null;
    }
}