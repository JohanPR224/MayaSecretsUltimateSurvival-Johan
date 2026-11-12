using UnityEngine;

public class EstadoEscalacionMapa : MonoBehaviour
{
    [Header("Configuración de Escalación")]
    [SerializeField] private float escalaMinima = 0.85f;
    [SerializeField] private float escalaMaxima = 1.15f;
    [SerializeField] private float velocidadPulsacion = 2f;

    private Vector3 escalaInicial;

    void Start()
    {
        escalaInicial = transform.localScale;
    }

    void Update()
    {
        // Calcula una onda senoidal suave para alterar la escala
        float factor = (Mathf.Sin(Time.time * velocidadPulsacion) + 1f) / 2f;
        float escalaActual = Mathf.Lerp(escalaMinima, escalaMaxima, factor);

        transform.localScale = escalaInicial * escalaActual;
    }
}