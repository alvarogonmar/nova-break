using UnityEngine;

public class ZonaMuerte : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Orbe"))
        {
            Debug.Log("Perdiste una vida");
            bool quedanVidas = GameSession.Instancia.PerderVida();
            OrbeMovimiento orbe = collision.GetComponent<OrbeMovimiento>();

            if (orbe != null && quedanVidas)
            {
                orbe.ReiniciarOrbe();
            }
        }
    }
}
