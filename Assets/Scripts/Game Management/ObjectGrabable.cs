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

    [Header("Grab Settings")]
    [SerializeField] private float grabDuration = 2f;
    [SerializeField] private AnimationCurve grabCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

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
        objectRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        gameObject.layer = LayerMask.NameToLayer("NoPlayerCollision");
        isMovingToHand = true;
        grabProgress = 0f;
    }

    public void Drop()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");
        objectRigidbody.useGravity = true;
        objectGrabPointTransform = null;
        isGrabbed = false;
        isMovingToHand = false;
    }

    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null)
        {
            if (isMovingToHand)
            {
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
                // Snap to hand during carry phase
                objectRigidbody.MovePosition(objectGrabPointTransform.position);
                objectRigidbody.MoveRotation(objectGrabPointTransform.rotation);
            }
        }
    }
}