using UnityEngine;

public class AuraMagnetica : MonoBehaviour
{
    [Header("Configuración Visual")]
    public Sprite imagenAureola; 
    public float tamanoAura = 3.5f;
    public float velocidadRotacion = 45f;

    [Header("Configuración de Atracción")]
    public float radioAtraccion = 3f;
    public float velocidadAtraccion = 10f;
    public string tagGemas = "GemaEXP"; 

    private GameObject auraVisualObj;
    private float anguloZ;

    void OnEnable()
    {
        if (auraVisualObj == null && imagenAureola != null)
        {
            auraVisualObj = new GameObject("AuraVisual_Generada");
            SpriteRenderer sr = auraVisualObj.AddComponent<SpriteRenderer>();
            sr.sprite = imagenAureola;
            sr.material = new Material(Shader.Find("Sprites/Default"));
            sr.sortingOrder = -1; 
        }
        if (auraVisualObj != null) auraVisualObj.SetActive(true);
    }

    void OnDisable()
    {
        if (auraVisualObj != null) auraVisualObj.SetActive(false);
    }

    void Update()
    {
        if (auraVisualObj != null)
        {
            auraVisualObj.transform.position = transform.position;
            anguloZ += velocidadRotacion * Time.deltaTime;
            auraVisualObj.transform.rotation = Quaternion.Euler(0, 0, anguloZ);
            
            float pulso = 1f + (Mathf.Sin(Time.time * 2f) * 0.08f);
            float escalaFinal = tamanoAura * pulso;
            auraVisualObj.transform.localScale = new Vector3(escalaFinal, escalaFinal, 1f);
        }

        Collider2D[] objetosCercanos = Physics2D.OverlapCircleAll(transform.position, radioAtraccion);
        foreach (Collider2D obj in objetosCercanos)
        {
            if (obj.CompareTag(tagGemas))
            {
                obj.transform.position = Vector3.MoveTowards(
                    obj.transform.position,
                    transform.position,
                    velocidadAtraccion * Time.deltaTime
                );
            }
        }
    }
}