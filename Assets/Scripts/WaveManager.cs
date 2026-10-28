using System.Collections;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [Header("Configuración de Oleadas")]
    [SerializeField] private int currentWave = 1;
    [Tooltip("Cantidad exacta de enemigos por oleada: Oleada 1 = 25, Oleada 2 = 35, Oleada 3 = 50")]
    [SerializeField] private int[] enemiesPerWave = new int[] { 25, 35, 50 };
    [SerializeField] private float countdownBetweenWaves = 3.5f;

    [Header("Efecto de Parpadeo de Oleada")]
    [SerializeField] private float blinkInterval = 0.25f;

    [Header("Referencias UI - Anuncio de Oleada")]
    [Tooltip("Arrastra aquí el panel contenedor completo con el marco de piedra (WaveAnnouncementPanel)")]
    [SerializeField] private GameObject waveAnnouncementPanel;
    [SerializeField] private TextMeshProUGUI waveAnnouncementText;

    [Header("Referencias UI - Contador del HUD")]
    [Tooltip("Arrastra aquí el panel contenedor del contador (WaveCounterPanel)")]
    [SerializeField] private GameObject waveCounterPanel;
    [SerializeField] private TextMeshProUGUI waveCounterHUDText;

    private int totalEnemiesThisWave;
    private int defeatedEnemies = 0;
    private bool waveActive = false;
    private EnemySpawner spawner;

    public int CurrentWave => currentWave;

    void Start()
    {
        spawner = FindFirstObjectByType<EnemySpawner>();

        // Apagar paneles al iniciar la escena
        if (waveAnnouncementPanel != null)
        {
            waveAnnouncementPanel.SetActive(false);
        }

        if (waveCounterPanel != null)
        {
            waveCounterPanel.SetActive(false);
        }

        BlinkText cartelBienvenida = FindFirstObjectByType<BlinkText>();
        if (cartelBienvenida == null)
        {
            ComenzarPrimeraOleada();
        }
    }

    public void ComenzarPrimeraOleada()
    {
        StartCoroutine(RutinaIniciarOleada());
    }

    private IEnumerator RutinaIniciarOleada()
    {
        waveActive = false;

        // Apagar el contador del HUD durante el anuncio de la nueva oleada
        if (waveCounterPanel != null)
        {
            waveCounterPanel.SetActive(false);
        }

        if (spawner != null)
        {
            spawner.DetenerGeneracion();
        }

        // Anuncio grande con marco de piedra parpadeando completo
        if (waveAnnouncementText != null && waveAnnouncementPanel != null)
        {
            waveAnnouncementText.text = $"OLEADA {currentWave}";
            waveAnnouncementPanel.SetActive(true);

            float tiempoTranscurrido = 0f;
            while (tiempoTranscurrido < countdownBetweenWaves)
            {
                // Alternar la visibilidad de todo el panel (fondo + texto)
                waveAnnouncementPanel.SetActive(!waveAnnouncementPanel.activeSelf);
                yield return new WaitForSeconds(blinkInterval);
                tiempoTranscurrido += blinkInterval;
            }

            waveAnnouncementPanel.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(countdownBetweenWaves);
        }

        // Calcular total de enemigos
        int index = currentWave - 1;
        if (index < enemiesPerWave.Length)
        {
            totalEnemiesThisWave = enemiesPerWave[index];
        }
        else
        {
            totalEnemiesThisWave = enemiesPerWave[enemiesPerWave.Length - 1] + ((currentWave - enemiesPerWave.Length) * 15);
        }

        defeatedEnemies = 0;
        waveActive = true;

        // Mostrar el marco del contador en el HUD
        if (waveCounterPanel != null)
        {
            waveCounterPanel.SetActive(true);
            ActualizarHUDContador();
        }

        if (spawner != null)
        {
            spawner.IniciarOleadaDeEnemigos(totalEnemiesThisWave, currentWave);
        }
    }

    public void OnEnemyKilled()
    {
        if (!waveActive) return;

        defeatedEnemies++;
        ActualizarHUDContador();

        if (defeatedEnemies >= totalEnemiesThisWave)
        {
            currentWave++;
            StartCoroutine(RutinaIniciarOleada());
        }
    }

    private void ActualizarHUDContador()
    {
        if (waveCounterHUDText != null)
        {
            int restantes = totalEnemiesThisWave - defeatedEnemies;
            waveCounterHUDText.text = $"OLEADA {currentWave}  |  RESTANTES: {restantes}";
        }
    }
}