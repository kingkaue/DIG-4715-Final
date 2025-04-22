using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIScript : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public float startWaitTime = 4;
    public float speedWalk = 6;
    public float speedRun = 9;

    public float viewRadius = 15;
    public float viewAngle = 90;
    public LayerMask playerMask;
    public LayerMask obstacleMask;

    public Transform[] waypoints;

    private int m_CurrentWaypointIndex;
    private Vector3 playerLastPosition = Vector3.zero;
    private Vector3 m_PlayerPosition;

    private float m_WaitTime;
    private bool m_PlayerInRange;
    private bool m_PlayerNear;
    private bool m_IsPatrol;
    private bool m_CaughtPlayer;
    private bool isColliding = false;

    void Start()
    {
        m_PlayerPosition = Vector3.zero;
        m_IsPatrol = true;
        m_CaughtPlayer = false;
        m_PlayerInRange = false;
        m_WaitTime = startWaitTime;

        m_CurrentWaypointIndex = 0;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speedWalk;
        navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);

        navMeshAgent.updateRotation = false; // Disable automatic rotation

        // Disable physics-based movement
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // Adjust NavMeshAgent settings
        navMeshAgent.stoppingDistance = 0.5f;
        navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        navMeshAgent.avoidancePriority = 50;
        navMeshAgent.angularSpeed = 120f;
        navMeshAgent.acceleration = 8f;
    }

    void Update()
    {
        EnvironmentView();

        // Check if we should return to patrol if targets are destroyed
        if (!m_IsPatrol)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            GameObject canPickUp = GameObject.FindGameObjectWithTag("Pickup Item");

            // If both targets are destroyed or missing, return to patrol
            if ((player == null && canPickUp == null) ||
                (player == null && m_PlayerPosition == Vector3.zero) ||
                (canPickUp == null && m_PlayerPosition == Vector3.zero))
            {
                ReturnToPatrol();
                return;
            }
        }

        if (!m_IsPatrol)
        {
            Chasing();
        }
        else
        {
            Patroling();
        }

        RotateTowardsMovementDirection();
    }

    // Add this new helper method
    private void ReturnToPatrol()
    {
        m_IsPatrol = true;
        m_PlayerNear = false;
        m_PlayerInRange = false;
        m_PlayerPosition = Vector3.zero;
        Move(speedWalk);
        m_WaitTime = startWaitTime;
        navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
    }

    private void Chasing()
    {
        m_PlayerNear = false;
        playerLastPosition = Vector3.zero;

        if (!m_CaughtPlayer)
        {
            Move(speedRun);

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            GameObject canPickUp = GameObject.FindGameObjectWithTag("Pickup Item");

            // If both targets are destroyed or missing, return to patrol
            if (player == null && canPickUp == null)
            {
                ReturnToPatrol();
                return;
            }

            if (player != null && canPickUp != null)
            {
                float playerDistance = Vector3.Distance(transform.position, player.transform.position);
                float canPickUpDistance = Vector3.Distance(transform.position, canPickUp.transform.position);

                if (playerDistance < canPickUpDistance)
                {
                    m_PlayerPosition = player.transform.position;
                    navMeshAgent.SetDestination(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
                }
                else
                {
                    m_PlayerPosition = canPickUp.transform.position;
                    navMeshAgent.SetDestination(new Vector3(canPickUp.transform.position.x, transform.position.y, canPickUp.transform.position.z));
                }
            }
            else if (player != null)
            {
                m_PlayerPosition = player.transform.position;
                navMeshAgent.SetDestination(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
            }
            else if (canPickUp != null)
            {
                m_PlayerPosition = canPickUp.transform.position;
                navMeshAgent.SetDestination(new Vector3(canPickUp.transform.position.x, transform.position.y, canPickUp.transform.position.z));
            }
        }

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (m_WaitTime <= 0 && !m_CaughtPlayer)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                GameObject canPickUp = GameObject.FindGameObjectWithTag("Pickup Item");

                if ((player != null && Vector3.Distance(transform.position, player.transform.position) >= 6f &&
                    (canPickUp != null && Vector3.Distance(transform.position, canPickUp.transform.position) >= 6f)))
                {
                    ReturnToPatrol();
                }
                else if (player == null && canPickUp == null)
                {
                    ReturnToPatrol();
                }
            }
            else
            {
                Stop();
                m_WaitTime -= Time.deltaTime;
            }
        }
    }

    private void Patroling()
    {
        if (m_PlayerNear)
        {
            Move(speedWalk);
            LookingPlayer(playerLastPosition);
        }
        else
        {
            m_PlayerNear = false;
            playerLastPosition = Vector3.zero;
            navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);

            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (m_WaitTime <= 0)
                {
                    NextPoint();
                    Move(speedWalk);
                    m_WaitTime = startWaitTime;
                }
                else
                {
                    Stop();
                    m_WaitTime -= Time.deltaTime;
                }
            }
        }
    }

    void Move(float speed)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speed;
    }

    void Stop()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.speed = 0;
    }

    public void NextPoint()
    {
        m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
        navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
    }

    void CaughtPlayer()
    {
        m_CaughtPlayer = true;
    }

    void LookingPlayer(Vector3 player)
    {
        navMeshAgent.SetDestination(new Vector3(player.x, transform.position.y, player.z));

        if (Vector3.Distance(transform.position, player) <= 0.3)
        {
            if (m_WaitTime <= 0)
            {
                m_PlayerNear = false;
                Move(speedWalk);
                navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
                m_WaitTime = startWaitTime;
            }
            else
            {
                Stop();
                m_WaitTime -= Time.deltaTime;
            }
        }
    }

    void EnvironmentView()
    {
        // Detect objects in playerMask (layers)
        Collider[] playerTargets = Physics.OverlapSphere(transform.position, viewRadius, playerMask);

        // Detect objects with "Pickup Item" tag (regardless of layer)
        GameObject[] pickupTargets = GameObject.FindGameObjectsWithTag("Pickup Item");

        // Combine results
        List<Transform> allTargets = new List<Transform>();

        // Add player-mask objects
        foreach (Collider col in playerTargets)
        {
            allTargets.Add(col.transform);
        }

        // Add pickup-tagged objects (if close enough)
        foreach (GameObject obj in pickupTargets)
        {
            if (Vector3.Distance(transform.position, obj.transform.position) <= viewRadius)
            {
                allTargets.Add(obj.transform);
            }
        }

        // Now find the closest target (same as before)
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform target in allTargets)
        {
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (!Physics.Raycast(transform.position, dirToTarget, distanceToTarget, obstacleMask))
            {
                if (distanceToTarget < closestDistance)
                {
                    closestDistance = distanceToTarget;
                    closestTarget = target;
                }
            }
        }

        if (closestTarget != null)
        {
            Debug.Log($"Chasing: {closestTarget.name}");
            m_IsPatrol = false;
            m_PlayerInRange = true;
            m_PlayerPosition = closestTarget.position;
        }
        else
        {
            m_PlayerInRange = false;
        }
    }

    void RotateTowardsMovementDirection()
    {
        if (!m_IsPatrol && m_PlayerInRange)
        {
            // Chase mode - full 360° rotation towards target (unchanged)
            float distanceToPlayer = Vector3.Distance(transform.position, m_PlayerPosition);
            if (distanceToPlayer > 0.5f)
            {
                Vector3 directionToPlayer = (m_PlayerPosition - transform.position).normalized;
                directionToPlayer.y = 0;
                if (directionToPlayer != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                    float rotationOffset = 270f;
                    targetRotation *= Quaternion.Euler(0, rotationOffset, 0);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
                }
            }
        }
        else if (m_IsPatrol && navMeshAgent.velocity.sqrMagnitude > 0.01f)
        {
            // Patrol mode - 4-direction rotation
            Vector3 moveDirection = navMeshAgent.velocity.normalized;

            // Determine primary movement direction (prioritize larger axis)
            if (Mathf.Abs(moveDirection.x) > Mathf.Abs(moveDirection.z))
            {
                // Left/Right movement
                if (moveDirection.x > 0)
                    transform.rotation = Quaternion.Euler(0, 0, 0);    // Right
                else
                    transform.rotation = Quaternion.Euler(0, 180, 0); // Left
            }
            else
            {
                // Up/Down movement (relative to world space)
                if (moveDirection.z > 0)
                    transform.rotation = Quaternion.Euler(0, 270, 0);   // Up (world Z+)
                else
                    transform.rotation = Quaternion.Euler(0, 90, 0); // Down (world Z-)
            }
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Pickup Item")) && !isColliding)
        {
            // Stop the NavMeshAgent during the collision
            navMeshAgent.isStopped = true;
            isColliding = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if ((collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Pickup Item")) && isColliding)
        {
            // Resume the NavMeshAgent after a short delay
            StartCoroutine(ResumeAfterCollision());
        }
    }

    IEnumerator ResumeAfterCollision()
    {
        yield return new WaitForSeconds(0.5f); // Adjust delay as needed
        navMeshAgent.isStopped = false;
        isColliding = false;
    }
}