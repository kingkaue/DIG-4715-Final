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
    }

    public void Drop()
    {
        this.objectGrabPointTransform = null;
        objectrigidbody.useGravity = true;

    }

    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null)
        {
            float lerpSpeed = 10f;
            Vector3 newposition = Vector3.Lerp(transform.position, objectGrabPointTransform.position, Time.deltaTime * lerpSpeed);
            objectrigidbody.MovePosition(newposition);
        }
    }
}
