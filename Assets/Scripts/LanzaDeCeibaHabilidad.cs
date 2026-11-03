using System.Collections;
using UnityEngine;

public class LanzaDeCeibaHabilidad : MonoBehaviour
{
    [Header("Estado")]
    [SerializeField] private bool desbloqueada = false;

    [Header("Configuración del Disparo")]
    [SerializeField] private GameObject prefabLanza;
    [SerializeField] private Transform puntoDisparo; // FirePoint del Player
    [SerializeField] private float cooldown = 20f;

    private bool puedeDisparar = true;

    void Update()
    {
        if (desbloqueada && puedeDisparar)
        {
            StartCoroutine(RutinaDisparo());
        }
    }

    public void Desbloquear()
    {
        desbloqueada = true;
        Debug.Log("¡La Lanza de la Ceiba ha sido desbloqueada!");
    }

    private IEnumerator RutinaDisparo()
    {
        puedeDisparar = false;
        DispararLanza();

        yield return new WaitForSeconds(cooldown);
        puedeDisparar = true;
    }

   private void DispararLanza()
{
    if (prefabLanza == null) return;

    Vector3 origen = puntoDisparo != null ? puntoDisparo.position : transform.position;
    Vector2 direccionDisparo;

    // 1. Buscar al enemigo más cercano
    Enemy enemigoCercano = BuscarEnemigoMasCercano();

    if (enemigoCercano != null)
    {
        // Calcular dirección hacia el enemigo
        direccionDisparo = ((Vector2)enemigoCercano.transform.position - (Vector2)origen).normalized;
    }
    else
    {
        // Si no hay enemigos en el mapa, disparar hacia donde mira Balam
        direccionDisparo = transform.localScale.x < 0 ? Vector2.left : Vector2.right;
    }

    // 2. Instanciar la lanza y pasarle la dirección
    GameObject lanzaObj = Instantiate(prefabLanza, origen, Quaternion.identity);
    ProyectilLanzaCeiba proyectil = lanzaObj.GetComponent<ProyectilLanzaCeiba>();

    if (proyectil != null)
    {
        proyectil.Inicializar(direccionDisparo);
    }
}

private Enemy BuscarEnemigoMasCercano()
{
    // Encuentra todos los objetos de tipo Enemy en la escena
    Enemy[] enemigos = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

    Enemy enemigoMasCercano = null;
    float distanciaMinima = Mathf.Infinity;
    Vector3 posicionActual = transform.position;

    foreach (Enemy enemigo in enemigos)
    {
        if (enemigo == null) continue;

        float distancia = Vector3.Distance(posicionActual, enemigo.transform.position);
        if (distancia < distanciaMinima)
        {
            distanciaMinima = distancia;
            enemigoMasCercano = enemigo;
        }
    }

    return enemigoMasCercano;
}
}