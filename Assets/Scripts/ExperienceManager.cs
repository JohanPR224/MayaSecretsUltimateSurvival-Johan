using UnityEngine;
using UnityEngine.UI;

public class ExperienceManager : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private Slider expSlider;

    [Header("Nivel y Experiencia")]
    [SerializeField] private int nivelActual = 1;
    [SerializeField] private int expActual = 0;
    [SerializeField] private int expParaSubirNivel = 50;
    [SerializeField] private float multiplicadorExpPorNivel = 1.35f;

    void Start()
    {
        ActualizarInterfaz();
    }

    public void AgregarExperiencia(int cantidad)
    {
        expActual += cantidad;

        while (expActual >= expParaSubirNivel)
        {
            expActual -= expParaSubirNivel;
            SubirDeNivel();
        }

        ActualizarInterfaz();
    }

    private void SubirDeNivel()
    {
        nivelActual++;
        expParaSubirNivel = Mathf.RoundToInt(expParaSubirNivel * multiplicadorExpPorNivel);
        Debug.Log($"¡Nivel Subido! Nivel actual: {nivelActual}");
    }

    private void ActualizarInterfaz()
    {
        if (expSlider != null)
        {
            expSlider.maxValue = expParaSubirNivel;
            expSlider.value = expActual;
        }
    }

    public int GetNivelActual() => nivelActual;
}