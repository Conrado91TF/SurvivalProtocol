using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float timeToDestroy = 5f;
    float timer;

    // Opcional: cachear Rigidbody si tu bala usa f�sica
    Rigidbody rb;
    private bool hasHit = false;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        // Reset del temporizador cada vez que se activa (pooling)
        timer = 0f;
        hasHit = false;

        // Reset f�sico para evitar que conserve velocidad previa
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void Update()
    {
        // Corregido: sumar deltaTime (antes era *= y era un bug)
        timer += Time.deltaTime;
        if (timer >= timeToDestroy)
        {
            // Devolver al pool; fallback a Destroy si no hay PoolManager
            if (PoolManager.Instance != null)
                PoolManager.Instance.ReturnToPool(this.gameObject);
            else
                Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
       if (hasHit) return;
        hasHit = true;

        Debug.Log("BULLET golpeó: " + collision.gameObject.name +
              " | Layer: " + collision.gameObject.layer);

        EnemyHealth enemy = collision.gameObject.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(33.3f);
        }

        else
        {
            Debug.Log("NO encontró EnemyHealth en: " + collision.gameObject.name);
        }


        if (PoolManager.Instance != null)
            PoolManager.Instance.ReturnToPool(this.gameObject);
        else
            Destroy(this.gameObject);
    }
}
