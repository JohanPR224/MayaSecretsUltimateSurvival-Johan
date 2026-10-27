using UnityEngine;

public class GemaExperiencia : MonoBehaviour
{
    [Header("Ajustes de Experiencia")]
    [SerializeField] private int valorExp = 10;
    [SerializeField] private float radioAtraccion = 3.5f;
    [SerializeField] private float velocidadAtraccion = 6.5f;

    private Transform playerTransform;
    private bool siendoAtraida = false;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            playerTransform = p.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distancia = Vector2.Distance(transform.position, playerTransform.position);

        // Si el jugador entra en el radio magnético, la orbe vuela hacia él
        if (distancia <= radioAtraccion)
        {
            siendoAtraida = true;
        }

        if (siendoAtraida)
        {
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, velocidadAtraccion * Time.deltaTime);

            // Al tocar al jugador se recolecta
            if (distancia < 0.35f)
            {
                Recolectar();
            }
        }
    }

    private void Recolectar()
    {
        ExperienceManager em = FindFirstObjectByType<ExperienceManager>();
        if (em != null)
        {
            em.AgregarExperiencia(valorExp);
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Recolectar();
        }
    }
}