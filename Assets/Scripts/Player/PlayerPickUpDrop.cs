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
    [Header("References")]
    [SerializeField] private Transform objectPickupTransform;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private LayerMask pickupLayerMask;
    [SerializeField] private PlayerManager playerManager;
    public Animator animator;

    [Header("Settings")]
    public float pickupDistance = 3f;
    public bool canPutInInventory = false;

    [Header("Runtime")]
    public ObjectGrabable currentGrabbedObject;
    private bool _canInteract = true;

    private void Start()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    public void OnPickupDrop(InputAction.CallbackContext context)
    {
        if (context.performed && _canInteract)
        {
            TryPickUpOrDrop();
        }
    }

    private void TryPickUpOrDrop()
    {
        if (!canPutInInventory)
        {
            if (currentGrabbedObject == null) TryPickUpObject();
            else TryDropObject();
        }
        else
        {
            TryAddToInventory();
        }
    }

    private void TryPickUpObject()
    {
        if (Physics.Raycast(objectPickupTransform.position, objectPickupTransform.forward,
            out RaycastHit hit, pickupDistance, pickupLayerMask))
        {
            if (hit.transform.TryGetComponent(out ObjectGrabable grabable) && !grabable.isInventoryItem)
            {
                StartPickup(grabable);
            }
        }
    }

    private void StartPickup(ObjectGrabable grabable)
    {
        currentGrabbedObject = grabable;
        grabable.Grab(objectGrabPointTransform);

        if (animator != null)
        {
            animator.SetBool("IsPickingUp", true);
            animator.SetBool("IsCarrying", false);
        }

        Invoke(nameof(FinishPickup), 0.3f);
    }

    private void FinishPickup()
    {
        if (animator != null)
        {
            animator.SetBool("IsPickingUp", false);
            animator.SetBool("IsCarrying", true);
        }
    }

    private void TryDropObject()
    {
        if (currentGrabbedObject != null && currentGrabbedObject.canBeDropped)
        {
            currentGrabbedObject.Drop();
            currentGrabbedObject = null;

            if (animator != null)
            {
                animator.SetBool("IsCarrying", false);
            }
        }
    }

    private void TryAddToInventory()
    {
        if (Physics.Raycast(objectPickupTransform.position, objectPickupTransform.forward,
            out RaycastHit hit, pickupDistance, pickupLayerMask))
        {
            if (hit.transform.TryGetComponent(out ObjectGrabable grabable) && grabable.isInventoryItem)
            {
                grabable.AddToInventory();

                if (animator != null)
                {
                    animator.SetBool("IsPickingUp", true);
                    Invoke(nameof(ResetPickupState), 0.5f);
                }
            }
        }
    }

    private void ResetPickupState()
    {
        if (animator != null) animator.SetBool("IsPickingUp", false);
    }
}