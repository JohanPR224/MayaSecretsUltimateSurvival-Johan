using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    [Header("Pantallas del Menú")]
    [SerializeField] private GameObject pressStartScreen;
    [SerializeField] private GameObject menuOptionsScreen;

    [Header("Ajustes de la Animación")]
    [SerializeField] private float fadeDuration = 0.6f;

    [Header("Configuración de Escenas")]
    [SerializeField] private string firstLevelSceneName = "Nivel1";

    [Header("Botones")]
    [SerializeField] private Button btnJugar;
    [SerializeField] private Button btnOpciones;
    [SerializeField] private Button btnCreditos;
    [SerializeField] private Button btnSalir;

    [Header("Indicador Visual (Daga)")]
    [SerializeField] private RectTransform daggerPointer;
    [SerializeField] private float daggerOffsetX = -180f;

    private bool hasStarted = false;
    private GameObject lastSelectedObject;

    private CanvasGroup startCanvasGroup;
    private CanvasGroup optionsCanvasGroup;

    void Start()
    {
        Time.timeScale = 1f;

        // Configuración de CanvasGroups para la transición suave
        if (pressStartScreen != null)
        {
            pressStartScreen.SetActive(true);
            startCanvasGroup = pressStartScreen.GetComponent<CanvasGroup>();
            if (startCanvasGroup == null)
            {
                startCanvasGroup = pressStartScreen.AddComponent<CanvasGroup>();
            }
            startCanvasGroup.alpha = 1f;
        }

        if (menuOptionsScreen != null)
        {
            menuOptionsScreen.SetActive(true);
            optionsCanvasGroup = menuOptionsScreen.GetComponent<CanvasGroup>();
            if (optionsCanvasGroup == null)
            {
                optionsCanvasGroup = menuOptionsScreen.AddComponent<CanvasGroup>();
            }
            optionsCanvasGroup.alpha = 0f;
            optionsCanvasGroup.interactable = false;
            optionsCanvasGroup.blocksRaycasts = false;
        }

        if (daggerPointer != null)
        {
            daggerPointer.gameObject.SetActive(false);
        }

        if (btnJugar != null) btnJugar.onClick.AddListener(PlayGame);
        if (btnSalir != null) btnSalir.onClick.AddListener(QuitGame);
    }

    void Update()
    {
        if (!hasStarted)
        {
            if (Input.anyKeyDown)
            {
                hasStarted = true;
                StartCoroutine(TransitionToMenuRoutine());
            }
            return;
        }

        // Posicionar la daga junto al botón seleccionado
        if (daggerPointer != null && EventSystem.current != null)
        {
            GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

            if (currentSelected != null && currentSelected != lastSelectedObject)
            {
                if (currentSelected.GetComponent<Button>() != null)
                {
                    lastSelectedObject = currentSelected;
                    RectTransform buttonRect = currentSelected.GetComponent<RectTransform>();

                    daggerPointer.position = new Vector3(
                        buttonRect.position.x + daggerOffsetX,
                        buttonRect.position.y,
                        buttonRect.position.z
                    );

                    if (!daggerPointer.gameObject.activeSelf)
                    {
                        daggerPointer.gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    private IEnumerator TransitionToMenuRoutine()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            float progress = timer / fadeDuration;

            if (startCanvasGroup != null)
            {
                startCanvasGroup.alpha = Mathf.Lerp(1f, 0f, progress);
            }

            if (optionsCanvasGroup != null)
            {
                optionsCanvasGroup.alpha = Mathf.Lerp(0f, 1f, progress);
            }

            yield return null;
        }

        if (startCanvasGroup != null)
        {
            startCanvasGroup.alpha = 0f;
            pressStartScreen.SetActive(false);
        }

        if (optionsCanvasGroup != null)
        {
            optionsCanvasGroup.alpha = 1f;
            optionsCanvasGroup.interactable = true;
            optionsCanvasGroup.blocksRaycasts = true;
        }

        if (btnJugar != null)
        {
            btnJugar.Select();
        }
    }

    public void PlayGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(firstLevelSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Cerrando el juego...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}