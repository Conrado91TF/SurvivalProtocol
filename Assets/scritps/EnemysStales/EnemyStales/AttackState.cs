using UnityEngine;

public class AttackState : MonoBehaviour
{
    [Header("Disparo")]
    [Tooltip("Prefab del proyectil")]
    public GameObject projectilePrefab;

    [Tooltip("Punto desde el que se instancian los proyectiles (cañón del arma)")]
    public Transform firePoint;

    [Tooltip("Cadencia de disparo: segundos entre cada bala")]
    public float fireRate = 2f;

    [Header("Efectos")]
    [Tooltip("Sistema de partículas del disparo (muzzle flash)")]
    public ParticleSystem muzzleFlash;

    [Tooltip("Clip de audio del disparo")]
    public AudioClip fireSound;

    [Tooltip("Volumen del sonido de disparo (0-1)")]
    [Range(0f, 1f)]
    public float fireSoundVolume = 0.8f;

    [Header("Rotación")]
    [Tooltip("Velocidad de giro al encarar al jugador (grados/seg)")]
    public float rotationSpeed = 8f;

    // ─────────────────────────────────────────────
    private EnemyFSM fsm;
    private float fireTimer;

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
        fsm.agent.isStopped = true;   // detener movimiento
        fsm.agent.velocity = Vector3.zero;
        fireTimer = fireRate; // disparo inmediato al entrar
    }

    public void OnUpdate()
    {
        if (fsm.player == null) return;

        // ─── Transición de vuelta a Persecución ───
        if (fsm.DistanceToPlayer() > fsm.attackRange)
        {
            fsm.TransitionTo(EnemyFSM.EnemyState.Chase);
            return;
        }

        // ─── LookAt (solo eje Y) ───
        FacePlayer();

        // ─── Cadencia de disparo ───
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            fireTimer = fireRate;
            Shoot();
        }
    }

    public void OnExit()
    {
        fsm.agent.isStopped = false;
    }

    // ─────────────────────────────────────────────
    //  Helpers
    // ─────────────────────────────────────────────

    /// <summary>
    /// Rota suavemente al enemigo hacia el jugador (solo en el eje horizontal).
    /// </summary>
    private void FacePlayer()
    {
        Vector3 direction = (fsm.player.position - transform.position);
        direction.y = 0f;

        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                                               rotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Instancia el proyectil, reproduce partícula y sonido.
    /// </summary>
    private void Shoot()
    {
        if (firePoint == null)
        {
            Debug.LogWarning("[AttackState] firePoint no asignado. Usando posición del enemigo.");
        }

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position + Vector3.up * 1.5f;
        Quaternion spawnRot = firePoint != null ? firePoint.rotation : transform.rotation;

        // Instanciar proyectil
        if (projectilePrefab != null)
            Instantiate(projectilePrefab, spawnPos, spawnRot);
        else
            Debug.LogWarning("[AttackState] projectilePrefab no asignado.");

        // Partícula (muzzle flash)
        if (muzzleFlash != null)
            muzzleFlash.Play();

        // Sonido
        if (fireSound != null)
            fsm.audioSource.PlayOneShot(fireSound, fireSoundVolume);
    }
}

