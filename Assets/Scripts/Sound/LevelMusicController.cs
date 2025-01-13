using UnityEngine;

public class LevelMusicController : MonoBehaviour
{
    [Header("Level Music")]
    public AudioClip levelMusic; // Música específica del nivel

    [Header("Music Settings")]
    public bool loopMusic = true; // Si la música debe repetirse en bucle
    [Range(0f, 1f)] public float musicVolume = 1f; // Volumen de la música

    private void Start()
    {
        if (SoundManager.Instance == null)
        {
            Debug.LogError("No se encontró un SoundManager en la escena. Asegúrate de que exista uno.");
            return;
        }

        PlayLevelMusic();
    }

    private void PlayLevelMusic()
    {
        if (levelMusic != null)
        {
            SoundManager.Instance.SetMusicVolume(musicVolume); // Establece el volumen
            SoundManager.Instance.PlayMusic(levelMusic, loopMusic); // Reproduce la música
        }
        else
        {
            Debug.LogWarning("No se asignó música para este nivel en el LevelMusicController.");
        }
    }
    public void ChangeMusic(AudioClip newMusic, float newVolume = 1f, bool loop = true)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetMusicVolume(newVolume);
            SoundManager.Instance.PlayMusic(newMusic, loop);
        }
    }
}
