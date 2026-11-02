using UnityEngine;

[System.Serializable]
public class DatosHabilidad
{
    public string idHabilidad;          // Ej: "Dash", "Aura", "DagaRotativa"
    public string nombreHabilidad;      // Ej: "Traslación (Dash)"
    public Sprite icono;                 // Icono de la habilidad
    [TextArea(2, 4)]
    public string descripcion;          // Descripción corta para la tarjeta
}