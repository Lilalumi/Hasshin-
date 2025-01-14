using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    public AudioClip buttonClickSound; // El sonido que quieres reproducir
    private Button button;

    private void Start()
    {
        // Obtén el componente Button
        button = GetComponent<Button>();

        if (button != null && buttonClickSound != null)
        {
            // Añade el listener para el click del botón
            button.onClick.AddListener(PlayButtonClickSound);
        }
        else
        {
            Debug.LogWarning("Falta asignar el botón o el sonido en el inspector.");
        }
    }

    private void PlayButtonClickSound()
    {
        // Usa el SoundManager para reproducir el sonido
        SoundManager.Instance.PlaySFX(buttonClickSound);
    }
}
