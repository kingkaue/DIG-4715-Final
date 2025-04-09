using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickUpDrop : MonoBehaviour
{

    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectgrabpointtransform;
    [SerializeField] private LayerMask pickuplayermask;
    private float interact;

    private ObjectGrabable objectgrabable;

    public void OnPickupDrop(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
            TryPickupOrDrop();
    }

    private void TryPickupOrDrop()
    {
        if (objectgrabable == null)
        {
            float pickupdistance = 40;
            Debug.DrawRay(playerCameraTransform.position, playerCameraTransform.forward * pickupdistance, Color.red, 1f);

            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHit, pickupdistance, pickuplayermask))
            {
                Debug.Log("Hit object: " + raycastHit.transform.name);

                if (raycastHit.transform.TryGetComponent(out objectgrabable))
                {
                    Debug.Log("Found grabable component");
                    objectgrabable.Grab(objectgrabpointtransform);
                }
                else
                {
                    Debug.Log("Hit object: " + raycastHit.transform.name);
                    Debug.Log("Object has no ObjectGrabable component");
                }
            }
            else
            {
                Debug.Log("No object hit by raycast");
            }
        }
        else
        {
            objectgrabable.Drop();
            objectgrabable = null;
        }
    }
}
