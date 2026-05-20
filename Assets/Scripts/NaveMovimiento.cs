using UnityEngine;
using UnityEngine.InputSystem;

public class NaveMovimiento : MonoBehaviour
{
    [SerializeField] private float velocidad = 7f;
    [SerializeField] private float limiteX = 8f;

    private Rigidbody2D rb;
    private float movimiento;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        movimiento = 0f;

        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
        {
            movimiento -= 1f;
        }

        if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
        {
            movimiento += 1f;
        }

        if (rb == null)
        {
            MoverNave(Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            MoverNave(Time.fixedDeltaTime);
        }
    }

    private void MoverNave(float deltaTime)
    {
        Vector2 posicion = rb != null ? rb.position : (Vector2)transform.position;

        posicion.x += movimiento * velocidad * deltaTime;
        posicion.x = Mathf.Clamp(posicion.x, -limiteX, limiteX);

        if (rb != null)
        {
            rb.MovePosition(posicion);
        }
        else
        {
            transform.position = posicion;
        }
    }
}
