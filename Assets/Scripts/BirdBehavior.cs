using UnityEngine;
using System.Collections;

public class BirdBehavior : MonoBehaviour
{
    [Header("Movement Settings")]
    public float flightSpeed = 5f;
    public float rotationSpeed = 3f;
    public float minWanderTime = 3f;
    public float maxWanderTime = 10f;
    public float wanderRadius = 15f;

    [Header("Bird Bath Settings")]
    public Transform birdBathTarget;
    public float bathCheckInterval = 15f;
    public float bathStayDuration = 10f;
    public float landingApproachDistance = 2f;
    public float landingSpeed = 2f;

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
            // Just wait until bath time is over
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

        // Check if it's time to consider going to the bird bath
        if (bathCheckTimer >= bathCheckInterval && birdBathTarget != null)
        {
            float decision = Random.Range(0f, 1f);
            if (decision > 0.3f) // 70% chance to go to bath when timer is up
            {
                StartApproachingBath();
                return;
            }
            bathCheckTimer = 0f; // Reset timer if not going to bath
        }

        // Normal wandering
        if (Time.time >= nextActionTime)
        {
            SetNewRandomTarget();
        }

        MoveToTarget();
    }

    void SetNewRandomTarget()
    {
        // Get a random point within the wander radius
        targetPosition = transform.position + Random.insideUnitSphere * wanderRadius;
        targetPosition.y = Mathf.Clamp(targetPosition.y, 1f, 5f); // Keep the bird at reasonable height

        // Set a random time until next target change
        nextActionTime = Time.time + Random.Range(minWanderTime, maxWanderTime);
    }

    void MoveToTarget()
    {
        // Calculate direction
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Rotate towards the target
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Move forward
        transform.Translate(Vector3.forward * flightSpeed * Time.deltaTime);

        // Add some slight up/down movement for more natural flight
        float verticalWobble = Mathf.Sin(Time.time * 3f) * 0.05f;
        transform.position += Vector3.up * verticalWobble;
    }

    void StartApproachingBath()
    {
        if (birdBathTarget == null) return;

        isApproachingBath = true;
        targetPosition = birdBathTarget.position;
        bathCheckTimer = 0f;
    }

    void ApproachBirdBath()
    {
        float distanceToBath = Vector3.Distance(transform.position, birdBathTarget.position);

        if (distanceToBath > landingApproachDistance)
        {
            // Fly towards the bath
            Vector3 direction = (birdBathTarget.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.Translate(Vector3.forward * flightSpeed * Time.deltaTime);
        }
        else
        {
            // Start landing procedure
            StartBathing();
        }
    }

    void StartBathing()
    {
        isApproachingBath = false;
        isBathing = true;
        nextActionTime = Time.time + bathStayDuration;

        // Position the bird in the bath (adjust as needed)
        transform.position = birdBathTarget.position;
        transform.rotation = birdBathTarget.rotation;

        // Here you could trigger an animation or other effects
        Debug.Log("Bird is now bathing");
    }

    void EndBathing()
    {
        isBathing = false;
        SetNewRandomTarget();

        // Here you could trigger take-off animation
        Debug.Log("Bird finished bathing");
    }

    // Draw gizmos for debugging
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetPosition, 0.3f);
        Gizmos.DrawLine(transform.position, targetPosition);
    }
}