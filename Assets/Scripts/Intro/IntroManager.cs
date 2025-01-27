using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class IntroManager : MonoBehaviour
{
    [Header("Intro Settings")]
    public Animator animator; // Asigna el Animator que contiene la animación de la intro
    public string mainMenuSceneName = "MainMenu"; // Nombre de la escena del menú principal

    [Header("Skip UI Settings")]
    public GameObject skipIntroUI; // Referencia al objeto UI de SkipIntro
    public TextMeshProUGUI skipText; // Texto que muestra las instrucciones de "Skip"
    public float skipUIFadeTime = 5f; // Tiempo antes de ocultar el UI si no se interactúa

    private int skipCount = 3; // Cantidad de presiones necesarias para saltar la intro
    private float lastInputTime; // Marca el tiempo desde la última entrada de usuario
    private bool isSkipUIActive = false; // Estado del UI de Skip

    void Start()
    {
        if (animator == null)
        {
            Debug.LogError("Animator no asignado en el inspector.");
        }

        if (skipIntroUI != null)
        {
            skipIntroUI.SetActive(false); // Asegúrate de que el UI esté desactivado al inicio
        }
    }
    void Awake()
    {
        LeanTween.init(3000); // Cambia 800 a un número más alto si lo necesitas.
    }
    void Update()
    {
        // Detecta cualquier entrada de teclado para mostrar el UI
        if (Input.anyKeyDown)
        {
            ShowSkipUI();
        }

        // Detecta si se presiona Z y actualiza el contador
        if (isSkipUIActive && Input.GetKeyDown(KeyCode.Z))
        {
            skipCount--;
            UpdateSkipText();

            if (skipCount <= 0)
            {
                SkipToMainMenu();
            }
        }

        // Oculta el UI si no hay interacción en el tiempo definido
        if (isSkipUIActive && Time.time - lastInputTime > skipUIFadeTime)
        {
            HideSkipUI();
        }
    }

    private void ShowSkipUI()
    {
        lastInputTime = Time.time; // Actualiza el tiempo de la última interacción
        if (skipIntroUI != null && !isSkipUIActive)
        {
            skipIntroUI.SetActive(true);
            isSkipUIActive = true;
        }

        UpdateSkipText();
    }

    private void HideSkipUI()
    {
        if (skipIntroUI != null)
        {
            skipIntroUI.SetActive(false);
            isSkipUIActive = false;
            skipCount = 3; // Reinicia el contador
        }
    }

    private void UpdateSkipText()
    {
        if (skipText != null)
        {
            skipText.text = $"PRESS Z {skipCount} TIMES TO SKIP INTRO";
        }

        lastInputTime = Time.time; // Actualiza el tiempo de la última interacción
    }

    private void SkipToMainMenu()
    {
        if (animator != null)
        {
            animator.enabled = false; // Detiene la animación
        }

        SceneManager.LoadScene(mainMenuSceneName); // Cambia a la escena del menú principal
    }

    // Este método debe llamarse al final de la animación
    public void OnIntroAnimationEnd()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
