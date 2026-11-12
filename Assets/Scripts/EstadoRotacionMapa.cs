using UnityEngine;

public class EstadoRotacionMapa : MonoBehaviour
{
    [Header("Configuración de Rotación")]
    [SerializeField] private float velocidadRotacion = 45f; // Grados por segundo

    void Update()
    {
        // Aplica una rotación continua sobre el eje Z
        transform.Rotate(0f, 0f, velocidadRotacion * Time.deltaTime);
    }
}