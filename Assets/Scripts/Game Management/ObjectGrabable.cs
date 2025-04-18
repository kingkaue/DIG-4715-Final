using UnityEngine;

public class ObjectGrabable : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody objectRigidbody;

    [Header("Settings")]
    [SerializeField] public string objectName;
    [SerializeField] public bool canBeDropped = true;
    [SerializeField] public bool isInventoryItem = false;
    [SerializeField] private float grabDuration = 0.3f;
    [SerializeField] private AnimationCurve grabCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Runtime")]
    public bool isGrabbed = false;
    public bool isMovingToHand = false;

    private Transform objectGrabPointTransform;
    private float grabProgress = 0f;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();
    }

    public void Grab(Transform grabPoint)
    {
        objectGrabPointTransform = grabPoint;
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        objectRigidbody.useGravity = false;
        objectRigidbody.isKinematic = true;
        gameObject.layer = LayerMask.NameToLayer("NoPlayerCollision");
        isMovingToHand = true;
        grabProgress = 0f;
    }

    public void Drop()
    {
        if (!canBeDropped) return;

        gameObject.layer = LayerMask.NameToLayer("Default");
        objectRigidbody.useGravity = true;
        objectRigidbody.isKinematic = false;
        objectGrabPointTransform = null;
        isGrabbed = false;
        isMovingToHand = false;
    }

    public void AddToInventory()
    {
        if (!isInventoryItem) return;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddFlowerToInventory(objectName);
        }
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (objectGrabPointTransform == null) return;

        if (isMovingToHand)
        {
            grabProgress += Time.fixedDeltaTime / grabDuration;
            float t = grabCurve.Evaluate(grabProgress);

            transform.position = Vector3.Lerp(initialPosition, objectGrabPointTransform.position, t);
            transform.rotation = Quaternion.Slerp(initialRotation, objectGrabPointTransform.rotation, t);

            if (grabProgress >= 1f)
            {
                isMovingToHand = false;
                isGrabbed = true;
                canBeDropped = true;
            }
        }
        else if (isGrabbed)
        {
            transform.position = objectGrabPointTransform.position;
            transform.rotation = objectGrabPointTransform.rotation;
        }
    }
}