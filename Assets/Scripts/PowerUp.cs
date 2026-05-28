using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum TipoPowerUp
    {
        NaveGrande,
        Explosion
    }

    [SerializeField] private TipoPowerUp tipo;
    [SerializeField] private float velocidadCaida = 2.5f;
    [SerializeField] private float duracionNaveGrande = 8f;
    [SerializeField] private float escalaNaveGrande = 1.8f;
    [SerializeField] private float radioExplosion = 3.5f;
    [SerializeField] private AudioClip sonidoRecoger;
    [SerializeField] private float volumenSonido = 0.7f;
    [SerializeField] private float limiteInferior = -6f;

    public static void Crear(TipoPowerUp tipo, Vector3 posicion, Sprite sprite, AudioClip sonidoRecoger)
    {
        if (sprite == null)
        {
            return;
        }

        GameObject objeto = new GameObject($"PowerUp_{tipo}");
        objeto.transform.position = posicion;
        objeto.transform.localScale = Vector3.one * 0.22f;

        SpriteRenderer spriteRenderer = objeto.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 25;
        spriteRenderer.color = tipo == TipoPowerUp.Explosion
            ? new Color(1f, 0.65f, 0.2f, 1f)
            : new Color(0.45f, 1f, 0.85f, 1f);

        Rigidbody2D rb = objeto.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        CircleCollider2D collider = objeto.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 1.5f;

        PowerUp powerUp = objeto.AddComponent<PowerUp>();
        powerUp.tipo = tipo;
        powerUp.velocidadCaida = 2.5f;
        powerUp.sonidoRecoger = sonidoRecoger;
        rb.linearVelocity = Vector2.down * powerUp.velocidadCaida;
    }

    private void Update()
    {
        if (transform.position.y < limiteInferior)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        NaveMovimiento nave = collision.GetComponent<NaveMovimiento>();

        if (nave == null)
        {
            nave = collision.GetComponentInParent<NaveMovimiento>();
        }

        if (nave == null && !collision.CompareTag("Nave"))
        {
            return;
        }

        ReproducirSonido();

        if (tipo == TipoPowerUp.NaveGrande)
        {
            GameSession.Instancia.MostrarAvisoTemporal("Power-up: nave grande");

            if (nave != null)
            {
                nave.ActivarNaveGrande(duracionNaveGrande, escalaNaveGrande);
            }
        }
        else
        {
            GameSession.Instancia.MostrarAvisoTemporal("Power-up: explosion");
            ExplotarAsteroidesCercanos();
        }

        Destroy(gameObject);
    }

    private void ExplotarAsteroidesCercanos()
    {
        Vector3 centroExplosion = transform.position;
        GameObject orbe = GameObject.FindGameObjectWithTag("Orbe");

        if (orbe != null)
        {
            centroExplosion = orbe.transform.position;
        }

        GameSession.Instancia.MostrarExplosionVisual(centroExplosion, radioExplosion);

        Collider2D[] cercanos = Physics2D.OverlapCircleAll(centroExplosion, radioExplosion);

        foreach (Collider2D cercano in cercanos)
        {
            Asteroide asteroide = cercano.GetComponent<Asteroide>();

            if (asteroide != null)
            {
                asteroide.DestruirPorPowerUp();
            }
        }
    }

    private void ReproducirSonido()
    {
        if (sonidoRecoger != null)
        {
            AudioSource.PlayClipAtPoint(sonidoRecoger, transform.position, volumenSonido);
        }
    }
}
