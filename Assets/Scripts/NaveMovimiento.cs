using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NaveMovimiento : MonoBehaviour
{
    [SerializeField] private float velocidad = 7f;
    [SerializeField] private float limiteX = 8f;
    [SerializeField] private float distanciaDash = 3f;
    [SerializeField] private float recargaDash = 3f;
    [SerializeField] private float duracionDash = 0.12f;

    private Rigidbody2D rb;
    private float movimiento;
    private float tiempoUltimoDash = -999f;
    private Text textoDash;
    private bool dashActivo;
    private float tiempoDash;
    private Vector2 inicioDash;
    private Vector2 destinoDash;
    private Vector3 escalaOriginal;
    private Coroutine rutinaNaveGrande;
    private Image barraDash;
    private SpriteRenderer spriteRenderer;
    private Color colorOriginal = Color.white;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            colorOriginal = spriteRenderer.color;
        }
    }

    private void Start()
    {
        escalaOriginal = transform.localScale;
        CrearEstelaDash();
        CrearIndicadorDash();
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

        if ((Keyboard.current.leftShiftKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame) && DashDisponible())
        {
            AplicarDash();
        }

        ActualizarIndicadorDash();

        if (rb == null)
        {
            if (dashActivo)
            {
                ActualizarDash(Time.deltaTime);
            }
            else
            {
                MoverNave(Time.deltaTime);
            }
        }
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            if (dashActivo)
            {
                ActualizarDash(Time.fixedDeltaTime);
            }
            else
            {
                MoverNave(Time.fixedDeltaTime);
            }
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

    private bool DashDisponible()
    {
        return Time.time >= tiempoUltimoDash + recargaDash;
    }

    private void AplicarDash()
    {
        if (Mathf.Approximately(movimiento, 0f))
        {
            return;
        }

        inicioDash = rb != null ? rb.position : (Vector2)transform.position;
        destinoDash = inicioDash;
        destinoDash.x += Mathf.Sign(movimiento) * distanciaDash;
        destinoDash.x = Mathf.Clamp(destinoDash.x, -limiteX, limiteX);

        if (Mathf.Approximately(inicioDash.x, destinoDash.x))
        {
            return;
        }

        dashActivo = true;
        tiempoDash = 0f;
        tiempoUltimoDash = Time.time;
    }

    private void ActualizarDash(float deltaTime)
    {
        tiempoDash += deltaTime;
        float progreso = Mathf.Clamp01(tiempoDash / duracionDash);
        float suavizado = Mathf.SmoothStep(0f, 1f, progreso);
        Vector2 posicion = Vector2.Lerp(inicioDash, destinoDash, suavizado);

        if (rb != null)
        {
            rb.MovePosition(posicion);
        }
        else
        {
            transform.position = posicion;
        }

        if (progreso >= 1f)
        {
            dashActivo = false;
        }
    }

    private void CrearEstelaDash()
    {
        TrailRenderer estela = GetComponent<TrailRenderer>();

        if (estela == null)
        {
            estela = gameObject.AddComponent<TrailRenderer>();
        }

        estela.time = 0.18f;
        estela.startWidth = 0.65f;
        estela.endWidth = 0f;
        estela.sortingOrder = 9;
        estela.emitting = true;
        estela.autodestruct = false;
        Shader shader = Shader.Find("Sprites/Default");

        if (shader != null)
        {
            estela.material = new Material(shader);
        }

        estela.startColor = new Color(0.35f, 1f, 1f, 0.65f);
        estela.endColor = new Color(0.2f, 0.55f, 1f, 0f);
    }

    private void CrearIndicadorDash()
    {
        GameObject canvasObjeto = new GameObject("DashUI");

        Canvas canvas = canvasObjeto.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 101;

        CanvasScaler scaler = canvasObjeto.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObjeto.AddComponent<GraphicRaycaster>();

        GameObject textoObjeto = new GameObject("IndicadorDash");
        textoObjeto.transform.SetParent(canvasObjeto.transform, false);

        textoDash = textoObjeto.AddComponent<Text>();
        textoDash.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textoDash.fontSize = 30;
        textoDash.alignment = TextAnchor.UpperRight;
        textoDash.raycastTarget = false;

        RectTransform rect = textoDash.rectTransform;
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = new Vector2(-20f, -20f);
        rect.sizeDelta = new Vector2(360f, 70f);

        GameObject fondoBarraObjeto = new GameObject("FondoBarraDash");
        fondoBarraObjeto.transform.SetParent(canvasObjeto.transform, false);

        Image fondoBarra = fondoBarraObjeto.AddComponent<Image>();
        fondoBarra.color = new Color(0f, 0f, 0f, 0.45f);
        fondoBarra.raycastTarget = false;

        RectTransform rectFondo = fondoBarra.rectTransform;
        rectFondo.anchorMin = new Vector2(1f, 1f);
        rectFondo.anchorMax = new Vector2(1f, 1f);
        rectFondo.pivot = new Vector2(1f, 1f);
        rectFondo.anchoredPosition = new Vector2(-24f, -72f);
        rectFondo.sizeDelta = new Vector2(260f, 16f);

        GameObject barraObjeto = new GameObject("BarraDash");
        barraObjeto.transform.SetParent(fondoBarraObjeto.transform, false);

        barraDash = barraObjeto.AddComponent<Image>();
        barraDash.color = new Color(0.35f, 1f, 0.75f, 0.95f);
        barraDash.raycastTarget = false;
        barraDash.type = Image.Type.Filled;
        barraDash.fillMethod = Image.FillMethod.Horizontal;
        barraDash.fillOrigin = (int)Image.OriginHorizontal.Left;

        RectTransform rectBarra = barraDash.rectTransform;
        rectBarra.anchorMin = Vector2.zero;
        rectBarra.anchorMax = Vector2.one;
        rectBarra.offsetMin = new Vector2(2f, 2f);
        rectBarra.offsetMax = new Vector2(-2f, -2f);

        ActualizarIndicadorDash();
    }

    private void ActualizarIndicadorDash()
    {
        if (textoDash == null)
        {
            return;
        }

        if (DashDisponible())
        {
            textoDash.text = "Dash listo";
            textoDash.color = new Color(0.35f, 1f, 0.75f, 1f);
            ActualizarBarraDash(1f);
            return;
        }

        float tiempoRestante = tiempoUltimoDash + recargaDash - Time.time;
        textoDash.text = $"Dash: {tiempoRestante:0.0}s";
        textoDash.color = new Color(1f, 0.75f, 0.35f, 1f);
        ActualizarBarraDash(Mathf.Clamp01((recargaDash - tiempoRestante) / recargaDash));
    }

    private void ActualizarBarraDash(float progreso)
    {
        if (barraDash != null)
        {
            barraDash.fillAmount = progreso;
        }
    }

    public void Recentrar()
    {
        dashActivo = false;

        Vector2 posicion = rb != null ? rb.position : (Vector2)transform.position;
        posicion.x = 0f;

        if (rb != null)
        {
            rb.position = posicion;
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            transform.position = posicion;
        }
    }

    public void ActivarNaveGrande(float duracion, float multiplicador)
    {
        if (rutinaNaveGrande != null)
        {
            StopCoroutine(rutinaNaveGrande);
        }

        rutinaNaveGrande = StartCoroutine(NaveGrandeTemporal(duracion, multiplicador));
    }

    private IEnumerator NaveGrandeTemporal(float duracion, float multiplicador)
    {
        float tiempoEntrada = 0.45f;
        float tiempoSalida = 0.25f;
        float tiempo = 0f;
        Color colorBrillo = new Color(1f, 0.9f, 0.25f, 1f);

        while (tiempo < tiempoEntrada)
        {
            tiempo += Time.deltaTime;
            float progreso = Mathf.Clamp01(tiempo / tiempoEntrada);
            float pulso = 1f + Mathf.Sin(progreso * Mathf.PI) * 0.18f;
            float escalaX = Mathf.Lerp(escalaOriginal.x, escalaOriginal.x * multiplicador, progreso) * pulso;

            transform.localScale = new Vector3(escalaX, escalaOriginal.y, escalaOriginal.z);

            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.Lerp(colorOriginal, colorBrillo, Mathf.PingPong(tiempo * 8f, 1f));
            }

            yield return null;
        }

        transform.localScale = new Vector3(escalaOriginal.x * multiplicador, escalaOriginal.y, escalaOriginal.z);

        float tiempoActivo = Mathf.Max(0f, duracion - tiempoEntrada - tiempoSalida);
        tiempo = 0f;

        while (tiempo < tiempoActivo)
        {
            tiempo += Time.deltaTime;

            if (spriteRenderer != null)
            {
                float brillo = 0.18f + Mathf.PingPong(tiempo * 2.5f, 0.18f);
                spriteRenderer.color = Color.Lerp(colorOriginal, colorBrillo, brillo);
            }

            yield return null;
        }

        tiempo = 0f;

        while (tiempo < tiempoSalida)
        {
            tiempo += Time.deltaTime;
            float progreso = Mathf.Clamp01(tiempo / tiempoSalida);
            float escalaX = Mathf.Lerp(escalaOriginal.x * multiplicador, escalaOriginal.x, progreso);

            transform.localScale = new Vector3(escalaX, escalaOriginal.y, escalaOriginal.z);

            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.Lerp(colorBrillo, colorOriginal, progreso);
            }

            yield return null;
        }

        transform.localScale = escalaOriginal;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = colorOriginal;
        }

        rutinaNaveGrande = null;
    }
}
