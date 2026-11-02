using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] private int damageAmount = 20;

    private void OnTriggerStay2D(Collider2D other)
{
    // Ignora por completo cualquier trigger (Aura de XP, detectores, etc.)
    if (other.isTrigger) return;

    // Aplica daño únicamente al collider físico de Balam
    if (other.CompareTag("Player") || other.GetComponent<PlayerController>() != null)
    {
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(damageAmount);
        }
    }
}
}
