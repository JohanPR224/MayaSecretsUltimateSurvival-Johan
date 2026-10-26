using System.Collections;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [Header("Configuración de Oleadas")]
    [SerializeField] private int currentWave = 1;
    [Tooltip("Cantidad exacta de enemigos por oleada: índice 0 = Oleada 1, índice 1 = Oleada 2, etc.")]
    [SerializeField] private int[] enemiesPerWave = new int[] { 25, 35, 50 };
    [SerializeField] private float countdownBetweenWaves = 3.5f;

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
        StartCoroutine(RutinaIniciarOleada());
    }

    private IEnumerator RutinaIniciarOleada()
    {
        waveActive = false;

        if (spawner != null)
        {
            spawner.DetenerGeneracion();
        }

        // Mostrar texto de anuncio de la oleada
        if (waveAnnouncementText != null)
        {
            waveAnnouncementText.text = $"OLEADA {currentWave}";
            waveAnnouncementText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(countdownBetweenWaves);

        if (waveAnnouncementText != null)
        {
            waveAnnouncementText.gameObject.SetActive(false);
        }

        // 1. Obtener la cantidad fija según el índice del arreglo
        int index = currentWave - 1;
        if (index < enemiesPerWave.Length)
        {
            totalEnemiesThisWave = enemiesPerWave[index];
        }
        else
        {
            // Respaldo para oleadas posteriores a la 3 (último valor + 15 por oleada)
            totalEnemiesThisWave = enemiesPerWave[enemiesPerWave.Length - 1] + ((currentWave - enemiesPerWave.Length) * 15);
        }

        defeatedEnemies = 0;
        waveActive = true;

        // 2. Notificar al Spawner la cantidad exacta que debe instanciar
        if (spawner != null)
        {
            spawner.IniciarOleadaDeEnemigos(totalEnemiesThisWave, currentWave);
        }
    }

    public void OnEnemyKilled()
    {
        if (!waveActive) return;

        defeatedEnemies++;

        // Avanzar de oleada cuando se hayan eliminado todos los enemigos asignados
        if (defeatedEnemies >= totalEnemiesThisWave)
        {
            currentWave++;
            StartCoroutine(RutinaIniciarOleada());
        }
    }
}