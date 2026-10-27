using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Estadísticas Base del Enemigo")]
    public float maxHealth = 30f;
    public float currentHealth;
    public float damage = 10f;

    [Header("Efectos Visuales")]
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;
    [Header("Recompensas")]
    [SerializeField] private GameObject expDropPrefab;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine flashCoroutine;
    private bool isDead = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        if (currentHealth <= 0f)
        {
            currentHealth = maxHealth;
        }
    }

    // Configura la vida y daño escalados desde EnemySpawner
    public void ConfigurarEstadisticas(int numeroOleada, float factorVida, float factorDanio)
    {
        int nivelExtra = Mathf.Max(0, numeroOleada - 1);

        maxHealth = Mathf.Round(maxHealth * (1f + nivelExtra * factorVida));
        currentHealth = maxHealth;

        damage = Mathf.Round(damage * (1f + nivelExtra * factorDanio));
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (spriteRenderer != null)
        {
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(DamageFlash());
        }

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private IEnumerator DamageFlash()
    {
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
        flashCoroutine = null;
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        // Soltar gema de experiencia en la posición donde murió el Alux
        if (expDropPrefab != null)
        {
            Instantiate(expDropPrefab, transform.position, Quaternion.identity);
        }

        // Notificar a WaveManager
        WaveManager waveManager = FindFirstObjectByType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.OnEnemyKilled();
        }

        Destroy(gameObject);
    }
}