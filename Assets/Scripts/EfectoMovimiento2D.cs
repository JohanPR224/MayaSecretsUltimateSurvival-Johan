using UnityEngine;

public class EfectoMovimiento2D : MonoBehaviour
{
    [Header("Respiración en Reposo")]
    [SerializeField] private float frecuenciaRespiracion = 2.5f;   // Ritmo calmado y pausado
    [SerializeField] private float intensidadRespiracion = 0.025f; // Apenas un 2.5% de cambio

    [Header("Pasos al Caminar")]
    [SerializeField] private float frecuenciaCaminar = 10f;
    [SerializeField] private float intensidadPasos = 0.04f;
    [SerializeField] private float anguloInclinacion = 3.5f;

    private Vector3 escalaOriginal;
    private Rigidbody2D rb;

    void Start()
    {
        escalaOriginal = transform.localScale;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Detectar si se está moviendo usando Rigidbody2D o las teclas de dirección
        bool enMovimiento = (rb != null && rb.linearVelocity.sqrMagnitude > 0.05f) ||
                            Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f ||
                            Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.1f;

        if (enMovimiento)
        {
            // Rebote de pasos con conservación de volumen
            float onda = Mathf.Sin(Time.time * frecuenciaCaminar);
            float factorY = 1f + (onda * intensidadPasos);
            float factorX = 1f - (onda * intensidadPasos * 0.5f);

            transform.localScale = new Vector3(
                escalaOriginal.x * factorX,
                escalaOriginal.y * factorY,
                escalaOriginal.z
            );

            // Ligero bamboleo lateral
            float balanceo = Mathf.Sin(Time.time * frecuenciaCaminar) * anguloInclinacion;
            transform.localRotation = Quaternion.Euler(0f, 0f, balanceo);
        }
        else
        {
            // Respiración sutil: sube suavemente el torso compensando el ancho
            float onda = Mathf.Sin(Time.time * frecuenciaRespiracion);
            float factorY = 1f + (onda * intensidadRespiracion);
            float factorX = 1f - (onda * intensidadRespiracion * 0.6f);

            transform.localScale = new Vector3(
                escalaOriginal.x * factorX,
                escalaOriginal.y * factorY,
                escalaOriginal.z
            );

            // Regreso suave a rotación neutral
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, Time.deltaTime * 8f);
        }
    }
}