using UnityEngine;
using UnityEngine.InputSystem;

public class NaveMovimiento : MonoBehaviour
{
    [SerializeField] private float velocidad = 7f;
    [SerializeField] private float limiteX = 8f;

    private void Update()
    {
        float movimiento = 0f;

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

        Vector3 posicion = transform.position;

        posicion.x += movimiento * velocidad * Time.deltaTime;
        posicion.x = Mathf.Clamp(posicion.x, -limiteX, limiteX);

        transform.position = posicion;
    }
}
