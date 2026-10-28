using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Referencias de UI")]
    [SerializeField] private Slider healthSlider; // Opcional
    [SerializeField] private Image hpFillImage;   // Referencia a HP_Fill en el Canvas
    [SerializeField] private GameOverManager gameOverManager;

    [Header("Efectos al recibir daño (Invulnerabilidad)")]
    [SerializeField] private float invulnerabilityDuration = 0.5f;
    private float invulnerabilityTimer = 0f;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        // Inicializar UI al arrancar
        ActualizarInterfazUI();

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

        if (CameraFollow.Instance != null)
        {
            CameraFollow.Instance.TriggerShake(0.2f, 0.4f);
        }

        ActualizarInterfazUI();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        ActualizarInterfazUI();
    }

    private void ActualizarInterfazUI()
    {
        // 1. Actualizar la barra roja del marco de piedra
        if (hpFillImage != null)
        {
            hpFillImage.fillAmount = currentHealth / maxHealth;
        }

        // 2. Actualizar Slider si sigue en uso
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        currentHealth = 0f;
        ActualizarInterfazUI();

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

    // Detecta si el enemigo tiene Is Trigger activado
    private void OnTriggerEnter2D(Collider2D other)
    {
        ProcesarContacto(other.gameObject);
    }

    // Detecta si el enemigo choca con física sólida (sin Is Trigger)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        ProcesarContacto(collision.gameObject);
    }

    private void ProcesarContacto(GameObject obj)
    {
        // Si el objeto tiene el script Enemy o el tag Enemy
        Enemy enemy = obj.GetComponent<Enemy>();
        if (enemy != null || obj.CompareTag("Enemy"))
        {
            float dmg = enemy != null ? enemy.damage : 10f;
            TakeDamage(dmg);
        }
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}