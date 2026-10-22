using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    [Header("Salud y Barra de Vida")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private Slider healthSlider;

    [Header("Indicador de Daño")]
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float flashDuration = 0.15f;
    private Color originalColor;
    private Coroutine flashCoroutine;

    [Header("Ataque con Lanza")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.4f;
    [SerializeField] private float projectileSpeed = 14f;
    private float nextFireTime = 0f;

    [Header("Game Over")]
    [SerializeField] private GameOverManager gameOverManager;
    private bool isDead = false;

    private SpriteRenderer spriteRenderer;
    private Camera mainCam;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
        mainCam = Camera.main;

        // Configuración inicial de la barra de vida en base a 100
        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
            
            if (healthSlider.fillRect != null)
            {
                healthSlider.fillRect.gameObject.SetActive(true);
            }
        }

        if (gameOverManager == null)
        {
            gameOverManager = FindAnyObjectByType<GameOverManager>();
        }
    }

    void Update()
    {
        if (isDead) return;

        // Lectura de controles de movimiento
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        // Volteo horizontal del Cazador según dirección
        if (spriteRenderer != null)
        {
            if (moveInput.x > 0.01f) spriteRenderer.flipX = false;
            else if (moveInput.x < -0.01f) spriteRenderer.flipX = true;
        }

        // Ataque de la lanza
        if ((Input.GetButton("Fire1") || Input.GetKey(KeyCode.Space)) && Time.time >= nextFireTime)
        {
            DispararLanza();
            nextFireTime = Time.time + fireRate;
        }
    }

    void FixedUpdate()
    {
        if (isDead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = moveInput * moveSpeed;
    }

    private void DispararLanza()
    {
        if (projectilePrefab == null) return;
        if (mainCam == null) mainCam = Camera.main;
        if (mainCam == null) return;

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector2 direction = ((Vector2)mouseWorldPos - (Vector2)spawnPos).normalized;
        if (direction == Vector2.zero) direction = Vector2.right;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.Euler(0, 0, angle));

        Rigidbody2D projRb = projectile.GetComponent<Rigidbody2D>();
        if (projRb != null)
        {
            projRb.linearVelocity = direction * projectileSpeed;
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (currentHealth < 0f) currentHealth = 0f;

        // Actualiza el slider proporcionalmente al daño
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;

            // Apaga el relleno verde si llega a 0 para que no quede ningún sobrante
            if (healthSlider.fillRect != null)
            {
                healthSlider.fillRect.gameObject.SetActive(currentHealth > 0f);
            }
        }

        // Parpadeo rojo visual al recibir golpe
        if (spriteRenderer != null)
        {
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(DamageFlash());
        }

        if (currentHealth <= 0f)
        {
            Morir();
        }
    }

    private IEnumerator DamageFlash()
    {
        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
        flashCoroutine = null;
    }

    private void Morir()
    {
        if (isDead) return;
        isDead = true;

        rb.linearVelocity = Vector2.zero;
        if (spriteRenderer != null) spriteRenderer.color = originalColor;

        if (gameOverManager != null)
        {
            gameOverManager.ActivarGameOver();
        }
    }

    // Colisión física sólida
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                TakeDamage(enemy.damage); // Resta el valor exacto del enemigo
            }
            else
            {
                TakeDamage(5f);
            }
        }
    }

    // Colisión en caso de usar IsTrigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("EnemyAttack"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                TakeDamage(enemy.damage); // Resta el valor exacto del enemigo
            }
            else
            {
                TakeDamage(5f);
            }
        }
    }
}