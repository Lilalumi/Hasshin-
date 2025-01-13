using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider musicSlider; // Slider para música
    public Slider sfxSlider; // Slider para SFX

    private void Start()
    {
        if (SoundManager.Instance == null)
        {
            Debug.LogError("SoundManager no encontrado. Asegúrate de que exista en la escena.");
            return;
        }

        // Inicializar sliders con valores del SoundManager
        if (musicSlider != null)
        {
            musicSlider.value = SoundManager.Instance.musicVolume;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = SoundManager.Instance.sfxVolume;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
    }

    private void SetMusicVolume(float value)
    {
        SoundManager.Instance.SetMusicVolume(value); // Ajusta el volumen de la música
    }

    private void SetSFXVolume(float value)
    {
        SoundManager.Instance.SetSFXVolume(value); // Ajusta el volumen de los SFX
    }

    private void OnDestroy()
    {
        // Desuscribir eventos para evitar errores
        if (musicSlider != null)
            musicSlider.onValueChanged.RemoveListener(SetMusicVolume);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.RemoveListener(SetSFXVolume);
    }
}
