using System.Collections;
using UnityEngine;

public class HabilidadTransformacion2D : MonoBehaviour
{
    [Header("Estado de Habilidad")]
    [SerializeField] private bool dashDesbloqueado = false;

    [Header("1. Traslación (Dash)")]
    [SerializeField] private float distanciaTraslacion = 3f;
    [SerializeField] private float cooldownTime = 5f;
    [SerializeField] private float duracionEfectoActivado = 0.3f;

    private bool canDash = true;

    void Update()
    {
        if (dashDesbloqueado && (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) && canDash)
        {
            StartCoroutine(EjecutarDashConCooldown());
        }
    }

    public void DesbloquearDash()
    {
        dashDesbloqueado = true;
        Debug.Log("¡Dash (Traslación) desbloqueado!");
    }

    private IEnumerator EjecutarDashConCooldown()
    {
        canDash = false;
        EjecutarTraslacion();

        yield return new WaitForSeconds(cooldownTime);

        canDash = true;
    }

    private void EjecutarTraslacion()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direccionEntrada = new Vector3(horizontal, vertical, 0f);

        if (direccionEntrada == Vector3.zero)
        {
            direccionEntrada = Vector3.up;
        }

        Vector3 direccionNormalizada = direccionEntrada.normalized;
        transform.position += direccionNormalizada * distanciaTraslacion;

        Debug.Log($"[Traslación Dash] Posición: {transform.position}");
    }
}