using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    [Header("Pantallas del Menú")]
    [SerializeField] private GameObject pressStartScreen;
    [SerializeField] private GameObject menuOptionsScreen;
    [SerializeField] private GameObject panelOpciones;

    [Header("Botones Principales")]
    [SerializeField] private Button btnJugar;
    [SerializeField] private Button btnOpciones;
    [SerializeField] private Button btnCreditos;
    [SerializeField] private Button btnSalir;

    [Header("Navegación y Puntero")]
    [SerializeField] private RectTransform daggerPointer;
    [SerializeField] private Vector2 pointerOffset = new Vector2(-150f, 0f);

    [Header("Configuración de Escena")]
    [SerializeField] private string gameSceneName = "Nivel1";

    [Header("Ajustes de Audio (Opciones)")]
    [SerializeField] private Slider sliderMusica;
    [SerializeField] private Slider sliderSFX;
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    private Button[] menuButtons;
    private int currentSelectedIndex = 0;
    private bool isInMenuButtons = false;
    private bool isInSubPanel = false;

    void Awake()
    {
        menuButtons = new Button[] { btnJugar, btnOpciones, btnCreditos, btnSalir };
    }

    void Start()
    {
        if (pressStartScreen != null) pressStartScreen.SetActive(true);
        if (menuOptionsScreen != null) menuOptionsScreen.SetActive(false);
        if (panelOpciones != null) panelOpciones.SetActive(false);

        if (daggerPointer != null)
        {
            daggerPointer.gameObject.SetActive(false);
        }

        CargarPreferenciasAudio();
    }

    void Update()
    {
        // 1. Pantalla de inicio
        if (!isInMenuButtons && !isInSubPanel)
        {
            if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
            {
                EntrarAMenuPrincipal();
            }
            return;
        }

        // 2. Navegación en el menú principal
        if (isInMenuButtons && !isInSubPanel)
        {
            // Movimiento hacia abajo
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                CambiarSeleccion(1);
            }
            // Movimiento hacia arriba
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                CambiarSeleccion(-1);
            }
            // Confirmar selección
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))
            {
                EjecutarBotonSeleccionado();
            }

            // Sincronizar si el usuario selecciona con el mouse o con la navegación interna de Unity
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
            {
                for (int i = 0; i < menuButtons.Length; i++)
                {
                    if (menuButtons[i] != null && EventSystem.current.currentSelectedGameObject == menuButtons[i].gameObject)
                    {
                        if (currentSelectedIndex != i)
                        {
                            currentSelectedIndex = i;
                            ColocarDagaEnBoton(menuButtons[i]);
                        }
                        break;
                    }
                }
            }
        }

        // 3. Regresar con Escape
        if (isInSubPanel && Input.GetKeyDown(KeyCode.Escape))
        {
            VolverAlMenu();
        }
    }

    public void EntrarAMenuPrincipal()
    {
        isInMenuButtons = true;
        isInSubPanel = false;

        if (pressStartScreen != null) pressStartScreen.SetActive(false);
        if (menuOptionsScreen != null) menuOptionsScreen.SetActive(true);
        if (panelOpciones != null) panelOpciones.SetActive(false);

        currentSelectedIndex = 0;
        SeleccionarBoton(0);
    }

    public void AbrirOpciones()
    {
        isInMenuButtons = false;
        isInSubPanel = true;

        if (menuOptionsScreen != null) menuOptionsScreen.SetActive(false);
        if (panelOpciones != null) panelOpciones.SetActive(true);

        if (daggerPointer != null)
        {
            daggerPointer.gameObject.SetActive(false);
        }
    }

    public void VolverAlMenu()
    {
        isInMenuButtons = true;
        isInSubPanel = false;

        if (panelOpciones != null) panelOpciones.SetActive(false);
        if (menuOptionsScreen != null) menuOptionsScreen.SetActive(true);

        PlayerPrefs.Save();
        SeleccionarBoton(1); // Deja la daga en 'Opciones' al volver
    }

    public void Jugar()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void SalirDelJuego()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public void AbrirCreditos()
    {
        Debug.Log("Créditos seleccionados");
    }

    private void CambiarSeleccion(int direccion)
    {
        if (menuButtons == null || menuButtons.Length == 0) return;

        currentSelectedIndex += direccion;

        if (currentSelectedIndex >= menuButtons.Length) currentSelectedIndex = 0;
        else if (currentSelectedIndex < 0) currentSelectedIndex = menuButtons.Length - 1;

        SeleccionarBoton(currentSelectedIndex);
    }

    private void SeleccionarBoton(int index)
    {
        if (menuButtons == null || index < 0 || index >= menuButtons.Length) return;

        Button boton = menuButtons[index];
        if (boton != null)
        {
            boton.Select();
            ColocarDagaEnBoton(boton);
        }
    }

    private void ColocarDagaEnBoton(Button boton)
    {
        if (daggerPointer != null && boton != null)
        {
            daggerPointer.gameObject.SetActive(true);
            RectTransform targetRect = boton.GetComponent<RectTransform>();
            if (targetRect != null)
            {
                daggerPointer.position = targetRect.position + (Vector3)pointerOffset;
            }
        }
    }

    private void EjecutarBotonSeleccionado()
    {
        if (menuButtons != null && currentSelectedIndex >= 0 && currentSelectedIndex < menuButtons.Length)
        {
            if (menuButtons[currentSelectedIndex] != null)
            {
                menuButtons[currentSelectedIndex].onClick.Invoke();
            }
        }
    }

    private void CargarPreferenciasAudio()
    {
        float volMusica = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
        float volSFX = PlayerPrefs.GetFloat("SFXVolume", 0.8f);

        if (sliderMusica != null)
        {
            sliderMusica.value = volMusica;
            sliderMusica.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sliderSFX != null)
        {
            sliderSFX.value = volSFX;
            sliderSFX.onValueChanged.AddListener(SetSFXVolume);
        }

        AplicarVolumen(volMusica, volSFX);
    }

    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        if (musicAudioSource != null) musicAudioSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);
        if (sfxAudioSource != null) sfxAudioSource.volume = volume;
    }

    private void AplicarVolumen(float musica, float sfx)
    {
        if (musicAudioSource != null) musicAudioSource.volume = musica;
        if (sfxAudioSource != null) sfxAudioSource.volume = sfx;
    }
}