using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSession : MonoBehaviour
{
    private const int VidasIniciales = 5;

    private static GameSession instancia;

    private int vidas = VidasIniciales;
    private float tiempoPartida;
    private int asteroidesActivos;
    private int asteroidesRegistrados;
    private bool cambiandoNivel;

    private Text textoEstado;
    private Text textoMensaje;

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

        if (asteroidesRegistrados > 0 && asteroidesActivos == 0 && !cambiandoNivel && SceneManager.GetActiveScene().name == "Nivel1")
        {
            StartCoroutine(PasarANivel2());
        }
    }

    public bool PerderVida()
    {
        vidas = Mathf.Max(0, vidas - 1);
        ActualizarUI();

        if (vidas == 0)
        {
            MostrarMensaje("Game Over");
            return false;
        }

        return true;
    }

    private IEnumerator PasarANivel2()
    {
        cambiandoNivel = true;
        MostrarMensaje("Has pasado al nivel 2");

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("Nivel2");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        asteroidesActivos = 0;
        asteroidesRegistrados = 0;
        cambiandoNivel = false;
        CrearUI();
        OcultarMensaje();
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

    private void ActualizarUI()
    {
        if (textoEstado == null)
        {
            return;
        }

        int minutos = Mathf.FloorToInt(tiempoPartida / 60f);
        int segundos = Mathf.FloorToInt(tiempoPartida % 60f);
        textoEstado.text = $"Tiempo: {minutos:00}:{segundos:00}\nVidas: {vidas}";
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

    private void OcultarMensaje()
    {
        if (textoMensaje != null)
        {
            textoMensaje.gameObject.SetActive(false);
        }
    }
}
