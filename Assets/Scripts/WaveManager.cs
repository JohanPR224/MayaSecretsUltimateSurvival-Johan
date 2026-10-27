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
    [SerializeField] private float blinkInterval = 0.25f; // Velocidad del parpadeo

    [Header("Referencias UI")]
    [SerializeField] private TextMeshProUGUI waveAnnouncementText;

    private int totalEnemiesThisWave;
    private int defeatedEnemies = 0;
    private bool waveActive = false;
    private EnemySpawner spawner;

    public int CurrentWave => currentWave;

    void Start()
    {
        spawner = FindFirstObjectByType<EnemySpawner>();

        // Si no existe el cartel de bienvenida en la escena, arranca normal
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

        if (spawner != null)
        {
            spawner.DetenerGeneracion();
        }

        // 1. Configurar y encender el texto de la oleada
        if (waveAnnouncementText != null)
        {
            waveAnnouncementText.text = $"OLEADA {currentWave}";
            waveAnnouncementText.gameObject.SetActive(true);

            // 2. Parpadeo durante el tiempo de espera (countdownBetweenWaves)
            float tiempoTranscurrido = 0f;
            while (tiempoTranscurrido < countdownBetweenWaves)
            {
                waveAnnouncementText.enabled = !waveAnnouncementText.enabled;
                yield return new WaitForSeconds(blinkInterval);
                tiempoTranscurrido += blinkInterval;
            }

            // 3. Asegurar que quede visible antes de ocultar el GameObject
            waveAnnouncementText.enabled = true;
            waveAnnouncementText.gameObject.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(countdownBetweenWaves);
        }

        // 4. Calcular cantidad de enemigos para esta oleada
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

        // 5. Iniciar la generación de enemigos
        if (spawner != null)
        {
            spawner.IniciarOleadaDeEnemigos(totalEnemiesThisWave, currentWave);
        }
    }

    public void OnEnemyKilled()
    {
        if (!waveActive) return;

        defeatedEnemies++;

        if (defeatedEnemies >= totalEnemiesThisWave)
        {
            currentWave++;
            StartCoroutine(RutinaIniciarOleada());
        }
    }
}