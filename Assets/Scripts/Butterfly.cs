using UnityEngine;

public class Butterfly : MonoBehaviour
{
    // Movement Settings
    private float speed;
    private float directionChangeTimer;
    private float directionChangeInterval;
    private float maxWanderDistance;
    private float minHeight;
    private float maxHeight;

    // State
    private Vector3 spawnerCenter;
    private Vector3 targetPosition;
    private Vector3 currentDirection;
    private float currentHeight;

    // Components
    private Animator animator;
    private bool hasAnimator;

    // Flutter Effect
    private float flutterOffset;
    private const float FLUTTER_SPEED = 5f;
    private const float FLUTTER_AMOUNT = 0.1f;

    // Flocking
    private const float AVOIDANCE_RADIUS = 0.5f;
    private const float FLOCKING_RADIUS = 1.5f;

    public void Initialize(float speed, float directionChangeInterval, float maxWanderDistance,
                         Vector3 spawnerPosition, float minHeight, float maxHeight)
    {
        this.speed = speed;
        this.directionChangeInterval = directionChangeInterval;
        this.maxWanderDistance = maxWanderDistance;
        this.spawnerCenter = spawnerPosition;
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;

        animator = GetComponent<Animator>();
        hasAnimator = animator != null;

        flutterOffset = Random.Range(0f, 10f);

        currentDirection = Random.onUnitSphere;
        currentDirection.y = 0;
        currentHeight = Random.Range(minHeight, maxHeight);

        transform.position = GetNewSpawnPosition();
        transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        ChooseNewTarget();
    }

    void Update()
    {
        if (directionChangeTimer >= directionChangeInterval)
        {
            ChooseNewTarget();
            directionChangeTimer = 0f;
        }

        directionChangeTimer += Time.deltaTime;
        MoveToTarget();
        ApplyFlutterEffect();
    }

    Vector3 GetNewSpawnPosition()
    {
        return spawnerCenter +
               new Vector3(
                   Random.Range(-1f, 1f) * maxWanderDistance * 0.5f,
                   Random.Range(minHeight, maxHeight),
                   Random.Range(-1f, 1f) * maxWanderDistance * 0.5f
               );
    }

    void ChooseNewTarget()
    {
        Vector3 randomDirection = new Vector3(
            Random.Range(-1f, 1f),
            0,
            Random.Range(-1f, 1f)
        ).normalized;

        currentDirection = Vector3.Lerp(currentDirection, randomDirection, 0.5f).normalized;
        currentHeight = Mathf.Clamp(
            currentHeight + Random.Range(-0.5f, 0.5f),
            minHeight,
            maxHeight
        );

        targetPosition = spawnerCenter +
                        (currentDirection * maxWanderDistance * 0.7f) +
                        Vector3.up * currentHeight;
    }

    void MoveToTarget()
    {
        Vector3 toTarget = (targetPosition - transform.position).normalized;
        Vector3 avoidance = CalculateAvoidance();
        Vector3 flocking = CalculateFlocking();

        // Blend target direction with avoidance & flocking
        Vector3 moveDirection = (toTarget + avoidance * 1.5f + flocking * 0.5f).normalized;

        transform.position += moveDirection * speed * Time.deltaTime;

        // Rotate towards movement direction
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime * 2f);
        }

        // Keep within bounds
        float distanceFromCenter = Vector3.Distance(
            new Vector3(transform.position.x, 0, transform.position.z),
            new Vector3(spawnerCenter.x, 0, spawnerCenter.z)
        );

        if (distanceFromCenter > maxWanderDistance * 0.9f)
        {
            ChooseNewTarget();
        }
    }

    Vector3 CalculateAvoidance()
    {
        Collider[] nearby = Physics.OverlapSphere(transform.position, AVOIDANCE_RADIUS);
        Vector3 avoidance = Vector3.zero;

        foreach (Collider other in nearby)
        {
            if (other.gameObject != gameObject && other.CompareTag("Butterfly"))
            {
                avoidance += (transform.position - other.transform.position).normalized;
            }
        }

        return avoidance;
    }

    Vector3 CalculateFlocking()
    {
        Collider[] nearby = Physics.OverlapSphere(transform.position, FLOCKING_RADIUS);
        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;
        int neighbors = 0;

        foreach (Collider other in nearby)
        {
            if (other.gameObject != gameObject && other.CompareTag("Butterfly"))
            {
                alignment += other.transform.forward;
                cohesion += other.transform.position;
                neighbors++;
            }
        }

        if (neighbors > 0)
        {
            alignment /= neighbors;
            cohesion = (cohesion / neighbors - transform.position).normalized;
        }

        return (alignment + cohesion) * 0.5f;
    }

    void ApplyFlutterEffect()
    {
        float flutter = Mathf.Sin((Time.time + flutterOffset) * FLUTTER_SPEED) * FLUTTER_AMOUNT;
        transform.position += Vector3.up * flutter * Time.deltaTime;

        if (hasAnimator)
        {
            if (AnimatorParameterExists("FlutterSpeed"))
            {
                animator.SetFloat("FlutterSpeed", speed * (1 + Mathf.Abs(flutter)));
            }
            else if (AnimatorParameterExists("WingFlapSpeed"))
            {
                animator.SetFloat("WingFlapSpeed", speed);
            }
        }
    }

    private bool AnimatorParameterExists(string name)
    {
        if (!hasAnimator) return false;
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == name) return true;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("net"))
        {
            Destroy(gameObject);
        }
    }
}