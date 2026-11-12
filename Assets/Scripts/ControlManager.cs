using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Si usas TextMeshPro para los textos de los botones

public class ControlManager : MonoBehaviour
{
    public static ControlManager Instance;

    // Diccionario con las acciones y sus teclas asignadas
    public KeyCode Arriba { get; private set; }
    public KeyCode Abajo { get; private set; }
    public KeyCode Izquierda { get; private set; }
    public KeyCode Derecha { get; private set; }
    public KeyCode Atacar { get; private set; }
    public KeyCode Interactuar { get; private set; }

    [Header("Textos de los Botones en la UI")]
    public TMP_Text txtArriba;
    public TMP_Text txtAbajo;
    public TMP_Text txtIzquierda;
    public TMP_Text txtDerecha;
    public TMP_Text txtAtacar;
    public TMP_Text txtInteractuar;

    private bool esperandoTecla = false;
    private string accionActual = "";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persiste al cambiar a la escena del juego
            CargarControles();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ActualizarTextosUI();
    }

    private void CargarControles()
    {
        Arriba = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Key_Arriba", "W"));
        Abajo = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Key_Abajo", "S"));
        Izquierda = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Key_Izquierda", "A"));
        Derecha = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Key_Derecha", "D"));
        Atacar = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Key_Atacar", "Space"));
        Interactuar = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Key_Interactuar", "E"));
    }

    // Este método se asigna al evento OnClick() de cada botón en el Canvas
    public void IniciarReasignacion(string nombreAccion)
    {
        if (!esperandoTecla)
        {
            accionActual = nombreAccion;
            esperandoTecla = true;
            MostrarTextoEsperando(nombreAccion);
        }
    }

    private void Update()
    {
        if (esperandoTecla)
        {
            // Detecta cualquier tecla presionada en el teclado
            foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(kcode))
                {
                    // Ignorar clics del mouse si solo quieres teclado (opcional)
                    if (kcode.ToString().Contains("Mouse")) return;

                    AsignarNuevaTecla(accionActual, kcode);
                    esperandoTecla = false;
                    ActualizarTextosUI();
                    break;
                }
            }
        }
    }

    private void AsignarNuevaTecla(string accion, KeyCode nuevaTecla)
    {
        switch (accion)
        {
            case "Arriba": Arriba = nuevaTecla; PlayerPrefs.SetString("Key_Arriba", nuevaTecla.ToString()); break;
            case "Abajo": Abajo = nuevaTecla; PlayerPrefs.SetString("Key_Abajo", nuevaTecla.ToString()); break;
            case "Izquierda": Izquierda = nuevaTecla; PlayerPrefs.SetString("Key_Izquierda", nuevaTecla.ToString()); break;
            case "Derecha": Derecha = nuevaTecla; PlayerPrefs.SetString("Key_Derecha", nuevaTecla.ToString()); break;
            case "Atacar": Atacar = nuevaTecla; PlayerPrefs.SetString("Key_Atacar", nuevaTecla.ToString()); break;
            case "Interactuar": Interactuar = nuevaTecla; PlayerPrefs.SetString("Key_Interactuar", nuevaTecla.ToString()); break;
        }
        PlayerPrefs.Save();
    }

    private void MostrarTextoEsperando(string accion)
    {
        string msg = "...";
        switch (accion)
        {
            case "Arriba": if (txtArriba) txtArriba.text = msg; break;
            case "Abajo": if (txtAbajo) txtAbajo.text = msg; break;
            case "Izquierda": if (txtIzquierda) txtIzquierda.text = msg; break;
            case "Derecha": if (txtDerecha) txtDerecha.text = msg; break;
            case "Atacar": if (txtAtacar) txtAtacar.text = msg; break;
            case "Interactuar": if (txtInteractuar) txtInteractuar.text = msg; break;
        }
    }

    public void ActualizarTextosUI()
    {
        if (txtArriba) txtArriba.text = Arriba.ToString();
        if (txtAbajo) txtAbajo.text = Abajo.ToString();
        if (txtIzquierda) txtIzquierda.text = Izquierda.ToString();
        if (txtDerecha) txtDerecha.text = Derecha.ToString();
        if (txtAtacar) txtAtacar.text = Atacar.ToString();
        if (txtInteractuar) txtInteractuar.text = Interactuar.ToString();
    }

    public void RestablecerPorDefecto()
    {
        PlayerPrefs.DeleteKey("Key_Arriba");
        PlayerPrefs.DeleteKey("Key_Abajo");
        PlayerPrefs.DeleteKey("Key_Izquierda");
        PlayerPrefs.DeleteKey("Key_Derecha");
        PlayerPrefs.DeleteKey("Key_Atacar");
        PlayerPrefs.DeleteKey("Key_Interactuar");
        CargarControles();
        ActualizarTextosUI();
    }
}