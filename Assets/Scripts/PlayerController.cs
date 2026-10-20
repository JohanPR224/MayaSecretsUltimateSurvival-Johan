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

    [Header("Salud y UI")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private Slider healthSlider; // Arrastra tu Slider aquí

    [Header("Feedback de Daño (Flash)")]
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

    [Header("Integración Game Over")]
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
        if (mainCam == null)
        {
            mainCam = FindFirstObjectByType<Camera>();
        }

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (gameOverManager == null)
        {
            gameOverManager = FindFirstObjectByType<GameOverManager>();
        }
    }

    void Update()
    {
        if (isDead) return;

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        if (spriteRenderer != null)
        {
            if (moveInput.x > 0.01f) spriteRenderer.flipX = false;
            else if (moveInput.x < -0.01f) spriteRenderer.flipX = true;
        }

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

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;

        if (mainCam == null) mainCam = Camera.main;
        if (mainCam == null) return;

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

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        // Indicador visual de daño (parpadeo rojo)
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

        currentHealth = 0f;
        rb.linearVelocity = Vector2.zero;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        if (gameOverManager == null)
        {
            gameOverManager = FindFirstObjectByType<GameOverManager>();
        }

        if (gameOverManager != null)
        {
            gameOverManager.ActivarGameOver();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(15f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("EnemyAttack"))
        {
            TakeDamage(15f);
        }
    }
}