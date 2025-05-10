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

    private void LateUpdate()
    {
        if (navMeshAgent.isOnNavMesh)
        {
            transform.position = new Vector3(
                transform.position.x,
                navMeshAgent.nextPosition.y,
                transform.position.z
            );
        }
    }
    void Awake()
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

        navMeshAgent.updateRotation = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        navMeshAgent.stoppingDistance = 0.5f;
        navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        navMeshAgent.avoidancePriority = 50;
        navMeshAgent.angularSpeed = 120f;
        navMeshAgent.acceleration = 8f;
    }

    void Update()
    {
        EnvironmentView();

        if (!m_IsPatrol && !m_PlayerInRange)
        {
            ReturnToPatrol();
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
        if (!m_CaughtPlayer && m_PlayerInRange)
        {
            Move(speedRun);
            navMeshAgent.SetDestination(m_PlayerPosition);

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

        foreach (Collider col in playerTargets)
        {
            float distance = Vector3.Distance(transform.position, col.transform.position);
            Vector3 dirToTarget = (col.transform.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2 || distance < 5f)
            {
                if (!Physics.Raycast(transform.position, dirToTarget, distance, obstacleMask))
                {
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestTarget = col.transform;
                    }
                }
            }
        }

        if (closestTarget == null)
        {
            foreach (GameObject obj in pickupTargets)
            {
                float distance = Vector3.Distance(transform.position, obj.transform.position);
                if (distance < viewRadius)
                {
                    Vector3 dirToTarget = (obj.transform.position - transform.position).normalized;
                    if (!Physics.Raycast(transform.position, dirToTarget, distance, obstacleMask))
                    {
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestTarget = obj.transform;
                        }
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
            m_PlayerInRange = false;
            ReturnToPatrol();
        }
    }

    void RotateTowardsMovementDirection()
    {
        if (!m_IsPatrol && m_PlayerInRange)
        {
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
            Vector3 moveDirection = navMeshAgent.velocity.normalized;

            if (Mathf.Abs(moveDirection.x) > Mathf.Abs(moveDirection.z))
            {
                if (moveDirection.x > 0)
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                else
                    transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                if (moveDirection.z > 0)
                    transform.rotation = Quaternion.Euler(0, 270, 0);
                else
                    transform.rotation = Quaternion.Euler(0, 90, 0);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        bool validTarget = ((1 << collision.gameObject.layer) & playerMask.value) != 0 ||
                          collision.gameObject.CompareTag("Pickup Item");

        if (validTarget)
        {
            navMeshAgent.speed = speedWalk * 0.5f;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        bool validTarget = ((1 << collision.gameObject.layer) & playerMask.value) != 0 ||
                          collision.gameObject.CompareTag("Pickup Item");

        if (validTarget)
        {
            navMeshAgent.speed = m_IsPatrol ? speedWalk : speedRun;
        }
    }
}