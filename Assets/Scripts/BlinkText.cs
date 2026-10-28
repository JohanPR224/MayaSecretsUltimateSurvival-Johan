using System.Collections;
using UnityEngine;
using TMPro;

public class BlinkText : MonoBehaviour
{
    [Header("Modo de Uso")]
    [Tooltip("Marca esto si es para el 'PRESS START' del Menú. Desmárcalo si es el aviso de inicio del Nivel.")]
    [SerializeField] private bool esParaMenu = true;

    [Header("Tiempos")]
    [SerializeField] private float velocidadParpadeo = 0.4f;
    [SerializeField] private float duracionTotal = 3.5f; // Solo aplica si NO es para menú

    [Header("Fondo Opcional")]
    [SerializeField] private GameObject bannerFondo;

    private TextMeshProUGUI textoTMP;

    void Awake()
    {
        textoTMP = GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        if (esParaMenu)
        {
            StartCoroutine(RutinaParpadeoInfinitoMenu());
        }
        else
        {
            StartCoroutine(RutinaMensajeFijoNivel());
        }
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    // 1. Parpadeo constante e infinito para el Menú Principal ("PRESS START")
    private IEnumerator RutinaParpadeoInfinitoMenu()
    {
        while (true)
        {
            if (textoTMP != null)
            {
                textoTMP.enabled = !textoTMP.enabled;
            }

            yield return new WaitForSeconds(velocidadParpadeo);
        }
    }

    // 2. Mensaje estático temporal para la bienvenida del Nivel
    private IEnumerator RutinaMensajeFijoNivel()
    {
        if (textoTMP != null)
        {
            textoTMP.enabled = true;
        }

        yield return new WaitForSeconds(duracionTotal);

        if (bannerFondo != null)
        {
            bannerFondo.SetActive(false);
        }

        WaveManager waveManager = FindFirstObjectByType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.ComenzarPrimeraOleada();
        }

        gameObject.SetActive(false);
    }
}