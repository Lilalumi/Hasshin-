using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    public Animator animator; // Asigna el Animator que contiene la animación de la intro
    public string mainMenuSceneName = "MainMenu"; // Nombre de la escena del menú principal

    void Start()
    {
        if (animator == null)
        {
            Debug.LogError("Animator no asignado en el inspector.");
        }
    }

    // Este método debe llamarse al final de la animación
    public void OnIntroAnimationEnd()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
