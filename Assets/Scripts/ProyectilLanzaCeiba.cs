using UnityEngine;

public class ProyectilLanzaCeiba : MonoBehaviour
{
    [SerializeField] private float velocidad = 14f;
    [SerializeField] private float tiempoVida = 3f;
    [SerializeField] private float dano = 40f;

    private Vector2 direccionDisparo;

    public void Inicializar(Vector2 direccion)
{
    direccionDisparo = direccion.normalized;

    if (direccionDisparo == Vector2.zero)
    {
        direccionDisparo = Vector2.right;
    }

    // Rotar el sprite hacia el enemigo (sumando 180° por la orientación de tu imagen)
    float angulo = Mathf.Atan2(direccionDisparo.y, direccionDisparo.x) * Mathf.Rad2Deg + 180f;
    transform.rotation = Quaternion.Euler(0, 0, angulo);

    Destroy(gameObject, tiempoVida);
}
    void Update()
{
    // Mueve la lanza hacia adelante a lo largo de su eje X local (hacia donde apunta su rotación)
    transform.Translate(Vector3.right * velocidad * Time.deltaTime, Space.Self);
}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // Obtener el script Enemy del objeto colisionado
            Enemy enemigo = collision.GetComponent<Enemy>();
            if (enemigo != null)
            {
                enemigo.TakeDamage(dano);
                Debug.Log($"¡La Lanza de la Ceiba impactó a {collision.name} provocando {dano} de daño!");
            }
        }
    }
}   