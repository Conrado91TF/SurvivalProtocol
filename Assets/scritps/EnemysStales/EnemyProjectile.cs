using UnityEngine;

/// <summary>
/// Proyectil disparado por el enemigo.
/// Se mueve en línea recta y causa daño al jugador al colisionar.
/// Requiere un Collider con "Is Trigger" activado (o sin Trigger para física).
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class EnemyProjectile : MonoBehaviour
{
    [Header("Proyectil")]
    [Tooltip("Velocidad del proyectil (unidades/seg)")]
    public float speed = 20f;

    [Tooltip("Daño que inflige al jugador")]
    public float damage = 10f;

    [Tooltip("Segundos antes de autodestruirse si no golpea nada")]
    public float lifetime = 5f;

    [Header("Impacto")]
    [Tooltip("Prefab de efecto de impacto (partículas de explosión, etc.)")]
    public GameObject impactEffectPrefab;

    [Tooltip("Tag del objeto que recibirá el daño")]
    public string playerTag = "Player";

    // ─────────────────────────────────────────────
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // proyectil recto (sin gravedad)
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    private void Start()
    {
        rb.linearVelocity = transform.forward * speed;
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ─── Efecto de impacto ───
        if (impactEffectPrefab != null)
        {
            ContactPoint contact = collision.contacts[0];
            Instantiate(impactEffectPrefab, contact.point,
                        Quaternion.LookRotation(contact.normal));
        }

        // ─── Daño al jugador ───
        if (collision.gameObject.CompareTag(playerTag))
        {
            // Intenta llamar a un método TakeDamage en el jugador.
            // Adapta esto a tu propio sistema de vida.
            EnemyHealth eh = collision.gameObject.GetComponent<EnemyHealth>();
            if (eh != null) eh.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
