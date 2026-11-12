using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float x = 0f;
        float y = 0f;

        // 1. Si venimos del MainMenu, lee las teclas personalizadas de ControlManager
        if (ControlManager.Instance != null)
        {
            if (Input.GetKey(ControlManager.Instance.Arriba)) y = 1f;
            if (Input.GetKey(ControlManager.Instance.Abajo)) y = -1f;
            if (Input.GetKey(ControlManager.Instance.Izquierda)) x = -1f;
            if (Input.GetKey(ControlManager.Instance.Derecha)) x = 1f;
        }
        // 2. Respaldo por si pruebas la escena del nivel directamente sin pasar por MainMenu
        else
        {
            x = Input.GetAxisRaw("Horizontal");
            y = Input.GetAxisRaw("Vertical");
        }

        moveInput = new Vector2(x, y).normalized;
    }

    void FixedUpdate()
    {
        // Mueve el cuerpo con físicas constantes (compatible con Unity 6)
        rb.linearVelocity = moveInput * moveSpeed;
    }
}   