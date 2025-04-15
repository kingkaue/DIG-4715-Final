using UnityEngine;

public class Butterfly : MonoBehaviour
{
    private float speed;
    private float directionChangeTimer;
    private float directionChangeInterval;
    private float maxWanderDistance;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Animator animator;

    public void Initialize(float speed, float directionChangeInterval, float maxWanderDistance)
    {
        this.speed = speed;
        this.directionChangeInterval = directionChangeInterval;
        this.maxWanderDistance = maxWanderDistance;
        startPosition = transform.position;
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
        // Get a random point within wander distance
        Vector2 randomCircle = Random.insideUnitCircle * maxWanderDistance;
        targetPosition = startPosition + new Vector3(randomCircle.x, Random.Range(-1f, 1f), randomCircle.y);

        // Face the new direction
        Vector3 direction = targetPosition - transform.position;
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

        // Trigger flutter animation
        if (animator != null)
        {
            //animator.SetFloat("Speed", speed);
        }
    }
}