using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance;

    [Header("Objetivo a seguir")]
    public Transform target;
    [Range(1f, 25f)]
    [SerializeField] private float smoothSpeed = 10f;

    [Header("Límites del Mapa")]
    public bool useLimits = true;
    public Vector2 minLimits = new Vector2(-10f, -10f);
    public Vector2 maxLimits = new Vector2(10f, 10f);

    [Header("Ajustes de Sacudida")]
    [SerializeField] private float defaultDuration = 0.2f;
    [SerializeField] private float defaultMagnitude = 0.4f;

    private float fixedZ = -10f;
    private Vector3 shakeOffset = Vector3.zero;
    private Coroutine shakeCoroutine;

    void Awake()
    {
        Instance = this;
        fixedZ = transform.position.z != 0 ? transform.position.z : -10f;
    }

    void Start()
    {
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
            }
        }
    }

    void Update()
    {
        // Tecla T opcional de prueba
        if (Input.GetKeyDown(KeyCode.T))
        {
            TriggerShake(0.25f, 0.45f);
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 1. Destino base siguiendo al personaje
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, fixedZ);

        // 2. Interpolar suavemente
        Vector3 smoothPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        // 3. Sumar el offset de sacudida
        float finalX = smoothPosition.x + shakeOffset.x;
        float finalY = smoothPosition.y + shakeOffset.y;

        // 4. Clampear al final para que la sacudida jamás muestre el vacío exterior
        if (useLimits)
        {
            finalX = Mathf.Clamp(finalX, minLimits.x, maxLimits.x);
            finalY = Mathf.Clamp(finalY, minLimits.y, maxLimits.y);
        }

        transform.position = new Vector3(finalX, finalY, fixedZ);
    }

    public void TriggerShake(float duration = 0.2f, float magnitude = 0.4f)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        shakeCoroutine = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            shakeOffset = new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
    }
}