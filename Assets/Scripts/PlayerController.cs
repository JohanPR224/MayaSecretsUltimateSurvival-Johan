using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    [Header("Salud")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private Slider healthSlider; // Opcional: arrastra tu barra de vida si tienes una

    [Header("Disparo / Ataque")]
    [SerializeField] private GameObject projectilePrefab; // Prefab de la lanza
    [SerializeField] private Transform firePoint;          // Punto vacío en la mano del personaje
    [SerializeField] private float fireRate = 0.4f;        // Cadencia entre disparos
    [SerializeField] private float projectileSpeed = 14f;  // Velocidad de la lanza
    private float nextFireTime = 0f;

    [Header("Game Over & Estado")]
    [SerializeField] private GameOverManager gameOverManager;
    private bool isDead = false;

    [Header("Visuales")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        // Si no se asignó en el Inspector, busca el Canvas con el GameOverManager solo
        if (gameOverManager == null)
        {
            gameOverManager = FindFirstObjectByType<GameOverManager>();
        }
    }

    void Update()
    {
        if (isDead) return;

        // 1. Entrada de movimiento (WASD / Flechas)
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        // Voltear sprite según la dirección horizontal
        if (spriteRenderer != null)
        {
            if (moveInput.x > 0.01f) spriteRenderer.flipX = false;
            else if (moveInput.x < -0.01f) spriteRenderer.flipX = true;
        }

        // 2. Disparo con Clic Izquierdo del mouse o barra espaciadora
        if ((Input.GetButton("Fire1") || Input.GetKey(KeyCode.Space)) && Time.time >= nextFireTime)
        {
            Disparar();
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

        // Movimiento por físicas en 2D
        rb.linearVelocity = moveInput * moveSpeed;
    }

    private void Disparar()
    {
        if (projectilePrefab == null) return;

        // Si no tienes firePoint asignado, dispara desde el centro del jugador
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;

        // Calcular dirección hacia el puntero del mouse
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        Vector2 direction = ((Vector2)mouseWorldPos - (Vector2)spawnPos).normalized;

        if (direction == Vector2.zero) direction = Vector2.right;

        // Rotar proyectil hacia la dirección del mouse
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.Euler(0, 0, angle));

        // Aplicar velocidad al proyectil
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

        if (currentHealth <= 0f)
        {
            Morir();
        }
    }

    private void Morir()
    {
        if (isDead) return;
        isDead = true;

        currentHealth = 0f;
        rb.linearVelocity = Vector2.zero;

        // Activa la pantalla de muerte y congela la partida
        if (gameOverManager != null)
        {
            gameOverManager.ActivarGameOver();
        }
        else
        {
            Debug.LogError("PlayerController: No se encontró el GameOverManager en la escena.");
        }
    }

    // Detección de daño por contacto con enemigos o proyectiles hostiles
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