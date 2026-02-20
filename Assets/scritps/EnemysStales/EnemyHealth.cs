using UnityEngine;
using UnityEngine.AI;
public class EnemyHealth : MonoBehaviour
{
    [Header("Vida")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Muerte")]
    [Tooltip("Segundos antes de destruir el GameObject tras morir")]
    public float destroyDelay = 3f;

    [Tooltip("Prefab de efecto al morir (sangre, explosión, etc.)")]
    public GameObject deathEffectPrefab;

    [Header("Ragdoll (opcional)")]
    [Tooltip("Activa esto si usas ragdoll en lugar de animación de muerte")]
    public bool useRagdoll = false;

    // Referencias internas
    private Animator animator;
    private EnemyFSM fsm;
    private NavMeshAgent agent;
    private bool isDead = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        fsm = GetComponent<EnemyFSM>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    // ── Llamado externamente (por bala, explosión, etc.) ──
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0f);

        if (currentHealth <= 0f)
            Die();
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // 1. Detener la IA completamente
        if (fsm != null) fsm.enabled = false;
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        // 2. Desactivar el collider para que no siga recibiendo impactos
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // 3. Reproducir animación de muerte o activar ragdoll
        if (useRagdoll)
            ActivateRagdoll();
        else if (animator != null)
            animator.SetBool("isDead", true);

        // 4. Efecto visual de muerte
        if (deathEffectPrefab != null)
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

        // 5. Destruir el GameObject tras un delay
        Destroy(gameObject, destroyDelay);
    }

    private void ActivateRagdoll()
    {
        // Desactivar el Animator para que el ragdoll tome el control
        if (animator != null) animator.enabled = false;

        // Activar todos los Rigidbody hijos (huesos del ragdoll)
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = false;
        }

        // Activar todos los Collider hijos
        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            col.enabled = true;
        }
    }

    // ── Barra de vida en pantalla (opcional Gizmo) ──
    private void OnGUI()
    {
        // Solo visible en el Editor para debug
#if UNITY_EDITOR
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2.5f);
        if (screenPos.z > 0)
        {
            float barWidth = 60f;
            float barHeight = 8f;
            float healthPct = currentHealth / maxHealth;

            GUI.color = Color.black;
            GUI.DrawTexture(new Rect(screenPos.x - barWidth / 2 - 1, Screen.height - screenPos.y - 1,
                                     barWidth + 2, barHeight + 2), Texture2D.whiteTexture);
            GUI.color = Color.red;
            GUI.DrawTexture(new Rect(screenPos.x - barWidth / 2, Screen.height - screenPos.y,
                                     barWidth, barHeight), Texture2D.whiteTexture);
            GUI.color = Color.green;
            GUI.DrawTexture(new Rect(screenPos.x - barWidth / 2, Screen.height - screenPos.y,
                                     barWidth * healthPct, barHeight), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }
#endif
    }
}