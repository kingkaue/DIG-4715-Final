using UnityEngine;

public class ObjectGrabable : MonoBehaviour
{
    private Rigidbody objectrigidbody;
    private Transform objectGrabPointTransform;

    private void Awake()
    {
        objectrigidbody = GetComponent<Rigidbody>();
    }

    public void Grab(Transform objectGrabPointTransform)
    {
        this.objectGrabPointTransform = objectGrabPointTransform;
        objectrigidbody.useGravity = false;
        objectrigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative; // Smoother
        gameObject.layer = LayerMask.NameToLayer("NoPlayerCollision"); // Move to a non-colliding layer
    }

    public void Drop()
    {
        gameObject.layer = LayerMask.NameToLayer("Default"); // Restore original layer
        objectrigidbody.useGravity = true;
        objectGrabPointTransform = null;
    }

    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null)
        {
            float lerpSpeed = 50f;
            Vector3 newposition = Vector3.Lerp(transform.position, objectGrabPointTransform.position, Time.deltaTime * lerpSpeed);
            objectrigidbody.MovePosition(newposition);
        }
    }
}
