using UnityEngine;
using UnityEngine.SceneManagement; // Importar el espacio de nombres para manejar escenas

public class Menu : MonoBehaviour
{
    public void Jugar()
    {
        SceneManager.LoadScene("Nivel1"); // Cargar la escena del nivel 1
    }

    public void Salir()
    {
        Debug.Log("¡Saliendo del juego!"); // Mensaje de depuración para confirmar que se ha intentado salir del juego
        Application.Quit(); // Cerrar la aplicación
    }

}