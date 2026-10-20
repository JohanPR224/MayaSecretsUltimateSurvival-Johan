using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    // Velocidad de avance del enemigo (unidades por segundo)
    [SerializeField] private float speed = 2.5f;
    
    // Referencia a la posición del jugador
    private Transform player;

    void Start()
    {
        // Al aparecer en la escena, busca el objeto que tenga la etiqueta "Player"
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        // Si el jugador no existe o fue destruido, el enemigo no hace nada
        if (player == null) return;

        // 1. Calcula la dirección restando la posición del objetivo menos la suya propia
        // 2. .normalized hace que el vector tenga longitud 1 (para moverse a velocidad constante)
        Vector2 direction = (player.position - transform.position).normalized;

        // 3. Modifica la posición sumándole la dirección * velocidad * tiempo entre frames
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }
}