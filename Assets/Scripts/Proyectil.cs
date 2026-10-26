using UnityEngine;

public class Proyectil : MonoBehaviour
{
    [Header("Ajustes de Vuelo")]
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private float damage = 15f;

    private Vector2 moveDirection = Vector2.right;

    void Start()
    {
        // Se destruye automáticamente tras cumplirse el tiempo de vida
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Desplazamiento continuo en la dirección indicada
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }

    public void Inicializar(Vector2 direccion)
    {
        moveDirection = direccion.normalized;

        // Orientar el ángulo del sprite según la trayectoria
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignorar si colisiona con el propio personaje
        if (collision.CompareTag("Player") || collision.GetComponent<PlayerController>() != null)
        {
            return;
        }

        // Colisión con enemigo
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null || collision.CompareTag("Enemy"))
        {
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
            return;
        }

        // Colisión con muros u obstáculos del entorno
        if (collision.CompareTag("Obstaculo") || collision.name.Contains("Wall"))
        {
            Destroy(gameObject);
        }
    }
}