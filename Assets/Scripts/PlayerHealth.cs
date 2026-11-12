using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Referencias de UI")]
    [SerializeField] private Slider healthSlider; 
    [SerializeField] private Image hpFillImage;   
    [SerializeField] private GameOverManager gameOverManager;

    [Header("Efectos al recibir daño (Invulnerabilidad)")]
    [SerializeField] private float invulnerabilityDuration = 0.5f;
    private float invulnerabilityTimer = 0f;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        // BÚSQUEDA AUTOMÁTICA: Si la barra verde no se asignó en el Inspector, la encuentra por su nombre
        if (hpFillImage == null)
        {
            GameObject barraUI = GameObject.Find("HP_Fill");
            if (barraUI != null)
            {
                hpFillImage = barraUI.GetComponent<Image>();
            }
        }

        ActualizarInterfazUI();

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

        // Suma la vida sin pasarse del máximo
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        
        // Fuerza la actualización de la UI en este exacto instante
        ActualizarInterfazUI();

        // Notifica al PlayerController por si él también intenta controlar la barra
        SendMessage("UpdateUI", SendMessageOptions.DontRequireReceiver);
    }

    public void ActualizarInterfazUI()
    {
        // Seguro de vida: si la imagen se desvinculó en plena ejecución, la vuelve a buscar
        if (hpFillImage == null)
        {
            GameObject barraUI = GameObject.Find("HP_Fill");
            if (barraUI != null) hpFillImage = barraUI.GetComponent<Image>();
        }

        // Actualiza el relleno verde directamente
        if (hpFillImage != null)
        {
            hpFillImage.fillAmount = currentHealth / maxHealth;
        }

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

        if (gameOverManager != null)
        {
            gameOverManager.ActivarGameOver();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ProcesarContacto(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ProcesarContacto(collision.gameObject);
    }

    private void ProcesarContacto(GameObject obj)
    {
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
