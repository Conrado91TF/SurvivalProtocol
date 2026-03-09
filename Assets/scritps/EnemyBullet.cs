using UnityEngine;



public class EnemyBullet : MonoBehaviour
{
    [SerializeField] float speed = 15f;
    [SerializeField] float damage = 10f;
    [SerializeField] float timeToDestroy = 5f;

    Rigidbody rb;
    float timer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        timer = 0f;

        if (rb != null)
        {
            rb.useGravity = false;

        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timeToDestroy)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
{
    Debug.Log("ENEMYBULLET trigger con: " + other.gameObject.name);

    PlayerHealth player = other.gameObject.GetComponentInParent<PlayerHealth>();
    if (player != null)
    {
        Debug.Log("ENCONTRÓ PlayerHealth, quitando vida");
        player.TakeDamage(damage);
    }

        Destroy(gameObject);
    }
}