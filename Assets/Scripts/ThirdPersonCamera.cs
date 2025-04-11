using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform player;
    private Vector2 move;
    private Vector2 look;
    public Rigidbody rb;
    public Animator animator; // Add this reference

    public float rotationSpeed;

    void Start()
    {
        // Get the animator from the player
        animator = player.GetComponent<Animator>();
    }

    void Update()
    {
        move = player.GetComponent<PlayerMovement>().move;
        look = player.GetComponent<PlayerMovement>().look;
    }

    void FixedUpdate()
    {
        // Don't rotate if in sitting animation
        if (animator != null && animator.GetCurrentAnimatorStateInfo(0).IsName("playermodelsitting"))
        {
            return;
        }

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