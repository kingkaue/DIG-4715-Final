using UnityEngine;

public class BirdBehavior : MonoBehaviour
{
    [Header("Movement Settings")]
    public float flightSpeed = 5f;
    public float rotationSpeed = 3f;
    public float minWanderTime = 3f;
    public float maxWanderTime = 10f;
    public float wanderRadius = 15f;
    public float minFlightHeight = 1f;
    public float maxFlightHeight = 5f;

    [Header("Bird Bath Settings")]
    public Transform birdBathTarget;
    public float bathCheckInterval = 15f;
    public float bathStayDuration = 10f;
    public float landingApproachDistance = 2f;
    public float landingSpeed = 2f;
    public float bathLandingHeight = 0.2f; // How high above the bath the bird lands

    [Header("Ground Avoidance")]
    public LayerMask groundLayer; // Assign in Inspector (e.g., "Ground" layer)
    public float groundCheckDistance = 10f;
    public float groundAvoidanceForce = 5f;

    private Vector3 targetPosition;
    private bool isBathing = false;
    private bool isApproachingBath = false;
    private float nextActionTime;
    private float bathCheckTimer = 0f;

    void Start()
    {
        SetNewRandomTarget();
    }

    void Update()
    {
        if (isBathing)
        {
            if (Time.time >= nextActionTime)
            {
                EndBathing();
            }
            return;
        }

        if (isApproachingBath)
        {
            ApproachBirdBath();
            return;
        }

        // Regular wandering behavior
        bathCheckTimer += Time.deltaTime;

        if (bathCheckTimer >= bathCheckInterval && birdBathTarget != null)
        {
            float decision = Random.Range(0f, 1f);
            if (decision > 0.3f) // 70% chance to go to bath
            {
                StartApproachingBath();
                return;
            }
            bathCheckTimer = 0f;
        }

        if (Time.time >= nextActionTime)
        {
            SetNewRandomTarget();
        }

        MoveToTarget();
        AvoidGround();
    }

    void SetNewRandomTarget()
    {
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        targetPosition = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);

        // Ensure target is above ground
        RaycastHit hit;
        if (Physics.Raycast(targetPosition + Vector3.up * 50f, Vector3.down, out hit, groundCheckDistance, groundLayer))
        {
            targetPosition.y = hit.point.y + Random.Range(minFlightHeight, maxFlightHeight);
        }
        else
        {
            targetPosition.y = Random.Range(minFlightHeight, maxFlightHeight);
        }

        nextActionTime = Time.time + Random.Range(minWanderTime, maxWanderTime);
    }

    void MoveToTarget()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        transform.Translate(Vector3.forward * flightSpeed * Time.deltaTime);

        // Gentle flight bobbing effect
        float verticalWobble = Mathf.Sin(Time.time * 3f) * 0.05f;
        transform.position += Vector3.up * verticalWobble;
    }

    void AvoidGround()
    {
        RaycastHit hit;
        float safeHeight = minFlightHeight;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, safeHeight + 0.5f, groundLayer))
        {
            // Push upward if too close to ground
            float desiredHeight = hit.point.y + safeHeight;
            if (transform.position.y < desiredHeight)
            {
                transform.position = new Vector3(
                    transform.position.x,
                    Mathf.Lerp(transform.position.y, desiredHeight, Time.deltaTime * groundAvoidanceForce),
                    transform.position.z);
            }
        }
    }

    void StartApproachingBath()
    {
        if (birdBathTarget == null) return;

        isApproachingBath = true;
        targetPosition = birdBathTarget.position + Vector3.up * bathLandingHeight;
        bathCheckTimer = 0f;
    }

    void ApproachBirdBath()
    {
        float distanceToBath = Vector3.Distance(transform.position, targetPosition);

        if (distanceToBath > landingApproachDistance)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.Translate(Vector3.forward * landingSpeed * Time.deltaTime);
        }
        else
        {
            StartBathing();
        }
    }

    void StartBathing()
    {
        isApproachingBath = false;
        isBathing = true;
        nextActionTime = Time.time + bathStayDuration;

        // Snap to bath position (adjust rotation if needed)
        transform.position = birdBathTarget.position + Vector3.up * bathLandingHeight;
        transform.rotation = birdBathTarget.rotation;
    }

    void EndBathing()
    {
        isBathing = false;
        SetNewRandomTarget();
    }

    void OnDrawGizmosSelected()
    {
        // Draw wander radius
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);

        // Draw ground check
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * (minFlightHeight + 0.5f));

        // Draw current target
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(targetPosition, 0.3f);
            Gizmos.DrawLine(transform.position, targetPosition);
        }
    }
}