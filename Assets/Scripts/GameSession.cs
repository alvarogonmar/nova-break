using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSession : MonoBehaviour
{
    private const int VidasIniciales = 5;

    private static GameSession instancia;

    private int vidas = VidasIniciales;
    private int puntos;
    private float tiempoPartida;
    private int asteroidesActivos;
    private int asteroidesRegistrados;
    private bool cambiandoNivel;
    private bool verificandoFinNivel;
    private bool partidaTerminada;

    private Text textoEstado;
    private Text textoMensaje;
    private Text textoAviso;
    private Button botonReiniciar;
    private Button botonMenu;
    private readonly List<Image> iconosVida = new List<Image>();
    private Sprite spriteVida;
    private Coroutine rutinaAviso;

    public static GameSession Instancia
    {
        get
        {
            if (instancia == null)
            {
                GameObject objeto = new GameObject("GameSession");
                instancia = objeto.AddComponent<GameSession>();
            }

            return instancia;
        }
    }

    private void Awake()
    {
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        CrearUI();
    }

    private void OnDestroy()
    {
        if (instancia == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void Update()
    {
        if (partidaTerminada)
        {
            return;
        }

        tiempoPartida += Time.deltaTime;
        ActualizarUI();
    }

    public static void Asegurar()
    {
        _ = Instancia;
    }

    public void RegistrarAsteroide()
    {
        asteroidesActivos++;
        asteroidesRegistrados++;
    }

    public void AsteroideDestruido()
    {
        asteroidesActivos = Mathf.Max(0, asteroidesActivos - 1);

        if (cambiandoNivel || verificandoFinNivel)
        {
            return;
        }

        StartCoroutine(VerificarFinNivelDespuesDeDestruir());
    }

    private IEnumerator VerificarFinNivelDespuesDeDestruir()
    {
        verificandoFinNivel = true;

        yield return null;

        if (cambiandoNivel)
        {
            verificandoFinNivel = false;
            yield break;
        }

        if (QuedanAsteroidesEnEscena())
        {
            verificandoFinNivel = false;
            yield break;
        }

        string escenaActual = SceneManager.GetActiveScene().name;

        if (escenaActual == "Nivel1")
        {
            StartCoroutine(CambiarEscenaConMensaje("Has pasado al nivel 2", "Nivel2"));
        }
        else if (escenaActual == "Nivel2")
        {
            StartCoroutine(CambiarEscenaConMensaje("Has pasado al nivel 3", "Nivel3"));
        }
        else if (escenaActual == "Nivel3")
        {
            VictoriaFinal();
        }

        verificandoFinNivel = false;
    }

    private bool QuedanAsteroidesEnEscena()
    {
        Asteroide[] asteroides = FindObjectsByType<Asteroide>(FindObjectsSortMode.None);

        foreach (Asteroide asteroide in asteroides)
        {
            if (asteroide.gameObject.activeInHierarchy)
            {
                return true;
            }
        }

        return false;
    }

    public void SumarPuntos(int cantidad)
    {
        puntos += Mathf.Max(0, cantidad);
        ActualizarUI();
    }

    public bool PerderVida()
    {
        if (partidaTerminada)
        {
            return false;
        }

        vidas = Mathf.Max(0, vidas - 1);
        ActualizarUI();

        if (vidas == 0)
        {
            partidaTerminada = true;
            MostrarMensaje(CrearResumenFinal("Game Over"));
            MostrarBotonesFinales(true);
            return false;
        }

        return true;
    }

    public void ReiniciarJuego()
    {
        ReiniciarEstado();
        SceneManager.LoadScene("Nivel1");
    }

    public void VolverAlMenu()
    {
        ReiniciarEstado();
        SceneManager.LoadScene("Menu");
    }

    private IEnumerator CambiarEscenaConMensaje(string mensaje, string escenaDestino)
    {
        cambiandoNivel = true;
        MostrarMensaje(mensaje);

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(escenaDestino);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        asteroidesActivos = 0;
        asteroidesRegistrados = 0;
        cambiandoNivel = false;
        verificandoFinNivel = false;
        partidaTerminada = false;
        CrearUI();
        OcultarMensaje();
        OcultarBotonesFinales();
        ActualizarUI();
    }

    private void CrearUI()
    {
        if (textoEstado != null && textoMensaje != null)
        {
            return;
        }

        GameObject canvasObjeto = new GameObject("GameSessionUI");
        DontDestroyOnLoad(canvasObjeto);

        Canvas canvas = canvasObjeto.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObjeto.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObjeto.AddComponent<GraphicRaycaster>();

        textoEstado = CrearTexto("Estado", canvasObjeto.transform, new Vector2(20f, -20f), TextAnchor.UpperLeft, 34);
        textoEstado.rectTransform.sizeDelta = new Vector2(520f, 90f);

        CrearIconosVidas(canvasObjeto.transform);

        textoMensaje = CrearTexto("Mensaje", canvasObjeto.transform, Vector2.zero, TextAnchor.MiddleCenter, 54);
        textoMensaje.rectTransform.anchorMin = new Vector2(0f, 0f);
        textoMensaje.rectTransform.anchorMax = new Vector2(1f, 1f);
        textoMensaje.rectTransform.offsetMin = Vector2.zero;
        textoMensaje.rectTransform.offsetMax = Vector2.zero;
        textoMensaje.gameObject.SetActive(false);

        textoAviso = CrearTexto("Aviso", canvasObjeto.transform, Vector2.zero, TextAnchor.MiddleCenter, 42);
        textoAviso.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        textoAviso.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        textoAviso.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        textoAviso.rectTransform.anchoredPosition = new Vector2(0f, 170f);
        textoAviso.rectTransform.sizeDelta = new Vector2(680f, 80f);
        textoAviso.gameObject.SetActive(false);

        botonReiniciar = CrearBoton("BotonReiniciar", canvasObjeto.transform, new Vector2(-130f, -120f), "Reiniciar");
        botonReiniciar.onClick.AddListener(ReiniciarJuego);

        botonMenu = CrearBoton("BotonMenu", canvasObjeto.transform, new Vector2(130f, -120f), "Menu principal");
        botonMenu.onClick.AddListener(VolverAlMenu);

        OcultarBotonesFinales();
    }

    private Text CrearTexto(string nombre, Transform padre, Vector2 posicion, TextAnchor alineacion, int tamano)
    {
        GameObject objeto = new GameObject(nombre);
        objeto.transform.SetParent(padre, false);

        Text texto = objeto.AddComponent<Text>();
        texto.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        texto.fontSize = tamano;
        texto.color = Color.white;
        texto.alignment = alineacion;
        texto.raycastTarget = false;

        RectTransform rect = texto.rectTransform;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = posicion;
        rect.sizeDelta = new Vector2(520f, 130f);

        return texto;
    }

    private Button CrearBoton(string nombre, Transform padre, Vector2 posicion, string etiqueta)
    {
        GameObject objeto = new GameObject(nombre);
        objeto.transform.SetParent(padre, false);

        Image imagen = objeto.AddComponent<Image>();
        imagen.color = new Color(0.08f, 0.16f, 0.28f, 0.92f);

        Button boton = objeto.AddComponent<Button>();
        ColorBlock colores = boton.colors;
        colores.normalColor = imagen.color;
        colores.highlightedColor = new Color(0.16f, 0.32f, 0.55f, 0.95f);
        colores.pressedColor = new Color(0.05f, 0.1f, 0.2f, 1f);
        boton.colors = colores;

        RectTransform rect = objeto.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = posicion;
        rect.sizeDelta = new Vector2(220f, 64f);

        Text texto = CrearTexto($"{nombre}Texto", objeto.transform, Vector2.zero, TextAnchor.MiddleCenter, 28);
        texto.text = etiqueta;
        texto.rectTransform.anchorMin = Vector2.zero;
        texto.rectTransform.anchorMax = Vector2.one;
        texto.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        texto.rectTransform.anchoredPosition = Vector2.zero;
        texto.rectTransform.offsetMin = Vector2.zero;
        texto.rectTransform.offsetMax = Vector2.zero;

        return boton;
    }

    private void CrearIconosVidas(Transform padre)
    {
        GameObject contenedor = new GameObject("VidasIconos");
        contenedor.transform.SetParent(padre, false);

        RectTransform rectContenedor = contenedor.AddComponent<RectTransform>();
        rectContenedor.anchorMin = new Vector2(0f, 1f);
        rectContenedor.anchorMax = new Vector2(0f, 1f);
        rectContenedor.pivot = new Vector2(0f, 1f);
        rectContenedor.anchoredPosition = new Vector2(20f, -112f);
        rectContenedor.sizeDelta = new Vector2(320f, 50f);

        spriteVida = CrearSpriteVida();
        iconosVida.Clear();

        for (int i = 0; i < VidasIniciales; i++)
        {
            GameObject iconoObjeto = new GameObject($"Vida_{i + 1}");
            iconoObjeto.transform.SetParent(contenedor.transform, false);

            Image imagen = iconoObjeto.AddComponent<Image>();
            imagen.sprite = spriteVida;
            imagen.color = new Color(0.25f, 1f, 1f, 1f);
            imagen.raycastTarget = false;

            RectTransform rect = imagen.rectTransform;
            rect.anchorMin = new Vector2(0f, 0.5f);
            rect.anchorMax = new Vector2(0f, 0.5f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.anchoredPosition = new Vector2(i * 42f, 0f);
            rect.sizeDelta = new Vector2(34f, 34f);

            iconosVida.Add(imagen);
        }
    }

    private Sprite CrearSpriteVida()
    {
        const int tamano = 32;
        Texture2D textura = new Texture2D(tamano, tamano, TextureFormat.RGBA32, false);
        textura.filterMode = FilterMode.Point;

        for (int y = 0; y < tamano; y++)
        {
            for (int x = 0; x < tamano; x++)
            {
                float nx = (x - tamano * 0.5f) / (tamano * 0.5f);
                float ny = (y - tamano * 0.42f) / (tamano * 0.5f);
                float valor = Mathf.Pow(nx * nx + ny * ny - 0.32f, 3f) - nx * nx * ny * ny * ny;
                textura.SetPixel(x, y, valor <= 0f ? Color.white : Color.clear);
            }
        }

        textura.Apply();
        return Sprite.Create(textura, new Rect(0f, 0f, tamano, tamano), new Vector2(0.5f, 0.5f), tamano);
    }

    private void ActualizarUI()
    {
        if (textoEstado == null)
        {
            return;
        }

        int minutos = Mathf.FloorToInt(tiempoPartida / 60f);
        int segundos = Mathf.FloorToInt(tiempoPartida % 60f);
        textoEstado.text = $"Tiempo: {minutos:00}:{segundos:00}\nPuntos: {puntos}";
        ActualizarIconosVidas();
    }

    private void ActualizarIconosVidas()
    {
        for (int i = 0; i < iconosVida.Count; i++)
        {
            iconosVida[i].enabled = i < vidas;
        }
    }

    public void MostrarAvisoTemporal(string mensaje)
    {
        if (textoAviso == null)
        {
            return;
        }

        if (rutinaAviso != null)
        {
            StopCoroutine(rutinaAviso);
        }

        rutinaAviso = StartCoroutine(MostrarAvisoCoroutine(mensaje));
    }

    private IEnumerator MostrarAvisoCoroutine(string mensaje)
    {
        textoAviso.text = mensaje;
        textoAviso.color = new Color(0.55f, 1f, 0.9f, 1f);
        textoAviso.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.2f);

        textoAviso.gameObject.SetActive(false);
        rutinaAviso = null;
    }

    public void MostrarExplosionVisual(Vector3 posicion, float radio)
    {
        StartCoroutine(MostrarExplosionVisualCoroutine(posicion, radio));
    }

    private IEnumerator MostrarExplosionVisualCoroutine(Vector3 posicion, float radio)
    {
        GameObject objeto = new GameObject("ExplosionPowerUpVisual");
        LineRenderer linea = objeto.AddComponent<LineRenderer>();
        linea.useWorldSpace = false;
        linea.loop = true;
        linea.positionCount = 72;
        linea.startWidth = 0.08f;
        linea.endWidth = 0.08f;
        linea.sortingOrder = 45;

        Shader shader = Shader.Find("Sprites/Default");

        if (shader != null)
        {
            linea.material = new Material(shader);
        }

        objeto.transform.position = posicion;

        for (int i = 0; i < linea.positionCount; i++)
        {
            float angulo = i / (float)linea.positionCount * Mathf.PI * 2f;
            Vector3 punto = new Vector3(Mathf.Cos(angulo) * radio, Mathf.Sin(angulo) * radio, 0f);
            linea.SetPosition(i, punto);
        }

        float duracion = 0.35f;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float progreso = tiempo / duracion;
            Color color = new Color(1f, 0.75f, 0.15f, 1f - progreso);
            linea.startColor = color;
            linea.endColor = color;
            objeto.transform.localScale = Vector3.one * Mathf.Lerp(0.65f, 1.15f, progreso);
            yield return null;
        }

        Destroy(objeto);
    }

    private void MostrarMensaje(string mensaje)
    {
        if (textoMensaje == null)
        {
            return;
        }

        textoMensaje.text = mensaje;
        textoMensaje.gameObject.SetActive(true);
    }

    private void VictoriaFinal()
    {
        cambiandoNivel = true;
        partidaTerminada = true;
        MostrarMensaje(CrearResumenFinal("Victoria final"));
        MostrarBotonesFinales(true);
        ReproducirSonidoFinal("Sounds/win");
    }

    private void ReproducirSonidoFinal(string ruta)
    {
        AudioClip sonido = Resources.Load<AudioClip>(ruta);

        if (sonido == null)
        {
            return;
        }

        Vector3 posicion = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
        AudioSource.PlayClipAtPoint(sonido, posicion, 1f);
    }

    private string CrearResumenFinal(string titulo)
    {
        int minutos = Mathf.FloorToInt(tiempoPartida / 60f);
        int segundos = Mathf.FloorToInt(tiempoPartida % 60f);
        return $"{titulo}\nPuntos: {puntos}\nTiempo: {minutos:00}:{segundos:00}";
    }

    private void ReiniciarEstado()
    {
        vidas = VidasIniciales;
        puntos = 0;
        tiempoPartida = 0f;
        asteroidesActivos = 0;
        asteroidesRegistrados = 0;
        cambiandoNivel = false;
        verificandoFinNivel = false;
        partidaTerminada = false;
        OcultarMensaje();
        OcultarBotonesFinales();
        ActualizarUI();
    }

    private void MostrarBotonesFinales(bool mostrarMenu)
    {
        if (botonReiniciar != null)
        {
            botonReiniciar.gameObject.SetActive(true);
        }

        if (botonMenu != null)
        {
            botonMenu.gameObject.SetActive(mostrarMenu);
        }
    }

    private void OcultarBotonesFinales()
    {
        if (botonReiniciar != null)
        {
            botonReiniciar.gameObject.SetActive(false);
        }

        if (botonMenu != null)
        {
            botonMenu.gameObject.SetActive(false);
        }
    }

    private void OcultarMensaje()
    {
        if (textoMensaje != null)
        {
            textoMensaje.gameObject.SetActive(false);
        }
    }
}
