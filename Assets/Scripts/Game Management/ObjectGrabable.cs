using UnityEngine;

public class ObjectGrabable : MonoBehaviour
{
    private Rigidbody objectRigidbody;
    private Transform objectGrabPointTransform;
    public bool isGrabbed = false;

    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();
    }

    public void Grab(Transform objectGrabPointTransform)
    {
        this.objectGrabPointTransform = objectGrabPointTransform;
        objectRigidbody.useGravity = false;
        objectRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        gameObject.layer = LayerMask.NameToLayer("NoPlayerCollision");
        isGrabbed = true;
    }

    public void Drop()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");
        objectRigidbody.useGravity = true;
        objectGrabPointTransform = null;
        isGrabbed = false;
    }

    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null)
        {
            float lerpSpeed = 50f;
            Vector3 newPosition = Vector3.Lerp(transform.position, objectGrabPointTransform.position, Time.deltaTime * lerpSpeed);
            objectRigidbody.MovePosition(newPosition);
        }
    }
}