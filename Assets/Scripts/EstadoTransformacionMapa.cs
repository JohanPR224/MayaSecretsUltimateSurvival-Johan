using UnityEngine;
using UnityEngine.UI;

public class EstadoTransformacionMapa : MonoBehaviour
{
    [Header("Efecto Visual de Transformación")]
    [SerializeField] private Color colorTransformacion = Color.cyan;

    [Header("Efecto de Regeneración Progresiva")]
    [SerializeField] private float regeneracionPorSegundo = 15f; // Puntos de vida por segundo

    private Color colorOriginal;
    private SpriteRenderer playerSprite;
    private GameObject playerGlobal;
    private PlayerHealth playerHealth;
    private bool jugadorEnAltar = false;

    private Image barraVerdeForzada;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detectar si el objeto o su padre es el Player
        GameObject target = null;
        if (collision.CompareTag("Player"))
        {
            target = collision.gameObject;
        }
        else if (collision.transform.parent != null && collision.transform.parent.CompareTag("Player"))
        {
            target = collision.transform.parent.gameObject;
        }

        if (target != null)
        {
            playerGlobal = target;
            playerHealth = playerGlobal.GetComponent<PlayerHealth>();
            if (playerHealth == null) playerHealth = playerGlobal.GetComponentInChildren<PlayerHealth>();

            playerSprite = playerGlobal.GetComponent<SpriteRenderer>();
            if (playerSprite == null) playerSprite = playerGlobal.GetComponentInChildren<SpriteRenderer>();

            if (playerSprite != null)
            {
                colorOriginal = playerSprite.color;
                playerSprite.color = colorTransformacion;
            }

            jugadorEnAltar = true;
        }
    }

    private void Update()
    {
        if (jugadorEnAltar && playerGlobal != null)
        {
            RegenerarSaludProgresiva();
        }
    }

    private void RegenerarSaludProgresiva()
    {
        float curacionPorFrame = regeneracionPorSegundo * Time.deltaTime;

        // 1. Sumar vida en PlayerHealth si existe
        if (playerHealth != null)
        {
            playerHealth.Heal(curacionPorFrame);
        }

        // 2. Notificar a PlayerController o a cualquier otro script que tenga una función Heal
        playerGlobal.SendMessage("Heal", curacionPorFrame, SendMessageOptions.DontRequireReceiver);

        // 3. Localizar la imagen de la barra UI si aún no está en cache
        if (barraVerdeForzada == null)
        {
            Image[] todasLasImagenes = FindObjectsByType<Image>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (Image img in todasLasImagenes)
            {
                if (img.gameObject.name == "HP_Fill")
                {
                    barraVerdeForzada = img;
                    break;
                }
            }
        }

        // 4. Redibujar la barra verde progresivamente con la vida actual real
        if (barraVerdeForzada != null && playerHealth != null && playerHealth.GetMaxHealth() > 0)
        {
            barraVerdeForzada.fillAmount = playerHealth.GetCurrentHealth() / playerHealth.GetMaxHealth();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        bool esJugador = collision.CompareTag("Player") || 
                         (collision.transform.parent != null && collision.transform.parent.CompareTag("Player"));

        if (esJugador)
        {
            if (playerSprite != null)
            {
                playerSprite.color = colorOriginal;
            }

            jugadorEnAltar = false;
            playerGlobal = null;
            playerHealth = null;
        }
    }
}