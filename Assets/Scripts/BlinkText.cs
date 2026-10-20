using UnityEngine;
using TMPro; // Usamos TextMeshPro, que es el estándar de Unity para UI

public class BlinkText : MonoBehaviour
{
    [Header("Configuración de Parpadeo")]
    [SerializeField] private float blinkSpeed = 1f; // Tiempo en segundos de cada parpadeo

    private TMP_Text textComponent;
    private float timer;

    void Start()
    {
        // Obtiene el componente de texto del objeto
        textComponent = GetComponent<TMP_Text>();
        
        if (textComponent == null)
        {
            Debug.LogError("No se encontró un componente de TextMeshPro (TMP_Text) en este objeto.", gameObject);
            enabled = false;
        }
    }

    void Update()
    {
        // Incrementa el temporizador con el tiempo del juego
        timer += Time.deltaTime;

        // Comportamiento de parpadeo encendido/apagado natural
        if (timer >= blinkSpeed)
        {
            textComponent.enabled = !textComponent.enabled;
            timer = 0f;
        }
    }
    
    // Al desactivar o cambiar de escena, aseguramos que el texto sea visible por si acaso
    void OnDisable()
    {
        if (textComponent != null)
        {
            textComponent.enabled = true;
        }
    }
}