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

    private TipoAsteroide tipoAsteroide = TipoAsteroide.Normal;
    private int golpesRestantes = 1;
    private bool formacionGenerada;
    private bool registradoEnSesion;

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
            asteroide.RegistrarEnSesion();

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
        }

        gameObject.SetActive(false);
    }

    private void RegistrarEnSesion()
    {
        registradoEnSesion = true;
        GameSession.Instancia.RegistrarAsteroide();
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

        if (sonidoExplosion != null)
        {
            AudioSource.PlayClipAtPoint(sonidoExplosion, transform.position, volumenExplosion);
        }

        GameSession.Instancia.SumarPuntos(ObtenerPuntos());
        GameSession.Instancia.AsteroideDestruido();
        Destroy(gameObject);
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
        float centroX = (areaMinima.x + areaMaxima.x) * 0.5f;
        float ancho = areaMaxima.x - areaMinima.x;
        float separacion = ancho / 4f;
        float x = centroX + (indice - 1) * separacion;
        float y = areaMaxima.y - 0.25f;

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
}
