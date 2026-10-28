using UnityEngine;
using UnityEngine.UI;

public class ExperienceManager : MonoBehaviour
{
    [Header("Referencias UI")]
    [Tooltip("Imagen verde (EXP_Fill) configurada con Image Type: Filled")]
    [SerializeField] private Image expFillImage;
    [Tooltip("Opcional: Si conservas un Slider para la experiencia")]
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
        // Actualizar llenado mediante la imagen en la ranura de piedra
        if (expFillImage != null)
        {
            expFillImage.fillAmount = (float)expActual / expParaSubirNivel;
        }

        // Compatibilidad en caso de seguir usando un Slider convencional
        if (expSlider != null)
        {
            expSlider.maxValue = expParaSubirNivel;
            expSlider.value = expActual;
        }
    }

    public int GetNivelActual() => nivelActual;
}