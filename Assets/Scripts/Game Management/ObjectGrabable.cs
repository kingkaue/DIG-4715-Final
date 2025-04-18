using UnityEngine;

public class ObjectGrabable : MonoBehaviour
{
    private Rigidbody objectRigidbody;
    private Transform objectGrabPointTransform;
    public bool isGrabbed = false;
    private bool isMovingToHand = false;
    private float grabProgress = 0f;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    [SerializeField] public string objectName;

    [Header("Grab Settings")]
    [SerializeField] private float grabDuration = 2f;
    [SerializeField] private AnimationCurve grabCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public bool canBeDropped; // Makes sure player is finished grabbing object before being allowed to drop it

    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();
    }

    public void Grab(Transform grabPoint)
    {
        Debug.Log("Grabbing");
        objectGrabPointTransform = grabPoint;
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        objectRigidbody.useGravity = false;
        objectRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        gameObject.layer = LayerMask.NameToLayer("NoPlayerCollision");
        isMovingToHand = true;
        grabProgress = 0f;
    }

    public void Drop()
    {
        if (!canBeDropped) return;

        Debug.Log("Dropping");
        gameObject.layer = LayerMask.NameToLayer("Default");
        objectRigidbody.useGravity = true;
        objectGrabPointTransform = null;
        isGrabbed = false;
        isMovingToHand = false;
    }

    public void AddFlower()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameController");
        if (gameManager == null)
        {
            Debug.Log("Game Manager not found");
        }

        GameManager gameManagerScript = gameManager.GetComponent<GameManager>();
        if (gameManagerScript == null)
        {
            Debug.Log("Game Manager Script not found");
        }
        
        if (!gameManagerScript.flowers.ContainsKey(objectName))
        {
            gameManagerScript.flowers.Add(objectName, 1);
            Debug.Log("Added " + gameManagerScript.flowers[objectName] + " " + objectName);
        }
        else
        {
            gameManagerScript.flowers[objectName]++;
            Debug.Log("There are now " + gameManagerScript.flowers[objectName] + " " + objectName);
        }
    }

    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null)
        {
            if (isMovingToHand)
            {
                canBeDropped = false;
                // Smooth movement to hand
                grabProgress += Time.fixedDeltaTime / grabDuration;
                float curveValue = grabCurve.Evaluate(grabProgress);

                objectRigidbody.MovePosition(
                    Vector3.Lerp(initialPosition, objectGrabPointTransform.position, curveValue)
                );
                objectRigidbody.MoveRotation(
                    Quaternion.Lerp(initialRotation, objectGrabPointTransform.rotation, curveValue)
                );

                if (grabProgress >= 1f)
                {
                    isMovingToHand = false;
                    isGrabbed = true;
                }
            }
            else if (isGrabbed)
            {
                canBeDropped = true;
                // Snap to hand during carry phase
                objectRigidbody.MovePosition(objectGrabPointTransform.position);
                objectRigidbody.MoveRotation(objectGrabPointTransform.rotation);
            }
        }
    }
}