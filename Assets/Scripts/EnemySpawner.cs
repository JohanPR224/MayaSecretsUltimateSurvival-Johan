using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuracion de Spawn")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 2f; // Aparece uno cada 2 segundos
    [SerializeField] private float spawnDistance = 9f; // Distancia desde el jugador para aparecer fuera de vista

    private Transform player;
    private float timer;

    void Start()
    {
        // Localiza al jugador por su etiqueta
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        // Calcula un punto aleatorio en un círculo alrededor del jugador
        Vector2 randomPoint = Random.insideUnitCircle.normalized * spawnDistance;
        Vector3 spawnPosition = player.position + new Vector3(randomPoint.x, randomPoint.y, 0f);

        // Clona el prefab del enemigo en esa posicion
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}