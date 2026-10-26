using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuración del Prefab")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Puntos de Aparición")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Frecuencia de Aparición")]
    [SerializeField] private float timeBetweenSpawns = 2f;

    [Header("Escalado por Oleada")]
    [Tooltip("Incremento porcentual de salud por oleada (0.20 = +20%)")]
    [SerializeField] private float healthIncreasePerWave = 0.20f;
    [Tooltip("Incremento porcentual de daño por oleada (0.10 = +10%)")]
    [SerializeField] private float damageIncreasePerWave = 0.10f;

    private Coroutine spawnLoop;
    private WaveManager waveManager;

    void Awake()
    {
        // Si no se asignaron puntos en el Inspector, toma a sus hijos directos
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            List<Transform> points = new List<Transform>();
            foreach (Transform child in transform)
            {
                points.Add(child);
            }
            spawnPoints = points.ToArray();
        }
    }

    void Start()
    {
        waveManager = FindFirstObjectByType<WaveManager>();
    }

    public void IniciarOleadaDeEnemigos(int cantidadTotal, int numeroOleada)
    {
        if (spawnLoop != null)
        {
            StopCoroutine(spawnLoop);
        }

        spawnLoop = StartCoroutine(RutinaSpawnOleada(cantidadTotal, numeroOleada));
    }

    public void DetenerGeneracion()
    {
        if (spawnLoop != null)
        {
            StopCoroutine(spawnLoop);
            spawnLoop = null;
        }
    }

    private IEnumerator RutinaSpawnOleada(int totalEnemigos, int numeroOleada)
    {
        for (int i = 0; i < totalEnemigos; i++)
        {
            SpawnUnEnemigo(numeroOleada);
            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        spawnLoop = null;
    }

    private void SpawnUnEnemigo(int numeroOleada)
    {
        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0) return;

        // Seleccionar un spawn point aleatorio
        Transform punto = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject nuevo = Instantiate(enemyPrefab, punto.position, Quaternion.identity);

        // Aplicar el escalado de salud y fuerza
        Enemy scriptEnemy = nuevo.GetComponent<Enemy>();
        if (scriptEnemy != null)
        {
            scriptEnemy.ConfigurarEstadisticas(numeroOleada, healthIncreasePerWave, damageIncreasePerWave);
        }
    }
}