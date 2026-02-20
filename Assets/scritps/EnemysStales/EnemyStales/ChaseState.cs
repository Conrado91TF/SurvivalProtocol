using UnityEngine;
public class ChaseState : MonoBehaviour
{
    [Header("Persecución")]
    [Tooltip("Velocidad al perseguir al jugador")]
    public float chaseSpeed = 5.5f;

    [Tooltip("Con qué frecuencia (segundos) se actualiza el destino del NavMesh")]
    public float pathUpdateInterval = 0.2f;

    // ─────────────────────────────────────────────
    private EnemyFSM fsm;
    private float pathUpdateTimer;

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
        fsm.agent.speed = chaseSpeed;
        fsm.agent.isStopped = false;
        fsm.losePlayerTimer = 0f;
        pathUpdateTimer = 0f;

        fsm.animator.SetBool("isWalking", true);
        fsm.animator.SetBool("isAiming", false);
    }

    public void OnUpdate()
    {
        float distToPlayer = fsm.DistanceToPlayer();

        // ─── Transición a Ataque ───
        if (distToPlayer <= fsm.attackRange)
        {
            fsm.TransitionTo(EnemyFSM.EnemyState.Attack);
            return;
        }

        // ─── ¿Sigue viendo al jugador? ───
        if (fsm.CanSeePlayer())
        {
            fsm.losePlayerTimer = 0f; // resetear timer

            // Actualizar destino periódicamente
            pathUpdateTimer -= Time.deltaTime;
            if (pathUpdateTimer <= 0f)
            {
                pathUpdateTimer = pathUpdateInterval;
                fsm.agent.destination = fsm.player.position;
            }
        }
        else
        {
            // Acumular tiempo sin ver al jugador
            fsm.losePlayerTimer += Time.deltaTime;

            if (fsm.losePlayerTimer >= fsm.losePlayerTime)
            {
                fsm.TransitionTo(EnemyFSM.EnemyState.Patrol);
                return;
            }

            // Seguir yendo a la última posición conocida
            // (el destino ya está fijado, no se actualiza)
        }
    }

    public void OnExit()
    {
        fsm.losePlayerTimer = 0f;
    }
}

