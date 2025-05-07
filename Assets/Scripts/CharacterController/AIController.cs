using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIController : MonoBehaviour
{
    private AnimationController animationController;
    private NavMeshAgent agent;
    public float repathInterval = 5f;
    private float repathTimer;

    [Range(0, 100)] public float PathFindingRadius;

    [Header("Movement Settings")]
    public float acceleration = 8f;
    public float deceleration = 6f;
    public float smoothing = 5f; // For animation smoothing

    private Vector2 currentAnimBlend = Vector2.zero;

    private void OnEnable()
    {
        animationController = gameObject.AddComponent<AnimationController>();
        animationController.animator = GetComponent<Animator>();

        agent = GetComponent<NavMeshAgent>();
        agent.acceleration = acceleration;
        agent.angularSpeed = 120f; // Optional: tweak based on turning needs
    }

    void Update()
    {
        if (!agent.isOnNavMesh || !agent.isActiveAndEnabled) return;

        repathTimer -= Time.deltaTime;
        if (repathTimer <= 0f || agent.remainingDistance < 0.5f)
        {
            PathFinding();
            repathTimer = repathInterval;
        }

        SetAnimation();
    }

    private void SetAnimation()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity);

        if (agent.remainingDistance < 0.5f || agent.velocity.magnitude < 0.1f)
        {
            currentAnimBlend = Vector2.Lerp(currentAnimBlend, Vector2.zero, Time.deltaTime * deceleration);
            animationController.UpdateLocomotionBlend(currentAnimBlend.x, currentAnimBlend.y);
            return;
        }

        float targetZ = Mathf.Clamp(localVelocity.z, -2f, 2f);
        float targetX = Mathf.Clamp(localVelocity.x, -0.5f, 0.5f);

        Vector2 targetBlend = new Vector2(targetX, targetZ);
        currentAnimBlend = Vector2.Lerp(currentAnimBlend, targetBlend, Time.deltaTime * smoothing);

        animationController.UpdateLocomotionBlend(currentAnimBlend.x, currentAnimBlend.y);
    }
private void PathFinding()
{
    if (!agent.isOnNavMesh) return;

    const int maxAttempts = 10;
    const float minDistance = 20f; // Minimum distance from current position

    for (int attempt = 0; attempt < maxAttempts; attempt++)
    {
        Vector3 randomDirection = Random.insideUnitSphere * PathFindingRadius;
        randomDirection.y = 0f; // Keep it on the same plane
        Vector3 candidatePosition = transform.position + randomDirection;

        if (NavMesh.SamplePosition(candidatePosition, out NavMeshHit hit, PathFindingRadius, NavMesh.AllAreas))
        {
            float distance = Vector3.Distance(transform.position, hit.position);
            if (distance >= minDistance)
            {
                agent.SetDestination(hit.position);
                return;
            }
        }
    }

    // Optional: fallback to idle or nearest valid point
    agent.SetDestination(transform.position);
}


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, PathFindingRadius);

        if (agent != null && agent.hasPath)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < agent.path.corners.Length - 1; i++)
            {
                Gizmos.DrawLine(agent.path.corners[i], agent.path.corners[i + 1]);
            }

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(agent.destination, 0.3f);
        }
    }
}
