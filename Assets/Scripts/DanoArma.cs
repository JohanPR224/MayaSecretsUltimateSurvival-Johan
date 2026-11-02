using UnityEngine;

public class DanoArma : MonoBehaviour
{
    public string tagEnemigo = "Enemy"; 
    public float cantidadDano = 10f; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(tagEnemigo))
        {
            Enemy scriptEnemigo = collision.GetComponent<Enemy>();
            
            if (scriptEnemigo != null)
            {
                scriptEnemigo.TakeDamage(cantidadDano); 
            }
        }
    }
}