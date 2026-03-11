
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [Header("Munición")]
    [Tooltip("Munición que añade al recogerla")]
    public int ammoAmount = 16;

    [Header("Efectos")]
    public AudioClip pickupSound;
    public GameObject pickupEffect;

    [Header("Rotación")]
    public float rotationSpeed = 90f;

    private void Update()
    {
        // Rotar la caja para que llame la atención
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Buscar WeaponManager en el objeto y en todos sus hijos
        WeaponManager weapon = other.GetComponentInChildren<WeaponManager>();

        // Si no lo encuentra, buscar en el padre
        if (weapon == null)
            weapon = other.GetComponentInParent<WeaponManager>();

        if (weapon == null) return;

        weapon.AddAmmo(ammoAmount);

        if (pickupEffect != null)
            Instantiate(pickupEffect, transform.position, Quaternion.identity);

        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        Destroy(gameObject);
    }
}