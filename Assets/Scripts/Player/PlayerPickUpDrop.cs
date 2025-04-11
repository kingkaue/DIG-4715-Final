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
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectgrabpointtransform;
    [SerializeField] private LayerMask pickuplayermask;
    private float interact;

    private ObjectGrabable objectgrabable;
    private Animator animator; // Reference to the Animator component

    private void Start()
    {
        // Get the Animator component (assuming it's on the same GameObject)
        animator = GetComponent<Animator>();

        // If Animator is on a child object, use:
        // animator = GetComponentInChildren<Animator>();
    }

    public void OnPickupDrop(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        if (SceneManager.GetActiveScene().name == "SampleScene")
        {
            TryPickUpOrDrop();
        }

        else if (SceneManager.GetActiveScene().name == "Prototype Scene")
        {
            AddToInventory();
        }
    }

    private void TryPickUpOrDrop()
    {
        if (objectgrabable == null)
        {
            if (animator != null)
            {
                animator.SetBool("IsPickingUp", true);
                animator.SetBool("IsCarrying", false);
            }

            float pickupdistance = 40;
            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward,
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
            objectgrabable.Drop();
            objectgrabable = null;
            ResetPickupState();

            // Explicitly set IsCarrying to false when dropping
            if (animator != null)
            {
                animator.SetBool("IsCarrying", false);
            }
        }
    }

    private void AddToInventory()
    {
        if (animator != null)
        {
            animator.SetBool("IsPickingUp", true);
        }

        float pickupdistance = 40;
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward,
            out RaycastHit raycastHit, pickupdistance, pickuplayermask))
        {
            if (raycastHit.transform.TryGetComponent(out objectgrabable))
            {
                objectgrabable.AddFlower();
            }
            else ResetPickupState();
        }
        else ResetPickupState();
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

    private void ResetPickupState()
    {
        if (animator != null)
        {
            animator.SetBool("IsPickingUp", false);
            // Don't set IsCarrying here - we only want to do that when explicitly dropping
        }
    }
}