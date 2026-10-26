using UnityEngine;

public class AnimacionEnemigo2D : MonoBehaviour
{
    [Header("Rebote / Squash & Stretch")]
    [SerializeField] private float bounceSpeed = 10f;       // Velocidad del salto/caminar
    [SerializeField] private float bounceScaleY = 0.15f;     // Qué tanto se estira verticalmente
    [SerializeField] private float bounceScaleX = 0.1f;      // Compensación horizontal

    [Header("Inclinación al andar")]
    [SerializeField] private float tiltAngle = 8f;           // Inclinación lateral
    [SerializeField] private float tiltSpeed = 10f;

    private Vector3 originalScale;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    void Awake()
    {
        originalScale = transform.localScale;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 1. Detectar si el enemigo se está moviendo
        bool isMoving = rb != null ? rb.linearVelocity.sqrMagnitude > 0.05f : true;

        if (isMoving)
        {
            // Efecto de rebote usando una onda senoidal
            float sinWave = Mathf.Sin(Time.time * bounceSpeed);
            
            float newY = originalScale.y + (sinWave * bounceScaleY * originalScale.y);
            float newX = originalScale.x - (sinWave * bounceScaleX * originalScale.x);
            transform.localScale = new Vector3(newX, newY, originalScale.z);

            // Inclinación rítmica (bamboleo)
            float rotZ = Mathf.Sin(Time.time * tiltSpeed) * tiltAngle;
            transform.rotation = Quaternion.Euler(0f, 0f, rotZ);

            // Voltear el sprite según la dirección en X
            if (rb != null && sr != null)
            {
                if (rb.linearVelocity.x > 0.05f)
                {
                    sr.flipX = false;
                }
                else if (rb.linearVelocity.x < -0.05f)
                {
                    sr.flipX = true;
                }
            }
        }
        else
        {
            // Volver suavemente a la escala y rotación original si se frena
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * 8f);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * 8f);
        }
    }

    void OnDisable()
    {
        // Restaurar escala y rotación al morir o desactivarse
        transform.localScale = originalScale;
        transform.rotation = Quaternion.identity;
    }
}