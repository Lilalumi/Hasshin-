using UnityEngine;
using UnityEngine.SceneManagement; // Para gestionar escenas

public class PauseManager : MonoBehaviour
{
    public static bool IsPaused { get; private set; } = false; // Estado de pausa global
    public GameObject pauseBox; // Objeto del menú de pausa
    public Animator pauseAnimator; // Referencia al Animator
    public string mainMenuSceneName = "MainMenu"; // Nombre de la escena del menú principal

    private void Start()
    {
        if (pauseBox != null)
        {
            pauseBox.SetActive(false); // Asegurarse de que PauseBox esté inactivo al inicio
        }

        if (pauseAnimator == null && pauseBox != null)
        {
            pauseAnimator = pauseBox.GetComponent<Animator>(); // Obtener Animator del PauseBox si no está asignado
        }

        // Configurar Animator para usar tiempo no escalado
        if (pauseAnimator != null)
        {
            pauseAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
    }

    private void Update()
    {
        // Detectar si se presiona ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        if (IsPaused) return; // Evitar múltiples activaciones

        IsPaused = true;

        // Pausar el tiempo inmediatamente
        Time.timeScale = 0f;

        // Activar el PauseBox y reproducir la animación PauseIn
        if (pauseBox != null)
        {
            pauseBox.SetActive(true);
            if (pauseAnimator != null)
            {
                pauseAnimator.Play("PauseIn");
            }
        }
    }

    public void ResumeGame()
    {
        if (!IsPaused) return; // Evitar múltiples activaciones

        IsPaused = false;

        // Reproducir la animación PauseOut
        if (pauseAnimator != null)
        {
            pauseAnimator.Play("PauseOut");
        }

        // Reanudar el tiempo después de la animación
        StartCoroutine(ResumeAfterAnimation());
    }

    private System.Collections.IEnumerator ResumeAfterAnimation()
    {
        if (pauseAnimator != null)
        {
            // Esperar hasta que termine la animación PauseOut
            yield return new WaitForSecondsRealtime(pauseAnimator.GetCurrentAnimatorStateInfo(0).length);
        }

        if (pauseBox != null)
        {
            pauseBox.SetActive(false);
        }

        Time.timeScale = 1f; // Reanudar el tiempo del juego
    }

    public void EjectToMainMenu()
    {
        // Reanudar el tiempo del juego antes de cambiar de escena
        Time.timeScale = 1f;

        // Cargar la escena del menú principal
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
