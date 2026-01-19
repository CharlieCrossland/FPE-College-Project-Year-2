using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    private NavMeshAgent agent;
    private Animator animator;
    private EnemyHealth enemyHealth;

    [Header("AI Settings")]
    public float speed = 3.5f;
    public float detectionRange = 10f;
    public float patrolRadius = 15f;
    public float patrolWaitTime = 3f;

    private Vector3 patrolDestination;
    private bool isPatrolling = true;
    private float patrolTimer = 0f;

    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        SetNewPatrolPoint();
    }

    void Update()
    {
        if (!enemyHealth.stun)
        {
            agent.speed = speed;

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Determine behavior
            if (distanceToPlayer <= detectionRange)
            {
                // Chase player
                isPatrolling = false;
                agent.SetDestination(player.position);
            }
            else
            {
                // Patrol mode
                if (!isPatrolling)
                {
                    isPatrolling = true;
                    SetNewPatrolPoint();
                }
                Patrol();
            }

            // Update isMoving animation
            bool moving = agent.velocity.magnitude > 0.2f;
            animator.SetBool("isMoving", moving);
        }

        if (enemyHealth.stun)
        {
            agent.speed = 0f;
            StartCoroutine(StunDisable());
        }
    }

    IEnumerator StunDisable()
    {
        yield return new WaitForSeconds(1f);
        enemyHealth.stun = false;
        yield break;
    }

    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            patrolTimer += Time.deltaTime;
            if (patrolTimer >= patrolWaitTime)
            {
                SetNewPatrolPoint();
                patrolTimer = 0f;
            }
        }
    }

    void SetNewPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius + transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            patrolDestination = hit.position;
            agent.SetDestination(patrolDestination);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
    }
}