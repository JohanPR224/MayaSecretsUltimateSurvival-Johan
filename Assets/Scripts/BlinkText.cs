using System.Collections;
using UnityEngine;
using TMPro;

public class BlinkText : MonoBehaviour
{
    [Header("Tiempos")]
    [SerializeField] private float duracionTotal = 3.5f; // Tiempo fijo que se muestra el mensaje

    [Header("Fondo Opcional")]
    [SerializeField] private GameObject bannerFondo;     // Objeto DarkBanner para apagarlo junto al texto

    private TextMeshProUGUI textoTMP;

    void Awake()
    {
        textoTMP = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        StartCoroutine(RutinaMensajeFijo());
    }

    private IEnumerator RutinaMensajeFijo()
    {
        // 1. Asegurar que el texto esté visible sin parpadeos
        if (textoTMP != null)
        {
            textoTMP.enabled = true;
        }

        // 2. Esperar el tiempo completo de lectura
        yield return new WaitForSeconds(duracionTotal);

        // 3. Apagar el fondo oscuro si está asignado
        if (bannerFondo != null)
        {
            bannerFondo.SetActive(false);
        }

        // 4. Iniciar la primera oleada en el WaveManager
        WaveManager waveManager = FindFirstObjectByType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.ComenzarPrimeraOleada();
        }

        // 5. Apagar el mensaje
        gameObject.SetActive(false);
    }
}