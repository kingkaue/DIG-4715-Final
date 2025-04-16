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

    public void Initialize(float speed, float directionChangeInterval, float maxWanderDistance,
                         Vector3 spawnerPosition, float minHeight, float maxHeight)
    {
        this.speed = speed;
        this.directionChangeInterval = directionChangeInterval;
        this.maxWanderDistance = maxWanderDistance;
        this.spawnerCenter = spawnerPosition;
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;

        // Animator setup
        animator = GetComponent<Animator>();
        hasAnimator = animator != null;

        flutterOffset = Random.Range(0f, 10f);

        // Initialize movement state
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
        Vector3 toTarget = targetPosition - transform.position;
        float distance = toTarget.magnitude;

        if (distance > 0.5f)
        {
            transform.position += toTarget.normalized * speed * Time.deltaTime;
        }

        float distanceFromCenter = Vector3.Distance(
            new Vector3(transform.position.x, 0, transform.position.z),
            new Vector3(spawnerCenter.x, 0, spawnerCenter.z)
        );

        if (distanceFromCenter < maxWanderDistance * 0.3f)
        {
            ChooseNewTarget();
        }
    }

    void ApplyFlutterEffect()
    {
        float flutter = Mathf.Sin((Time.time + flutterOffset) * FLUTTER_SPEED) * FLUTTER_AMOUNT;
        transform.position += Vector3.up * flutter * Time.deltaTime;

        // Safe animator parameter control
        if (hasAnimator)
        {
            // Use whichever parameter your animator actually has
            if (AnimatorParameterExists("FlutterSpeed"))
            {
                animator.SetFloat("FlutterSpeed", speed * (1 + Mathf.Abs(flutter)));
            }
            // OR if you have a different parameter name
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