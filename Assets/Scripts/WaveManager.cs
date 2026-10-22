using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Wave
{
    public string waveName = "Oleada 1";
    public int enemyCount = 5;
    public float spawnInterval = 2f;
    public GameObject[] enemyPrefabs;
}

public class WaveManager : MonoBehaviour
{
    [Header("Fondo Oscuro para Textos")]
    [SerializeField] private GameObject darkBanner; // Panel semitransparente

    [Header("Mensaje de Bienvenida / Objetivo")]
    [SerializeField] private TextMeshProUGUI introNoticeText;
    [TextArea(2, 4)]
    [SerializeField] private string introMessage = "SOBREVIVE A TODAS LAS OLEADAS PARA PASAR AL SIGUIENTE NIVEL, BUENA SUERTE";
    [SerializeField] private float introDuration = 3.5f;

    [Header("Configuración de Oleadas")]
    [SerializeField] private List<Wave> waves = new List<Wave>();
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float timeBetweenWaves = 3f;

    [Header("UI Anuncio de Oleada")]
    [SerializeField] private TextMeshProUGUI waveNoticeText;
    [SerializeField] private float noticeDuration = 2.5f;

    [Header("Progreso")]
    private int currentWaveIndex = 0;
    private int enemiesRemaining = 0;
    private bool isSpawning = false;
    private ExperienceSystem expSystem;

    void Start()
    {
        expSystem = FindAnyObjectByType<ExperienceSystem>();

        // Asegurar que inicien ocultos
        if (waveNoticeText != null) waveNoticeText.gameObject.SetActive(false);
        if (introNoticeText != null) introNoticeText.gameObject.SetActive(false);
        if (darkBanner != null) darkBanner.SetActive(false);

        StartCoroutine(SecuenciaInicioJuego());
    }

    private IEnumerator SecuenciaInicioJuego()
    {
        // 1. Mostrar mensaje de inicio con fondo oscuro
        if (introNoticeText != null)
        {
            if (darkBanner != null) darkBanner.SetActive(true);
            introNoticeText.text = introMessage;
            introNoticeText.gameObject.SetActive(true);

            yield return new WaitForSeconds(introDuration);

            introNoticeText.gameObject.SetActive(false);
            if (darkBanner != null) darkBanner.SetActive(false);
        }

        yield return new WaitForSeconds(0.5f);

        // 2. Comenzar la primera oleada
        if (waves.Count > 0)
        {
            StartCoroutine(StartNextWave());
        }
    }

    private IEnumerator StartNextWave()
    {
        if (currentWaveIndex < waves.Count)
        {
            Wave currentWave = waves[currentWaveIndex];
            enemiesRemaining = currentWave.enemyCount;
            isSpawning = true;

            // Mostrar el cartel de la oleada con fondo oscuro
            if (waveNoticeText != null)
            {
                string textoAMostrar = string.IsNullOrEmpty(currentWave.waveName) 
                    ? $"Oleada {currentWaveIndex + 1}" 
                    : currentWave.waveName;

                if (darkBanner != null) darkBanner.SetActive(true);
                waveNoticeText.text = textoAMostrar;
                waveNoticeText.gameObject.SetActive(true);
            }

            yield return new WaitForSeconds(noticeDuration);

            if (waveNoticeText != null)
            {
                waveNoticeText.gameObject.SetActive(false);
                if (darkBanner != null) darkBanner.SetActive(false);
            }

            // Generar los enemigos
            for (int i = 0; i < currentWave.enemyCount; i++)
            {
                if (spawnPoints.Length == 0 || currentWave.enemyPrefabs.Length == 0) break;

                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                GameObject enemyPrefab = currentWave.enemyPrefabs[Random.Range(0, currentWave.enemyPrefabs.Length)];

                if (enemyPrefab != null && spawnPoint != null)
                {
                    GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

                    Enemy enemyScript = enemy.GetComponent<Enemy>();
                    if (enemyScript != null)
                    {
                        enemyScript.onEnemyKilled += OnEnemyDefeated;
                    }
                }

                yield return new WaitForSeconds(currentWave.spawnInterval);
            }

            isSpawning = false;
        }
    }

    void OnEnemyDefeated(int expReward)
    {
        enemiesRemaining--;

        if (expSystem == null)
        {
            expSystem = FindAnyObjectByType<ExperienceSystem>();
        }

        if (expSystem != null)
        {
            expSystem.AddExperience(expReward);
        }

        ComprobarFinDeOleada();
    }

    void ComprobarFinDeOleada()
    {
        if (enemiesRemaining <= 0 && !isSpawning)
        {
            currentWaveIndex++;

            if (currentWaveIndex < waves.Count)
            {
                StartCoroutine(EsperarYSiguienteOleada());
            }
            else
            {
                Debug.Log("¡Todas las oleadas completadas!");
            }
        }
    }

    private IEnumerator EsperarYSiguienteOleada()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        StartCoroutine(StartNextWave());
    }
}