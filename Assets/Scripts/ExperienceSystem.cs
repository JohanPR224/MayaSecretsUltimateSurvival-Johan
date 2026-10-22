using UnityEngine;
using UnityEngine.UI;

public class ExperienceSystem : MonoBehaviour
{
    [Header("Configuración de Niveles")]
    public int currentLevel = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100;
    public float expGrowthMultiplier = 1.25f;

    [Header("UI")]
    [SerializeField] private Slider expSlider;

    void Start()
    {
        // Reinicia la experiencia actual al arrancar la partida
        currentExp = 0;
        ActualizarUI();
    }

    public void AddExperience(int amount)
    {
        currentExp += amount;

        // Si llena la barra y sube de nivel
        while (currentExp >= expToNextLevel)
        {
            SubirNivel();
        }

        ActualizarUI();
    }

    private void SubirNivel()
    {
        currentExp -= expToNextLevel;
        currentLevel++;
        expToNextLevel = Mathf.RoundToInt(expToNextLevel * expGrowthMultiplier);
        Debug.Log("¡Subiste de nivel! Nivel actual: " + currentLevel);
    }

    private void ActualizarUI()
    {
        if (expSlider != null)
        {
            expSlider.minValue = 0;
            expSlider.maxValue = expToNextLevel;
            expSlider.value = currentExp;
        }
    }
}