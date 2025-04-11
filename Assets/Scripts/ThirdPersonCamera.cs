using UnityEngine;
using UnityEngine.Rendering;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform player;
    private Vector2 move;
    private Vector2 look;
    public Rigidbody rb;

    public float rotationSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        move = player.GetComponent<PlayerMovement>().move;
        look = player.GetComponent<PlayerMovement>().look;
    }

    void FixedUpdate()
    {
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        float horizontalInput = move.x;
        float verticalInput = move.y;
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (inputDir != Vector3.zero)
        {
            rb.rotation = Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(inputDir), Time.deltaTime * rotationSpeed);
        }
    }
}
