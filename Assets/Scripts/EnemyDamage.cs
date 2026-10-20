using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] private int damageAmount = 20;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica si colisionó con el jugador
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }

            // Opcional: el enemigo se destruye al impactar (estilo kamikaze/brote)
            Destroy(gameObject);
        }
    }
}