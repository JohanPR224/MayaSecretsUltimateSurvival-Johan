using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Header("Ajustes del Enemigo")]
    public float speed = 2.5f;
    public float health = 30f;
    public float damage = 5f;
    public int expReward = 20;

    [Header("Cadencia de Ataque al Cazador")]
    [SerializeField] private float attackRate = 1.0f;
    private float nextAttackTime = 0f;

    public Action<int> onEnemyKilled;

    private Transform playerTransform;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        BuscarJugador();
    }

    void FixedUpdate()
    {
        if (playerTransform == null)
        {
            BuscarJugador();
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Dirección directa hacia el cazador
        Vector2 direction = ((Vector2)playerTransform.position - rb.position).normalized;

        // Mueve al enemigo por velocidad física para respetar los choques suaves
        rb.linearVelocity = direction * speed;
    }

    private void BuscarJugador()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        onEnemyKilled?.Invoke(expReward);
        Destroy(gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && Time.time >= nextAttackTime)
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage);
                nextAttackTime = Time.time + attackRate;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Time.time >= nextAttackTime)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage);
                nextAttackTime = Time.time + attackRate;
            }
        }
    }
}