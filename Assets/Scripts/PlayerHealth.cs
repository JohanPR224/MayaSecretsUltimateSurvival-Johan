using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Referencias de UI")]
    [SerializeField] private Slider healthSlider; // Opcional: arrastra tu barra de vida si tienes una
    [SerializeField] private GameOverManager gameOverManager;

    [Header("Efectos al recibir daño (Invulnerabilidad)")]
    [SerializeField] private float invulnerabilityDuration = 0.5f;
    private float invulnerabilityTimer = 0f;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        // Si no se asignó en el Inspector, intenta buscarlo automáticamente en la escena
        if (gameOverManager == null)
        {
            gameOverManager = FindFirstObjectByType<GameOverManager>();
        }
    }

    void Update()
    {
        if (invulnerabilityTimer > 0f)
        {
            invulnerabilityTimer -= Time.unscaledDeltaTime;
        }
    }

    public void TakeDamage(float damage)
    {
        // Evita daño si ya murió o si está en tiempo de invulnerabilidad
        if (isDead || invulnerabilityTimer > 0f) return;

        currentHealth -= damage;
        invulnerabilityTimer = invulnerabilityDuration;

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        currentHealth = 0f;

        // Dispara la pantalla de muerte y congela el juego
        if (gameOverManager != null)
        {
            gameOverManager.ActivarGameOver();
        }
        else
        {
            Debug.LogError("PlayerHealth: No se encontró el GameOverManager en la escena.");
        }
    }

    // Compatibilidad en caso de que tus enemigos llamen al daño por Trigger o Collision
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            TakeDamage(20f); // Daño estándar por defecto si el script del enemigo no llama TakeDamage directamente
        }
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}