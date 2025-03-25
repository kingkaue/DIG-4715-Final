using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickUpDrop : MonoBehaviour
{

    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectgrabpointtransform;
    [SerializeField] private LayerMask pickuplayermask;
    private float interact;

    private ObjectGrabable objectgrabable;
    void Start()
    {

    }

    public void OnPickupDrop(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TryPickupOrDrop();
        }
    }

    private void TryPickupOrDrop()
    {
        if (objectgrabable == null)
        {
            float pickupdistance = 20;
            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHit, pickupdistance, pickuplayermask))
            {
                if (raycastHit.transform.TryGetComponent(out objectgrabable))
                {
                    objectgrabable.Grab(objectgrabpointtransform);
                    Debug.Log(objectgrabable);
                }
            }
        }
        else
        {
            objectgrabable.Drop();
            objectgrabable = null;
        }
    }
}
