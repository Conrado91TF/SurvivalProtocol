using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controlador principal de la Máquina de Estados del Enemigo.
/// Gestiona las transiciones entre Patrulla, Persecución y Ataque.
/// Requiere: NavMeshAgent, AudioSource en el mismo GameObject.
/// </summary>

public class EnemyFSM : MonoBehaviour
{
   
    public enum EnemyState { Patrol, Chase, Attack }

    [Header("Estado Actual (solo lectura)")]
    [SerializeField] private EnemyState currentState = EnemyState.Patrol;

    
    [Header("Referencias")]
    public Transform player;               

    [Header("Detección")]
    [Tooltip("Radio en el que el enemigo puede ver al jugador")]
    public float visionRange = 15f;

    [Tooltip("Ángulo del cono de visión (grados totales)")]
    public float visionAngle = 120f;

    [Tooltip("Capas que bloquean la visión (paredes, obstáculos)")]
    public LayerMask obstacleMask;

    [Tooltip("Segundos sin ver al jugador para volver a Patrulla")]
    public float losePlayerTime = 3f;

   
    [Header("Distancias")]
    public float attackRange = 5f;

    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public AudioSource audioSource;

    // Subestados
    private PatrolState patrolState;
    private ChaseState chaseState;
    private AttackState attackState;

    // Timer para perder al jugador
    [HideInInspector] public float losePlayerTimer;
    [HideInInspector] public Animator animator;


    private void Awake()
    {
        
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        patrolState = GetComponent<PatrolState>();
        chaseState = GetComponent<ChaseState>();
        attackState = GetComponent<AttackState>();

        if (patrolState == null) Debug.LogError("[EnemyFSM] Falta el componente PatrolState.");
        if (chaseState == null) Debug.LogError("[EnemyFSM] Falta el componente ChaseState.");
        if (attackState == null) Debug.LogError("[EnemyFSM] Falta el componente AttackState.");
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag("Player");
            if (go) player = go.transform;
            else Debug.LogWarning("[EnemyFSM] No se encontró el jugador con tag 'Player'.");
        }

        EnterState(currentState);
    }

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrol: patrolState.OnUpdate(); break;
            case EnemyState.Chase: chaseState.OnUpdate(); break;
            case EnemyState.Attack: attackState.OnUpdate(); break;
        }
        
    }

   
    // ?????????????????????????????????????????????
    //  Gestión de transiciones
    // ?????????????????????????????????????????????
    public void TransitionTo(EnemyState newState)
    {
        if (newState == currentState) return;

        ExitState(currentState);
        currentState = newState;
        EnterState(currentState);
    }

    private void EnterState(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Patrol: patrolState.OnEnter(); break;
            case EnemyState.Chase: chaseState.OnEnter(); break;
            case EnemyState.Attack: attackState.OnEnter(); break;
        }
    }

    private void ExitState(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Patrol: patrolState.OnExit(); break;
            case EnemyState.Chase: chaseState.OnExit(); break;
            case EnemyState.Attack: attackState.OnExit(); break;
        }
    }

    // ?????????????????????????????????????????????
    //  Utilidades de detección (compartidas)
    // ?????????????????????????????????????????????

    /// <summary>
    /// Comprueba si el jugador está dentro del rango y ángulo de visión,
    /// y además hay línea de visión directa (sin obstáculos).
    /// </summary>
    public bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 dirToPlayer = player.position - transform.position;
        float distance = dirToPlayer.magnitude;

        // 1. Distancia
        if (distance > visionRange) return false;

        // 2. Ángulo
        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > visionAngle * 0.5f) return false;

        // 3. Raycast: sin obstáculos entre enemigo y jugador
        Vector3 origin = transform.position + Vector3.up * 1.5f; // altura ojos
        Vector3 target = player.position + Vector3.up * 1.0f;

        if (Physics.Raycast(origin, (target - origin).normalized, distance, obstacleMask))
            return false; // hay un obstáculo

        return true;
    }

    /// <summary>
    /// Distancia al jugador.
    /// </summary>
    public float DistanceToPlayer()
    {
        if (player == null) return Mathf.Infinity;
        return Vector3.Distance(transform.position, player.position);
    }

    // ?????????????????????????????????????????????
    //  Gizmos (debug visual en Scene)
    // ?????????????????????????????????????????????
    private void OnDrawGizmosSelected()
    {
        // Rango de visión
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        // Rango de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Cono de visión
        Vector3 leftBound = Quaternion.Euler(0, -visionAngle * 0.5f, 0) * transform.forward * visionRange;
        Vector3 rightBound = Quaternion.Euler(0, visionAngle * 0.5f, 0) * transform.forward * visionRange;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + leftBound);
        Gizmos.DrawLine(transform.position, transform.position + rightBound);
    }
}