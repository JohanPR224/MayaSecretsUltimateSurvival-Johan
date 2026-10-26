using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    [SerializeField] private float speed = 2.5f;

    private Transform target;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        BuscarJugador();
    }

    void BuscarJugador()
    {
        // 1. Intentar por Tag
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            target = p.transform;
            return;
        }

        // 2. Si no tiene Tag, buscar por el script PlayerController
        PlayerController pc = FindFirstObjectByType<PlayerController>();
        if (pc != null)
        {
            target = pc.transform;
        }
    }

    void FixedUpdate()
    {
        // Si aún no encuentra al player, intentar reconectar
        if (target == null)
        {
            BuscarJugador();
            return;
        }

        if (rb != null)
        {
            // Dirección hacia el cazador
            Vector2 direccion = ((Vector2)target.position - rb.position).normalized;

            // Movimiento mediante física para empujar y deslizarse sobre obstáculos
            rb.linearVelocity = direccion * speed;
        }
        else
        {
            // Respaldo por posición si no tuviera Rigidbody2D
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
    }
}