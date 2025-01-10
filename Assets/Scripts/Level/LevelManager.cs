using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public string mainMenuSceneName = "MainMenu"; // Nombre de la escena del menú principal

    public void OnGameOver()
    {
        Debug.Log("Game Over! Regresando al menú principal...");
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OnVictory()
    {
        Debug.Log("¡Victoria! Regresando al menú principal...");
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
