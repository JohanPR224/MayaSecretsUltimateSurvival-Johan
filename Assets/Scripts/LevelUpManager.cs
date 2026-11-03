using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpManager : MonoBehaviour
{
    [Header("UI - Panel Principal")]
    [SerializeField] private GameObject levelUpPanel;

    [Header("UI - Tarjeta Opcion 1 (Izquierda)")]
    [SerializeField] private Image iconoTarjeta1;
    [SerializeField] private TextMeshProUGUI tituloTarjeta1;
    [SerializeField] private TextMeshProUGUI descripcionTarjeta1;
    [SerializeField] private Button botonSeleccionar1;

    [Header("UI - Tarjeta Opcion 2 (Derecha)")]
    [SerializeField] private Image iconoTarjeta2;
    [SerializeField] private TextMeshProUGUI tituloTarjeta2;
    [SerializeField] private TextMeshProUGUI descripcionTarjeta2;
    [SerializeField] private Button botonSeleccionar2;

    [Header("Pool de Habilidades Disponibles")]
    [SerializeField] private List<DatosHabilidad> listaHabilidades = new List<DatosHabilidad>();

    [Header("Contenedor HUD de Habilidades Adquiridas")]
    [SerializeField] private Transform skillsContainer; 
    [SerializeField] private GameObject skillIconPrefab; 
    [SerializeField] private LanzaDeCeibaHabilidad habilidadLanzaCeiba;

    [Header("Referencias a Sistemas")]
    [SerializeField] private HabilidadTransformacion2D habilidadTransformacion;

    private DatosHabilidad habilidadElegida1;
    private DatosHabilidad habilidadElegida2;

    void Start()
    {
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(false);
        }

        if (botonSeleccionar1 != null)
            botonSeleccionar1.onClick.AddListener(() => ConfirmarSeleccion(habilidadElegida1));

        if (botonSeleccionar2 != null)
            botonSeleccionar2.onClick.AddListener(() => ConfirmarSeleccion(habilidadElegida2));
    }

    public void MostrarPantallaSeleccion()
    {
        // Si ya no quedan habilidades en la lista, no muestra la ventana
        if (listaHabilidades.Count == 0)
        {
            Debug.Log("¡Todas las habilidades han sido desbloqueadas!");
            return;
        }

        // Si solo queda 1 habilidad disponible, muestra esa en ambas tarjetas o deshabilita la segunda
        if (listaHabilidades.Count == 1)
        {
            habilidadElegida1 = listaHabilidades[0];
            habilidadElegida2 = null;

            ActualizarTarjetaUI(iconoTarjeta1, tituloTarjeta1, descripcionTarjeta1, habilidadElegida1);
            if (botonSeleccionar2 != null) botonSeleccionar2.interactable = false;
        }
        else
        {
            if (botonSeleccionar2 != null) botonSeleccionar2.interactable = true;

            // Elegir 2 habilidades aleatorias sin repetir
            int index1 = Random.Range(0, listaHabilidades.Count);
            int index2 = Random.Range(0, listaHabilidades.Count);

            while (index2 == index1)
            {
                index2 = Random.Range(0, listaHabilidades.Count);
            }

            habilidadElegida1 = listaHabilidades[index1];
            habilidadElegida2 = listaHabilidades[index2];

            ActualizarTarjetaUI(iconoTarjeta1, tituloTarjeta1, descripcionTarjeta1, habilidadElegida1);
            ActualizarTarjetaUI(iconoTarjeta2, tituloTarjeta2, descripcionTarjeta2, habilidadElegida2);
        }

        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    private void ActualizarTarjetaUI(Image icono, TextMeshProUGUI titulo, TextMeshProUGUI desc, DatosHabilidad datos)
    {
        if (datos == null) return;
        if (icono != null) icono.sprite = datos.icono;
        if (titulo != null) titulo.text = datos.nombreHabilidad;
        if (desc != null) desc.text = datos.descripcion;
    }

    private void ConfirmarSeleccion(DatosHabilidad habilidadSeleccionada)
    {
        if (habilidadSeleccionada == null) return;

        // 1. Crear el icono dinámico en el HUD
        AgregarIconoAlHUD(habilidadSeleccionada.icono);

        // 2. Ejecutar la lógica del jugador según la habilidad
        switch (habilidadSeleccionada.idHabilidad)
        {
            case "Dash":
                if (habilidadTransformacion != null) habilidadTransformacion.DesbloquearDash();
                break;

            case "Aura":
             AuraMagnetica aura = habilidadTransformacion.GetComponent<AuraMagnetica>();
            if (aura != null)
            {
               aura.enabled = true; // Activa el comportamiento de atracción
            }
                Debug.Log("Habilidad Desbloqueada: Aura (Escalación / Atracción XP)");
                break;
                
            case "Rotacion":
             HabilidadRotacion rotacion = habilidadTransformacion.GetComponent<HabilidadRotacion>();
            if (rotacion != null)
            {
                rotacion.enabled = true; 
            }
                Debug.Log("Habilidad Desbloqueada: Armas Orbitales");
                break;
            
            case "Transformacion":
             if (habilidadLanzaCeiba != null)
            {
               habilidadLanzaCeiba.Desbloquear();
            }
             Debug.Log("Habilidad Desbloqueada: La Lanza de la Ceiba");
             break;
        }

        // 3. REMOVER la habilidad elegida de la lista para que NUNCA vuelva a salir
        listaHabilidades.Remove(habilidadSeleccionada);

        CerrarPantallaSeleccion();
    }

    private void AgregarIconoAlHUD(Sprite iconoSprite)
    {
        if (skillsContainer != null && skillIconPrefab != null)
        {
            GameObject nuevoIcono = Instantiate(skillIconPrefab, skillsContainer);
            Image img = nuevoIcono.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = iconoSprite;
            }
        }
    }

    private void CerrarPantallaSeleccion()
    {
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(false);
        }
        Time.timeScale = 1f;
    }
}