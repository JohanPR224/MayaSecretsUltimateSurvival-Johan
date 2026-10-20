using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Objetivo")]
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.2f;

    [Header("Límites del Mapa (Clamp)")]
    [SerializeField] private bool useLimits = true;
    [SerializeField] private float minX = -4.13f;
    [SerializeField] private float maxX = 4.13f;
    [SerializeField] private float minY = -2.86f;
    [SerializeField] private float maxY = 2.86f;

    private Vector3 velocity = Vector3.zero;

void LateUpdate()
    {
        if (target == null) return;

        // 1. Objetivo deseado siguiendo al jugador en X e Y
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
        
        // 2. Movimiento suavizado
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        // 3. Bloqueo estricto contra los límites del mapa
        if (useLimits)
        {
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minX, maxX);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minY, maxY);
        }

        // 4. Asignar posición final
        transform.position = smoothedPosition;
    }
}