using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    [Header("Disparo / Lanzas")]
    [SerializeField] private GameObject lanzaPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.4f;
    private float nextFireTime = 0f;

    [Header("Estadísticas de Salud")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private Slider healthSlider;

    [Header("Efectos Visuales de Daño")]
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float flashDuration = 0.15f;
    [SerializeField] private float invulnerabilityDuration = 0.5f;

    [Header("Sacudida de Pantalla")]
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeMagnitude = 0.45f;

    private bool isDead = false;
    private float invulnerabilityTimer = 0f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine flashCoroutine;
    private GameOverManager gameOverManager;

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

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        gameOverManager = FindFirstObjectByType<GameOverManager>();
    }

    void Update()
    {
        if (isDead) return;

        // 1. Captura de teclas de movimiento (WASD / Flechas)
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        // 2. Volteo de sprite horizontal según dirección
        if (moveInput.x > 0.01f && spriteRenderer != null)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveInput.x < -0.01f && spriteRenderer != null)
        {
            spriteRenderer.flipX = true;
        }

        // 3. Disparo con Clic Izquierdo o Barra Espaciadora
        if ((Input.GetButton("Fire1") || Input.GetKey(KeyCode.Space)) && Time.time >= nextFireTime)
        {
            DispararLanza();
        }

        // 4. Temporizador de invulnerabilidad
        if (invulnerabilityTimer > 0f)
        {
            invulnerabilityTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;

        // Aplicación del movimiento sobre el Rigidbody2D
        if (rb != null)
        {
            rb.linearVelocity = moveInput * moveSpeed;
        }
    }

    private Vector2 lastFacingDirection = Vector2.right;

    // En Update(), actualiza hacia dónde mira el jugador:
    // (Añade esto justo debajo de donde lees moveInput):
    // if (moveInput.sqrMagnitude > 0.01f) lastFacingDirection = moveInput;

    private void DispararLanza()
    {
        nextFireTime = Time.time + fireRate;

        if (lanzaPrefab != null)
        {
            // Posición de salida de la lanza (FirePoint si existe, si no, el Player)
            Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;

            // 1. Obtener la posición del ratón en coordenadas del mundo 2D
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;

            // 2. Calcular la dirección desde el punto de disparo hacia el ratón
            Vector2 direccionDisparo = (mousePos - spawnPos).normalized;

            // 3. Instanciar la lanza
            GameObject lanzaObj = Instantiate(lanzaPrefab, spawnPos, Quaternion.identity);

            // 4. Inicializar con la dirección calculada hacia el cursor
            Proyectil proyectilScript = lanzaObj.GetComponent<Proyectil>();
            if (proyectilScript != null)
            {
                proyectilScript.Inicializar(direccionDisparo);
            }
        }
    }
    public void TakeDamage(float amount)
    {
        if (isDead || invulnerabilityTimer > 0f) return;

        invulnerabilityTimer = invulnerabilityDuration;

        // Disparar sacudida de pantalla
        if (CameraFollow.Instance != null)
        {
            CameraFollow.Instance.TriggerShake(shakeDuration, shakeMagnitude);
        }

        currentHealth -= amount;
        if (currentHealth < 0f) currentHealth = 0f;

        // Actualizar barra verde en la UI
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;

            if (healthSlider.fillRect != null)
            {
                healthSlider.fillRect.gameObject.SetActive(currentHealth > 0f);
            }
        }

        // Parpadeo rojo visual
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

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;

            if (healthSlider.fillRect != null)
            {
                healthSlider.fillRect.gameObject.SetActive(true);
            }
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        currentHealth = 0f;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Desactivar movimiento del script secundario si existe
        PlayerMovement pm = GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = false;

        // Activar pantalla de Game Over
        if (gameOverManager != null)
        {
            gameOverManager.ActivarGameOver();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DetectarContacto(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        DetectarContacto(collision.gameObject);
    }

    private void DetectarContacto(GameObject obj)
    {
        if (isDead) return;

        Enemy enemy = obj.GetComponent<Enemy>();
        if (enemy != null || obj.CompareTag("Enemy"))
        {
            float danio = enemy != null ? enemy.damage : 10f;
            TakeDamage(danio);
        }
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}