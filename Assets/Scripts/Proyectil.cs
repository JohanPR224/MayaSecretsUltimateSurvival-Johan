using UnityEngine;

public class Proyectil : MonoBehaviour
{
    [Header("Ajustes del Proyectil")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifeTime = 3f;

    void Start()
    {
        // Se destruye automáticamente tras el tiempo de vida
        Destroy(gameObject, lifeTime);
    }

    // Detección si el proyectil o el obstáculo tiene 'Is Trigger' activado
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ProcesarImpacto(collision.gameObject);
    }

    // Detección si la colisión es física sólida
    private void OnCollisionEnter2D(Collision2D collision)
    {
        ProcesarImpacto(collision.gameObject);
    }

    private void ProcesarImpacto(GameObject target)
    {
        // Ignora al propio jugador si sale de su centro
        if (target.CompareTag("Player"))
        {
            return;
        }

        // Si choca con un enemigo le hace daño y se destruye
        if (target.CompareTag("Enemy"))
        {
            Enemy enemy = target.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
            return;
        }

        // Si choca con los obstáculos y muros recién etiquetados
        if (target.CompareTag("Obstaculo") || target.CompareTag("Wall"))
        {
            Destroy(gameObject);
            return;
        }
    }
}