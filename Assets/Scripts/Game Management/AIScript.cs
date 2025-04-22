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
            GameObject player = GameObject.FindGameObjectWithTag("Pickup Item");
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
        if (!m_CaughtPlayer)
        {
            Move(speedRun);

            // Find all potential targets
            List<Transform> targets = new List<Transform>();
            GameObject player = GameObject.FindGameObjectWithTag("Pickup Item");
            GameObject pickup = GameObject.FindGameObjectWithTag("Pickup Item");

            if (player != null) targets.Add(player.transform);
            if (pickup != null) targets.Add(pickup.transform);

            if (targets.Count == 0)
            {
                ReturnToPatrol();
                return;
            }

            // Find the closest reachable target
            Transform closestTarget = null;
            float minDistance = float.MaxValue;
            NavMeshPath path = new NavMeshPath();

            foreach (Transform target in targets)
            {
                if (navMeshAgent.CalculatePath(target.position, path))
                {
                    if (path.status == NavMeshPathStatus.PathComplete)
                    {
                        float distance = Vector3.Distance(transform.position, target.position);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closestTarget = target;
                        }
                    }
                }
            }

            if (closestTarget != null)
            {
                m_PlayerPosition = closestTarget.position;
                navMeshAgent.SetDestination(m_PlayerPosition);

                // Check if we've reached the target
                if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                {
                    if (!navMeshAgent.pathPending && navMeshAgent.velocity.sqrMagnitude == 0f)
                    {
                        if (m_WaitTime <= 0)
                        {
                            ReturnToPatrol();
                        }
                        else
                        {
                            m_WaitTime -= Time.deltaTime;
                        }
                    }
                }
            }
            else
            {
                ReturnToPatrol();
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
        Collider[] playerTargets = Physics.OverlapSphere(transform.position, viewRadius, playerMask);
        GameObject[] pickupTargets = GameObject.FindGameObjectsWithTag("Pickup Item");

        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        // Check player targets first
        foreach (Collider col in playerTargets)
        {
            float distance = Vector3.Distance(transform.position, col.transform.position);
            if (distance < closestDistance)
            {
                Vector3 dirToTarget = (col.transform.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2 || distance < 5f) // Always detect very close targets
                {
                    if (!Physics.Raycast(transform.position, dirToTarget, distance, obstacleMask))
                    {
                        closestDistance = distance;
                        closestTarget = col.transform;
                    }
                }
            }
        }

        // Then check pickup items if no player target found
        if (closestTarget == null)
        {
            foreach (GameObject obj in pickupTargets)
            {
                float distance = Vector3.Distance(transform.position, obj.transform.position);
                if (distance < viewRadius && distance < closestDistance)
                {
                    Vector3 dirToTarget = (obj.transform.position - transform.position).normalized;
                    if (!Physics.Raycast(transform.position, dirToTarget, distance, obstacleMask))
                    {
                        closestDistance = distance;
                        closestTarget = obj.transform;
                    }
                }
            }
        }

        if (closestTarget != null)
        {
            m_IsPatrol = false;
            m_PlayerInRange = true;
            m_PlayerPosition = closestTarget.position;
        }
        else if (m_PlayerInRange)
        {
            // Lost sight of target
            m_PlayerInRange = false;
            ReturnToPatrol();
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
        if ((collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Pickup Item")))
        {
            // Don't stop the agent completely, just reduce speed
            navMeshAgent.speed = speedWalk * 0.5f;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if ((collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Pickup Item")))
        {
            // Restore appropriate speed based on current state
            navMeshAgent.speed = m_IsPatrol ? speedWalk : speedRun;
        }
    }

    IEnumerator ResumeAfterCollision()
    {
        yield return new WaitForSeconds(0.5f); // Adjust delay as needed
        navMeshAgent.isStopped = false;
        isColliding = false;
    }
}