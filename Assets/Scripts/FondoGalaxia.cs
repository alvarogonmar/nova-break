using UnityEngine;

public class FondoGalaxia : MonoBehaviour
{
    [SerializeField] private float tasaEmision = 80f;
    [SerializeField] private float velocidadEstrellas = 3f;
    [SerializeField] private float tiempoVida = 2.4f;
    [SerializeField] private float radioOrigen = 0.8f;
    [SerializeField] private Vector2 tamanoEstrella = new Vector2(0.035f, 0.08f);
    [SerializeField] private int maxParticulas = 700;
    [SerializeField] private int sortingOrder = -9;
    [SerializeField] private Color colorPrincipal = new Color(0.75f, 0.9f, 1f, 0.9f);
    [SerializeField] private Color colorSecundario = new Color(0.85f, 0.45f, 1f, 0.75f);
    [SerializeField] private float aperturaVertical = 0.65f;
    [SerializeField] private float precargaSegundos = 2f;

    private ParticleSystem sistema;
    private float acumuladorEmision;

    private void Awake()
    {
        CrearSistemaParticulas();
        PrecargarEstrellas();
    }

    private void Update()
    {
        EmitirEstrellas();
    }

    private void CrearSistemaParticulas()
    {
        GameObject objetoParticulas = new GameObject("EfectoGalaxia");
        objetoParticulas.transform.SetParent(transform, false);
        objetoParticulas.transform.localPosition = Vector3.zero;

        sistema = objetoParticulas.AddComponent<ParticleSystem>();

        ParticleSystem.MainModule main = sistema.main;
        main.loop = false;
        main.playOnAwake = false;
        main.maxParticles = maxParticulas;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.startLifetime = tiempoVida;
        main.startSpeed = 0f;
        main.startSize = tamanoEstrella.x;
        main.gravityModifier = 0f;

        ParticleSystem.EmissionModule emission = sistema.emission;
        emission.enabled = false;

        ParticleSystem.ShapeModule shape = sistema.shape;
        shape.enabled = false;

        ParticleSystemRenderer rendererParticulas = sistema.GetComponent<ParticleSystemRenderer>();
        rendererParticulas.renderMode = ParticleSystemRenderMode.Billboard;
        rendererParticulas.lengthScale = 1f;
        rendererParticulas.velocityScale = 0f;
        rendererParticulas.sortingOrder = sortingOrder;

        Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if (shader == null)
        {
            shader = Shader.Find("Sprites/Default");
        }

        if (shader != null)
        {
            rendererParticulas.material = new Material(shader);
        }

        sistema.Play();
    }

    private void EmitirEstrellas()
    {
        if (sistema == null)
        {
            return;
        }

        acumuladorEmision += tasaEmision * Time.deltaTime;
        int cantidad = Mathf.FloorToInt(acumuladorEmision);
        acumuladorEmision -= cantidad;

        for (int i = 0; i < cantidad; i++)
        {
            EmitirEstrella();
        }
    }

    private void EmitirEstrella()
    {
        float angulo = Random.Range(0f, Mathf.PI * 2f);
        Vector3 direccion = new Vector3(Mathf.Cos(angulo), Mathf.Sin(angulo) * aperturaVertical, 0f).normalized;
        float distanciaInicial = Random.Range(radioOrigen * 0.55f, radioOrigen);
        Vector3 posicion = direccion * distanciaInicial;
        float velocidad = velocidadEstrellas * Random.Range(0.65f, 1.15f);
        Color color = Color.Lerp(colorPrincipal, colorSecundario, Random.value);
        color.a *= Random.Range(0.35f, 0.8f);

        ParticleSystem.EmitParams parametros = new ParticleSystem.EmitParams
        {
            position = posicion,
            velocity = direccion * velocidad,
            startLifetime = tiempoVida * Random.Range(0.9f, 1.25f),
            startSize = Random.Range(tamanoEstrella.x, tamanoEstrella.y),
            startColor = color
        };

        sistema.Emit(parametros, 1);
    }

    private void PrecargarEstrellas()
    {
        if (sistema == null || precargaSegundos <= 0f)
        {
            return;
        }

        int cantidadInicial = Mathf.RoundToInt(tasaEmision * precargaSegundos);

        for (int i = 0; i < cantidadInicial; i++)
        {
            EmitirEstrellaPrecargada();
        }
    }

    private void EmitirEstrellaPrecargada()
    {
        float angulo = Random.Range(0f, Mathf.PI * 2f);
        Vector3 direccion = new Vector3(Mathf.Cos(angulo), Mathf.Sin(angulo) * aperturaVertical, 0f).normalized;
        float progreso = Random.Range(0.15f, 1f);
        float edad = tiempoVida * progreso;
        float distanciaInicial = Random.Range(radioOrigen * 0.55f, radioOrigen);
        float velocidad = velocidadEstrellas * Random.Range(0.65f, 1.15f);
        Vector3 posicion = direccion * (distanciaInicial + velocidad * edad);
        Color color = Color.Lerp(colorPrincipal, colorSecundario, Random.value);
        color.a *= Random.Range(0.35f, 0.8f) * (1f - progreso * 0.35f);

        ParticleSystem.EmitParams parametros = new ParticleSystem.EmitParams
        {
            position = posicion,
            velocity = direccion * velocidad,
            startLifetime = tiempoVida * Random.Range(0.9f, 1.25f),
            startSize = Random.Range(tamanoEstrella.x, tamanoEstrella.y),
            startColor = color
        };

        sistema.Emit(parametros, 1);
    }
}
