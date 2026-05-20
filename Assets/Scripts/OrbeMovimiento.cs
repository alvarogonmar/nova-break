using System.Collections;
using UnityEngine;

public class OrbeMovimiento : MonoBehaviour
{
    [SerializeField] private float velocidad = 6f;
    [SerializeField] private float maxAnguloRebote = 65f;
    [SerializeField] private Vector2 posicionInicio = new Vector2(0f, 1f);
    [SerializeField] private float esperaReinicio = 0.8f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        ReiniciarOrbe();
    }

    private void FixedUpdate()
    {
        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * velocidad;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
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
        rb.linearVelocity = direccion.normalized * velocidad;
    }

    public void ReiniciarOrbe()
    {
        StopAllCoroutines();
        StartCoroutine(ReiniciarDespuesDePausa());
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
            rb.linearVelocity = rb.linearVelocity.normalized * velocidad;
        }
    }
}
