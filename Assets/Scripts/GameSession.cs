using System.Collections;
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

    private Text textoEstado;
    private Text textoMensaje;
    private Button botonReiniciar;
    private Button botonMenu;

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

        if (asteroidesRegistrados == 0 || asteroidesActivos > 0 || cambiandoNivel)
        {
            return;
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
    }

    public void SumarPuntos(int cantidad)
    {
        puntos += Mathf.Max(0, cantidad);
        ActualizarUI();
    }

    public bool PerderVida()
    {
        vidas = Mathf.Max(0, vidas - 1);
        ActualizarUI();

        if (vidas == 0)
        {
            MostrarMensaje("Game Over");
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
        textoMensaje = CrearTexto("Mensaje", canvasObjeto.transform, Vector2.zero, TextAnchor.MiddleCenter, 54);
        textoMensaje.rectTransform.anchorMin = new Vector2(0f, 0f);
        textoMensaje.rectTransform.anchorMax = new Vector2(1f, 1f);
        textoMensaje.rectTransform.offsetMin = Vector2.zero;
        textoMensaje.rectTransform.offsetMax = Vector2.zero;
        textoMensaje.gameObject.SetActive(false);

        botonReiniciar = CrearBoton("BotonReiniciar", canvasObjeto.transform, new Vector2(-130f, -120f), "Reiniciar");
        botonReiniciar.onClick.AddListener(ReiniciarJuego);

        botonMenu = CrearBoton("BotonMenu", canvasObjeto.transform, new Vector2(130f, -120f), "Menu");
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

    private void ActualizarUI()
    {
        if (textoEstado == null)
        {
            return;
        }

        int minutos = Mathf.FloorToInt(tiempoPartida / 60f);
        int segundos = Mathf.FloorToInt(tiempoPartida % 60f);
        textoEstado.text = $"Tiempo: {minutos:00}:{segundos:00}\nVidas: {vidas}\nPuntos: {puntos}";
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
        MostrarMensaje("Victoria final");
        MostrarBotonesFinales(true);
    }

    private void ReiniciarEstado()
    {
        vidas = VidasIniciales;
        puntos = 0;
        tiempoPartida = 0f;
        asteroidesActivos = 0;
        asteroidesRegistrados = 0;
        cambiandoNivel = false;
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
