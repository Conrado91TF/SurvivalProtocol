using UnityEngine;


public class PatrolState : MonoBehaviour
{
    [Header("Waypoints")]
    [Tooltip("Puntos de patrulla en orden. Se recorren en bucle.")]
    public Transform[] waypoints;

    [Tooltip("Velocidad de movimiento en patrulla")]
    public float patrolSpeed = 3f;

    [Tooltip("Distancia mínima para considerar que se llegó al waypoint")]
    public float waypointTolerance = 0.5f;

    [Tooltip("Tiempo de espera en cada waypoint (segundos)")]
    public float waitAtWaypoint = 1f;

    // ─────────────────────────────────────────────
    private EnemyFSM fsm;
    private int currentWaypointIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    // ─────────────────────────────────────────────
    private void Awake()
    {
        fsm = GetComponent<EnemyFSM>();
    }

    // ─────────────────────────────────────────────
    //  Ciclo del estado
    // ─────────────────────────────────────────────
    public void OnEnter()
    {
        fsm.agent.speed = patrolSpeed;
        fsm.agent.isStopped = false;
        isWaiting = false;

        if (waypoints != null && waypoints.Length > 0)
            SetDestinationToCurrentWaypoint();
        else
            Debug.LogWarning("[PatrolState] No hay waypoints asignados.");
    }

    public void OnUpdate()
    {
        // ─── Detección del jugador ───
        if (fsm.CanSeePlayer())
        {
            fsm.TransitionTo(EnemyFSM.EnemyState.Chase);
            return;
        }

        // ─── Navegación entre waypoints ───
        if (waypoints == null || waypoints.Length == 0) return;

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                AdvanceWaypoint();
                SetDestinationToCurrentWaypoint();
            }
            return;
        }

        // Comprobar si llegamos al waypoint
        if (!fsm.agent.pathPending && fsm.agent.remainingDistance <= waypointTolerance)
        {
            isWaiting = true;
            waitTimer = waitAtWaypoint;
            fsm.agent.isStopped = true;
        }
    }

    public void OnExit()
    {
        // Nada especial al salir
    }

    // ─────────────────────────────────────────────
    //  Helpers
    // ─────────────────────────────────────────────
    private void SetDestinationToCurrentWaypoint()
    {
        if (waypoints.Length == 0) return;
        fsm.agent.isStopped = false;
        fsm.agent.destination = waypoints[currentWaypointIndex].position;
    }

    private void AdvanceWaypoint()
    {
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
    }
}
