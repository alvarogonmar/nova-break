using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ZonaMuerte : MonoBehaviour
{
    [SerializeField] private AudioClip sonidoPerderVida;
    [SerializeField] private float volumenSonido = 0.7f;
    [SerializeField] private Color colorDano = new Color(1f, 0f, 0f, 0.35f);
    [SerializeField] private float duracionFlash = 0.35f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Orbe"))
        {
            Debug.Log("Perdiste una vida");
            bool quedanVidas = GameSession.Instancia.PerderVida();
            ReproducirSonidoPerderVida();
            StartCoroutine(MostrarFlashDano());
            RecentrarNave();
            OrbeMovimiento orbe = collision.GetComponent<OrbeMovimiento>();

            if (orbe != null && quedanVidas)
            {
                orbe.ReiniciarOrbe();
            }
        }
    }

    private void RecentrarNave()
    {
        GameObject nave = GameObject.FindGameObjectWithTag("Nave");

        if (nave == null)
        {
            return;
        }

        NaveMovimiento naveMovimiento = nave.GetComponent<NaveMovimiento>();

        if (naveMovimiento != null)
        {
            naveMovimiento.Recentrar();
        }
    }

    private void ReproducirSonidoPerderVida()
    {
        if (sonidoPerderVida == null)
        {
            return;
        }

        Vector3 posicion = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
        AudioSource.PlayClipAtPoint(sonidoPerderVida, posicion, volumenSonido);
    }

    private IEnumerator MostrarFlashDano()
    {
        GameObject canvasObjeto = new GameObject("FlashDano");

        Canvas canvas = canvasObjeto.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 250;

        canvasObjeto.AddComponent<CanvasScaler>();
        canvasObjeto.AddComponent<GraphicRaycaster>();

        GameObject imagenObjeto = new GameObject("ColorDano");
        imagenObjeto.transform.SetParent(canvasObjeto.transform, false);

        Image imagen = imagenObjeto.AddComponent<Image>();
        imagen.color = colorDano;
        imagen.raycastTarget = false;

        RectTransform rect = imagen.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        float tiempo = 0f;

        while (tiempo < duracionFlash)
        {
            tiempo += Time.deltaTime;
            float alpha = Mathf.Lerp(colorDano.a, 0f, tiempo / duracionFlash);
            imagen.color = new Color(colorDano.r, colorDano.g, colorDano.b, alpha);
            yield return null;
        }

        Destroy(canvasObjeto);
    }
}
