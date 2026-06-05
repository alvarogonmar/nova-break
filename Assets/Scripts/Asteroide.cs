using UnityEngine;

public class Asteroide : MonoBehaviour
{
    private enum TipoAsteroide
    {
        Normal,
        Especial,
        Resistente
    }

    [SerializeField] private bool generarFormacion;
    [SerializeField] private int cantidad = 32;
    [SerializeField] private Vector2 areaMinima = new Vector2(-7f, 1.8f);
    [SerializeField] private Vector2 areaMaxima = new Vector2(7f, 4.4f);
    [SerializeField] private Vector2 escalaAleatoria = new Vector2(0.08f, 0.14f);
    [SerializeField] private float distanciaMinima = 1.15f;
    [SerializeField] private AudioClip sonidoExplosion;
    [SerializeField] private float volumenExplosion = 0.8f;
    [SerializeField] private Sprite spriteEspecial;
    [SerializeField] private int cantidadEspeciales;
    [SerializeField] private float incrementoVelocidad = 2f;
    [SerializeField] private float incrementoVelocidadEspecial = 2f;
    [SerializeField] private float escalaEspecial = 0.18f;
    [SerializeField] private Sprite spriteResistente;
    [SerializeField] private int cantidadResistentes;
    [SerializeField] private int golpesResistente = 2;
    [SerializeField] private AudioClip sonidoGolpeResistente;
    [SerializeField] private float incrementoVelocidadResistente = 0.65f;
    [SerializeField] private float escalaResistente = 0.15f;
    [SerializeField] private int puntosNormal = 100;
    [SerializeField] private int puntosEspecial = 250;
    [SerializeField] private int puntosResistente = 400;
    [SerializeField] private float probabilidadPowerUp = 0.12f;
    [SerializeField] private float probabilidadExplosion = 0.4f;
    [SerializeField] private Sprite spritePowerUpNaveGrande;
    [SerializeField] private Sprite spritePowerUpExplosion;
    [SerializeField] private AudioClip sonidoPowerUp;
    [SerializeField] private bool animacionSuave = true;
    [SerializeField] private Vector2 velocidadRotacion = new Vector2(-18f, 18f);
    [SerializeField] private float intensidadPulso = 0.035f;
    [SerializeField] private float velocidadPulso = 1.8f;
    [SerializeField] private bool ajustarHitboxAlSprite = true;
    [SerializeField] private float margenHitbox = 0.92f;

    private TipoAsteroide tipoAsteroide = TipoAsteroide.Normal;
    private int golpesRestantes = 1;
    private bool formacionGenerada;
    private bool registradoEnSesion;
    private Vector3 escalaBase;
    private float rotacionActual;
    private float velocidadRotacionActual;
    private float fasePulso;

    private void Start()
    {
        GameSession.Asegurar();

        if (!generarFormacion || formacionGenerada)
        {
            if (!generarFormacion && !registradoEnSesion)
            {
                RegistrarEnSesion();
            }

            return;
        }

        formacionGenerada = true;

        for (int i = 0; i < cantidad; i++)
        {
            bool seraEspecial = i < cantidadEspeciales;
            bool seraResistente = i >= cantidadEspeciales && i < cantidadEspeciales + cantidadResistentes;
            Vector2 posicion = seraEspecial ? ObtenerPosicionEspecial(i) : ObtenerPosicionAleatoria();
            Quaternion rotacion = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            Asteroide asteroide = Instantiate(this, posicion, rotacion);
            float escala = Random.Range(escalaAleatoria.x, escalaAleatoria.y);

            asteroide.transform.localScale = new Vector3(escala, escala, 1f);
            asteroide.generarFormacion = false;
            asteroide.name = $"Asteroide_{i + 1}";

            if (seraEspecial)
            {
                asteroide.transform.localScale = Vector3.one * escalaEspecial;
                asteroide.name = $"Asteroide_B_{i + 1}";
                asteroide.ConvertirEnEspecial();
            }
            else if (seraResistente)
            {
                asteroide.transform.localScale = Vector3.one * escalaResistente;
                asteroide.name = $"Asteroide_R_{i + 1}";
                asteroide.ConvertirEnResistente();
            }

            asteroide.PrepararAsteroideInstanciado();
        }

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!animacionSuave || generarFormacion || escalaBase == Vector3.zero)
        {
            return;
        }

        rotacionActual += velocidadRotacionActual * Time.deltaTime;
        float pulso = 1f + Mathf.Sin(Time.time * velocidadPulso + fasePulso) * intensidadPulso;
        transform.localRotation = Quaternion.Euler(0f, 0f, rotacionActual);
        transform.localScale = escalaBase * pulso;
    }

    private void RegistrarEnSesion()
    {
        registradoEnSesion = true;
        GameSession.Instancia.RegistrarAsteroide();
    }

    private void PrepararAsteroideInstanciado()
    {
        escalaBase = transform.localScale;
        rotacionActual = transform.eulerAngles.z;
        velocidadRotacionActual = Random.Range(velocidadRotacion.x, velocidadRotacion.y);
        fasePulso = Random.Range(0f, Mathf.PI * 2f);
        AjustarHitboxAlSprite();
        RegistrarEnSesion();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Orbe"))
        {
            return;
        }

        if (tipoAsteroide == TipoAsteroide.Resistente)
        {
            golpesRestantes--;

            if (golpesRestantes > 0)
            {
                if (sonidoGolpeResistente != null)
                {
                    AudioSource.PlayClipAtPoint(sonidoGolpeResistente, transform.position, volumenExplosion);
                }

                return;
            }
        }

        DestruirAsteroide(true);
    }

    public void DestruirPorPowerUp()
    {
        DestruirAsteroide(false);
    }

    public float ObtenerIncrementoVelocidad()
    {
        if (tipoAsteroide == TipoAsteroide.Especial)
        {
            return incrementoVelocidadEspecial;
        }

        if (tipoAsteroide == TipoAsteroide.Resistente)
        {
            return incrementoVelocidadResistente;
        }

        return incrementoVelocidad;
    }

    private Vector2 ObtenerPosicionAleatoria()
    {
        const int intentosMaximos = 30;

        for (int intento = 0; intento < intentosMaximos; intento++)
        {
            Vector2 posicion = new Vector2(
                Random.Range(areaMinima.x, areaMaxima.x),
                Random.Range(areaMinima.y, areaMaxima.y)
            );

            if (HayEspacioLibre(posicion))
            {
                return posicion;
            }
        }

        return new Vector2(
            Random.Range(areaMinima.x, areaMaxima.x),
            Random.Range(areaMinima.y, areaMaxima.y)
        );
    }

    private Vector2 ObtenerPosicionEspecial(int indice)
    {
        float margenHorizontal = 0.8f;
        float cantidad = Mathf.Max(1, cantidadEspeciales);
        float progreso = (indice + 0.5f) / cantidad;
        float x = Mathf.Lerp(areaMinima.x + margenHorizontal, areaMaxima.x - margenHorizontal, progreso);
        float y = areaMaxima.y - 0.75f;

        return new Vector2(x, y);
    }

    private bool HayEspacioLibre(Vector2 posicion)
    {
        Collider2D[] cercanos = Physics2D.OverlapCircleAll(posicion, distanciaMinima);

        foreach (Collider2D cercano in cercanos)
        {
            if (cercano.CompareTag("Asteroide"))
            {
                return false;
            }
        }

        return true;
    }

    private void ConvertirEnEspecial()
    {
        if (spriteEspecial == null)
        {
            return;
        }

        tipoAsteroide = TipoAsteroide.Especial;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = spriteEspecial;
            AjustarHitboxAlSprite();
        }
    }

    private void ConvertirEnResistente()
    {
        tipoAsteroide = TipoAsteroide.Resistente;
        golpesRestantes = Mathf.Max(1, golpesResistente);

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null && spriteResistente != null)
        {
            spriteRenderer.sprite = spriteResistente;
            AjustarHitboxAlSprite();
        }
    }

    private int ObtenerPuntos()
    {
        if (tipoAsteroide == TipoAsteroide.Especial)
        {
            return puntosEspecial;
        }

        if (tipoAsteroide == TipoAsteroide.Resistente)
        {
            return puntosResistente;
        }

        return puntosNormal;
    }

    private void DestruirAsteroide(bool puedeSoltarPowerUp)
    {
        if (sonidoExplosion != null)
        {
            AudioSource.PlayClipAtPoint(sonidoExplosion, transform.position, volumenExplosion);
        }

        if (puedeSoltarPowerUp)
        {
            IntentarCrearPowerUp();
        }

        GameSession.Instancia.SumarPuntos(ObtenerPuntos());
        GameSession.Instancia.AsteroideDestruido();
        Destroy(gameObject);
    }

    private void IntentarCrearPowerUp()
    {
        if (Random.value > probabilidadPowerUp)
        {
            return;
        }

        bool seraExplosion = Random.value < probabilidadExplosion;
        PowerUp.TipoPowerUp tipo = seraExplosion ? PowerUp.TipoPowerUp.Explosion : PowerUp.TipoPowerUp.NaveGrande;
        Sprite sprite = seraExplosion ? spritePowerUpExplosion : spritePowerUpNaveGrande;

        PowerUp.Crear(tipo, transform.position, sprite, sonidoPowerUp);
    }

    private void AjustarHitboxAlSprite()
    {
        if (!ajustarHitboxAlSprite)
        {
            return;
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null || spriteRenderer.sprite == null)
        {
            return;
        }

        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();

        if (circleCollider != null)
        {
            Bounds bounds = spriteRenderer.sprite.bounds;
            circleCollider.offset = bounds.center;
            circleCollider.radius = Mathf.Max(bounds.extents.x, bounds.extents.y) * margenHitbox;
            return;
        }

        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();

        if (boxCollider != null)
        {
            Bounds bounds = spriteRenderer.sprite.bounds;
            boxCollider.offset = bounds.center;
            boxCollider.size = bounds.size * margenHitbox;
        }
    }
}
