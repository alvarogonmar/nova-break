using System.Collections;
using UnityEngine;

public class OrbeMovimiento : MonoBehaviour
{
    [SerializeField] private float velocidad = 6f;
    [SerializeField] private float velocidadMaxima = 13f;
    [SerializeField] private float maxAnguloRebote = 65f;
    [SerializeField] private Vector2 posicionInicio = new Vector2(0f, 1f);
    [SerializeField] private float esperaReinicio = 0.8f;
    [SerializeField] private float minimoMovimientoVertical = 0.35f;

    private Rigidbody2D rb;
    private float velocidadInicial;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        velocidadInicial = velocidad;
    }

    private void Start()
    {
        ReiniciarOrbe();
    }

    private void FixedUpdate()
    {
        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            AjustarVelocidad(rb.linearVelocity);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Asteroide asteroide = collision.gameObject.GetComponent<Asteroide>();

        if (asteroide != null)
        {
            AumentarVelocidad(asteroide.ObtenerIncrementoVelocidad());
            return;
        }

        if (!collision.gameObject.CompareTag("Nave"))
        {
            MantenerVelocidad();
            return;
        }

        Bounds naveBounds = collision.collider.bounds;
        float diferenciaX = transform.position.x - naveBounds.center.x;
        float mitadNave = naveBounds.extents.x;
        float factorRebote = Mathf.Clamp(diferenciaX / mitadNave, -1f, 1f);
        float angulo = factorRebote * maxAnguloRebote;

        Vector2 direccion = Quaternion.Euler(0f, 0f, -angulo) * Vector2.up;
        AjustarVelocidad(direccion);
    }

    public void ReiniciarOrbe()
    {
        StopAllCoroutines();
        velocidad = velocidadInicial;
        StartCoroutine(ReiniciarDespuesDePausa());
    }

    public void AumentarVelocidad(float incremento)
    {
        velocidad = Mathf.Min(velocidad + incremento, velocidadMaxima);

        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            AjustarVelocidad(rb.linearVelocity);
        }
    }

    private IEnumerator ReiniciarDespuesDePausa()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.position = posicionInicio;

        yield return new WaitForSeconds(esperaReinicio);

        rb.linearVelocity = Vector2.down * velocidad;
    }

    private void MantenerVelocidad()
    {
        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            AjustarVelocidad(rb.linearVelocity);
        }
    }

    private void AjustarVelocidad(Vector2 direccion)
    {
        if (direccion.sqrMagnitude <= 0.01f)
        {
            return;
        }

        direccion = direccion.normalized;

        if (Mathf.Abs(direccion.y) < minimoMovimientoVertical)
        {
            float signoY = direccion.y >= 0f ? 1f : -1f;
            direccion.y = signoY * minimoMovimientoVertical;
            direccion.x = Mathf.Sign(direccion.x) * Mathf.Sqrt(1f - minimoMovimientoVertical * minimoMovimientoVertical);
            direccion.Normalize();
        }

        rb.linearVelocity = direccion * velocidad;
    }
}
