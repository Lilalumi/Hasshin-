using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public string levelSceneName = "Level"; // Nombre de la escena del nivel

    // Método para cargar la escena del nivel
    public void OnPlayButtonPressed()
    {
        SceneManager.LoadScene(levelSceneName);
    }
}
