using UnityEngine;

public class PlayerPickUpDrop : MonoBehaviour
{

    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectgrabpointtransform;
    [SerializeField] private LayerMask pickuplayermask;

    private ObjectGrabable objectgrabable;
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
         
        if(Input.GetKeyDown(KeyCode.E))
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
}
