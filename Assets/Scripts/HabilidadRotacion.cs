using UnityEngine;

public class HabilidadRotacion : MonoBehaviour
{
    [Header("Configuración del Arma")]
    public GameObject prefabArma;
    public int cantidadArmas = 2; 
    
    [Header("Movimiento y Orientación")]
    public float velocidadRotacion = 150f;
    public float radioOrbita = 2f;
    
    [Tooltip("Cambia este valor a 0, 90, o -90 si la daga no apunta con la punta hacia afuera")]
    public float ajusteAnguloArma = -90f; 

    private GameObject pivoteRotacion;

    void OnEnable()
    {
        if (prefabArma == null) return;

        if (pivoteRotacion == null)
        {
            pivoteRotacion = new GameObject("Pivote_ArmasRotatorias");
            GenerarArmas();
        }
        pivoteRotacion.SetActive(true);
    }

    void OnDisable()
    {
        if (pivoteRotacion != null) pivoteRotacion.SetActive(false);
    }

    void GenerarArmas()
    {
        float anguloPaso = 360f / cantidadArmas;

        for (int i = 0; i < cantidadArmas; i++)
        {
            // 1. Calcular posición en el círculo
            float angulo = i * anguloPaso; // Ángulo en grados
            float anguloRad = angulo * Mathf.Deg2Rad; // Convertir a radianes para la matemática
            
            Vector3 posicionRelativa = new Vector3(Mathf.Cos(anguloRad), Mathf.Sin(anguloRad), 0) * radioOrbita;

            GameObject nuevaArma = Instantiate(prefabArma, pivoteRotacion.transform.position + posicionRelativa, Quaternion.identity);
            nuevaArma.transform.SetParent(pivoteRotacion.transform);

            // 2. Rotar el arma para que mire hacia afuera del centro
            nuevaArma.transform.rotation = Quaternion.Euler(0, 0, angulo + ajusteAnguloArma);
        }
    }

    void Update()
    {
        if (pivoteRotacion != null)
        {
            pivoteRotacion.transform.position = transform.position;
            pivoteRotacion.transform.Rotate(Vector3.forward, velocidadRotacion * Time.deltaTime);
        }
    }
}