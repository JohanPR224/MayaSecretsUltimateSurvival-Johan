using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("Contenedor de UI")]
    [SerializeField] private GameObject gameOverScreen;

    [Header("Nombre de la escena del Menú")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    void Start()
    {
        // Asegura que el juego corra a velocidad normal al iniciar/reiniciar
        Time.timeScale = 1f;

        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
        }
    }

    public void ActivarGameOver()
    {
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }

        // Congela enemigos, jugador y proyectiles tal como quedaron
        Time.timeScale = 0f;
    }

    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void VolverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}