using UnityEngine;

public class Butterfly : MonoBehaviour
{
    private float speed;
    private float directionChangeTimer;
    private float directionChangeInterval;
    private float maxWanderDistance;
    private Vector3 startPosition; // Now relative to spawner
    private Vector3 targetPosition;
    private Animator animator;
    private Transform spawnerTransform; // Reference to spawner

    public void Initialize(float speed, float directionChangeInterval, float maxWanderDistance, Transform spawner)
    {
        this.speed = speed;
        this.directionChangeInterval = directionChangeInterval;
        this.maxWanderDistance = maxWanderDistance;
        this.spawnerTransform = spawner;
        this.startPosition = spawner.position; // Use spawner's position as center
        animator = GetComponent<Animator>();
        ChooseNewTarget();
    }

    void Update()
    {
        directionChangeTimer += Time.deltaTime;

        if (directionChangeTimer >= directionChangeInterval)
        {
            ChooseNewTarget();
            directionChangeTimer = 0f;
        }

        MoveToTarget();
    }

    void ChooseNewTarget()
    {
        // Get random point around spawner's position
        Vector2 randomCircle = Random.insideUnitCircle * maxWanderDistance;
        targetPosition = startPosition + new Vector3(
            randomCircle.x,
            Random.Range(-1f, 1f),
            randomCircle.y
        );

        // Face the new direction (optional: only if using forward movement)
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        // Optional: Trigger flutter animation
        if (animator != null)
        {
            animator.SetFloat("Speed", speed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("net"))
        {
            Destroy(gameObject);
        }
    }
}