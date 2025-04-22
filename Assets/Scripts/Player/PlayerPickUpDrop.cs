using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerPickUpDrop : MonoBehaviour
{
    [SerializeField] private Transform objectPickupTransform;
    [SerializeField] private Transform objectgrabpointtransform;
    [SerializeField] private LayerMask pickuplayermask;
    [SerializeField] private PlayerManager playerManager;
    private float interact;
    private bool canInteract = true;
    public bool canPutInInventory = false;
    public ObjectGrabable objectgrabable;
    public FlowerGarden flowerGarden;
    public Animator animator; // Reference to the Animator component

    private void Start()
    {
        // Get the Animator component (assuming it's on the same GameObject)
        animator = GetComponent<Animator>();

        // If Animator is on a child object, use:
        // animator = GetComponentInChildren<Animator>();
    }

    public void OnPickupDrop(InputAction.CallbackContext context)
    {
        if (canInteract == true)
        {
            TryPickUpOrDrop();
            Debug.Log("Picked up");
        }
        else
        {
            Debug.Log("Cannot pickup yet");
        }
    }

    void Update()
    {
        Debug.DrawRay(objectPickupTransform.position, objectPickupTransform.forward * 5, Color.red);
    }

    private void TryPickUpOrDrop()
    {
        // Checks which scene player is in
        // Change this later
        // Picks up object and carries it if in prototype scene
        if (!canPutInInventory)
        {
            if (objectgrabable == null)
            {
                if (animator != null)
                {
                    animator.SetBool("IsPickingUp", true);
                    animator.SetBool("IsCarrying", false);
                }

                float pickupdistance = 5;
                if (Physics.Raycast(objectPickupTransform.position, objectPickupTransform.forward,
                    out RaycastHit raycastHit, pickupdistance, pickuplayermask))
                {

                    if (raycastHit.transform.TryGetComponent(out objectgrabable))
                    {
                        objectgrabable.Grab(objectgrabpointtransform);
                        StartCoroutine(SetCarryingAfterDelay(0.1f));
                    }
                    else ResetPickupState();
                }
                else ResetPickupState();
            }
            else
            {
                if (objectgrabable.canBeDropped)
                {
                    float placeDistance = 5;
                    if (Physics.Raycast(objectPickupTransform.position, objectPickupTransform.forward, out RaycastHit hit, placeDistance, pickuplayermask))
                    {
                        if (!objectgrabable.name.Contains("poppy"))
                        {
                            Debug.Log("Can't plant this");
                            return;
                        }
                        else
                        {
                            if (hit.transform.TryGetComponent(out flowerGarden))
                            {
                                objectgrabable.Drop();
                                flowerGarden.PlaceFlower(objectgrabable.gameObject);
                                playerManager.SetSpirit(5);
                                objectgrabable = null;
                                ResetPickupState();
                            }
                        }
                    }
                    else
                    {
                        objectgrabable.Drop();
                        objectgrabable = null;
                        ResetPickupState();
                    }

                    // Explicitly set IsCarrying to false when dropping
                    if (animator != null)
                    {
                        animator.SetBool("IsCarrying", false);
                    }
                }
            }
        }

        // Picks up object and adds to inventory if it programming scene
        else if (canPutInInventory)
        {
            if (animator != null)
            {
                animator.SetBool("IsPickingUp", true);
            }

            float pickupdistance = 5;
            if (Physics.Raycast(objectPickupTransform.position, objectPickupTransform.forward,
                out RaycastHit raycastHit, pickupdistance, pickuplayermask))
            {
                if (raycastHit.transform.TryGetComponent(out objectgrabable))
                {
                    if (objectgrabable.objectName == "poppy")
                    {
                        playerManager.SetSpirit(2);
                    }

                    objectgrabable.AddFlower();
                    StartCoroutine(PutInInventory(3f, raycastHit.transform.gameObject));
                }
                else ResetPickupState();
            }
            else ResetPickupState();
        }
    }

    private IEnumerator SetCarryingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Wait until object is fully grabbed (if still moving)
        while (objectgrabable != null && !objectgrabable.isGrabbed)
        {
            yield return null;
        }

        if (animator != null)
        {
            animator.SetBool("IsPickingUp", false);
            animator.SetBool("IsCarrying", true);
        }
    }

    private IEnumerator PutInInventory(float delay, GameObject pickedObject)
    {
        canInteract = false;
        yield return new WaitForSeconds(delay);
        canInteract = true;

        if (animator != null)
        {
            animator.SetBool("IsPickingUp", false);
        }

        Destroy(pickedObject);
    }

    private void ResetPickupState()
    {
        if (animator != null)
        {
            animator.SetBool("IsPickingUp", false);
            // Don't set IsCarrying here - we only want to do that when explicitly dropping
        }
    }
}