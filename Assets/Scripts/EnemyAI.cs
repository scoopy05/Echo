using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // --- The Enemy's Brain ---
    public enum EnemyState { Idle, Chase }
    private EnemyState currentState = EnemyState.Idle;

    // --- Tuning Knobs (visible in Inspector) ---
    [Header("Detection")]
    public float detectionRadius = 10f;
    public Transform playerTransform;

    [Header("Patrol & Movement")]
    public float patrolRadius = 15f;     // How far from start position the enemy will roam
    public float patrolWaitTime = 2f;    // Seconds to pause at each patrol point
    public float turnSpeed = 6f;         // How fast the enemy smoothly rotates (higher = faster turn)

    // --- Internal ---
    private NavMeshAgent agent;          // The "legs" component
    private Animator animator;           // Controls which animation plays
    private Vector3 startPosition;       // Where the enemy was placed in the scene
    private float waitTimer;             // Countdown timer for pausing at patrol points

    void Start()
    {
        // Grab the NavMeshAgent component attached to this GameObject
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Disable NavMeshAgent's snap-turning so we can rotate smoothly ourselves
        agent.updateRotation = false;

        // Remember where we started (patrol points will be near here)
        startPosition = transform.position;

        // Pick the first patrol destination immediately
        GoToRandomPatrolPoint();
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // --- Detection check (same as Step 1) ---
        if (distanceToPlayer <= detectionRadius)
        {
            if (currentState != EnemyState.Chase)
            {
                currentState = EnemyState.Chase;
                Debug.Log("🚨 Player detected!");
            }
        }
        else
        {
            if (currentState != EnemyState.Idle)
            {
                currentState = EnemyState.Idle;
                Debug.Log("😴 Player lost. Resuming patrol.");
                GoToRandomPatrolPoint();  // Pick a new patrol point when we lose the player
            }
        }

        // --- Act based on current state ---
        if (currentState == EnemyState.Idle)
        {
            Patrol();
        }
        // (Chase behavior coming in Step 3!)

        // --- Smoothly rotate toward movement direction ---
        HandleRotation();

        // --- Tell the Animator how fast we're moving ---
        // agent.velocity.magnitude = current speed in meters/sec
        // Animator sees this and picks Idle (0) or Walking (>0.1)
        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    // --- Smoothly rotate the enemy to face where it is walking ---
    private void HandleRotation()
    {
        Vector3 moveDirection = agent.velocity;
        moveDirection.y = 0f; // Stay upright on the floor (don't tilt up or down)

        // Only turn if the enemy is actually moving
        if (moveDirection.sqrMagnitude > 0.05f)
        {
            // Calculate the rotation needed to face movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection.normalized);

            // Slerp = spherical smooth transition from current angle to target angle
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    // --- PATROL: Walk to random points, pause, pick a new one ---
    private void Patrol()
    {
        // Has the enemy arrived at its destination?
        // remainingDistance = how many meters left to walk
        // stoppingDistance = how close is "close enough" (set on the NavMeshAgent component)
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // Wait at this spot for a few seconds before moving again
            waitTimer += Time.deltaTime;
            if (waitTimer >= patrolWaitTime)
            {
                GoToRandomPatrolPoint();
                waitTimer = 0f;
            }
        }
    }

    // --- Pick a random walkable point and go there ---
    private void GoToRandomPatrolPoint()
    {
        // 1. Pick a random direction and distance (like throwing a dart)
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;

        // 2. Offset it from our START position (not current position)
        //    This keeps the enemy patrolling near where you placed it
        randomDirection += startPosition;

        // 3. Find the nearest valid point ON the NavMesh
        //    (the random point might be inside a wall — this snaps it to walkable ground)
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    // --- Visualize detection radius in Editor ---
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Also show patrol radius (green)
        Gizmos.color = Color.green;
        Vector3 center = Application.isPlaying ? startPosition : transform.position;
        Gizmos.DrawWireSphere(center, patrolRadius);
    }
}
