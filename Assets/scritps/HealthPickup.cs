
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Vida")]
    [Tooltip("Vida que restaura al recogerla")]
    public float healAmount = 50f;

    [Header("Efectos")]
    public AudioClip pickupSound;
    public GameObject pickupEffect;

    [Header("Rotación")]
    public float rotationSpeed = 90f;

    private void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime); // Rotar alrededor del eje Y a velocidad constante
    }

    private void OnTriggerEnter(Collider other)
    {    // Buscar PlayerHealth en el objeto y en todos sus hijos
        PlayerHealth player = other.GetComponentInChildren<PlayerHealth>();
        if (player == null)
            player = other.GetComponentInParent<PlayerHealth>();
        // Si el jugador no tiene un componente PlayerHealth, no hacemos nada
        if (player == null) return;
        if (player.GetCurrentHealth() >= player.GetMaxHealth()) return;

        player.Heal(healAmount);

        if (pickupEffect != null) // Si hay un efecto de recogida asignado, lo instanciamos en la posición del pickup
            Instantiate(pickupEffect, transform.position, Quaternion.identity);

        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        Destroy(gameObject);
    }
}