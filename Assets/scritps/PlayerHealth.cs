using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    public float maxHealth = 100f;
    private float currentHealth;
    private float targetFill; // hacia donde va la barra
    private bool isDead = false;

    [Header("UI")]
    public Image healthBarFill;

    [Header("Velocidad de la barra")]
    [Tooltip("Cuanto más alto más rápido baja la barra visualmente")]
    public float barSpeed = 3f;

    private void Start()
    {
        currentHealth = maxHealth;
        targetFill = 1f;
        if (healthBarFill != null)
            healthBarFill.fillAmount = 1f;
    }

    private void Update()
    {
        // La barra baja suavemente hacia el valor real de vida
        if (healthBarFill != null)
            healthBarFill.fillAmount = Mathf.Lerp(healthBarFill.fillAmount, targetFill, barSpeed * Time.deltaTime);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0f);

        // Actualizar el objetivo de la barra
        targetFill = currentHealth / maxHealth;

        if (currentHealth <= 0f)
            Die();
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        targetFill = currentHealth / maxHealth;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Jugador muerto");
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public bool IsDead() => isDead;
}