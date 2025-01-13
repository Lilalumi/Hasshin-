using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource sfxSource; // Fuente de audio para SFX
    public AudioSource musicSource; // Fuente de audio para música

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float sfxVolume = 1f; // Volumen para SFX
    [Range(0f, 1f)] public float musicVolume = 1f; // Volumen para música

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Evita que el SoundManager se destruya
            UpdateVolumes(); // Aplica los volúmenes iniciales al AudioSource
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Cargar volúmenes al iniciar
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", musicVolume);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", sfxVolume);
        UpdateVolumes();
    }

    public void PlaySFX(AudioClip clip, float volumeMultiplier = 1f)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume * volumeMultiplier);
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null || musicSource == null) return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void UpdateVolumes()
    {
        if (sfxSource != null) sfxSource.volume = sfxVolume;
        if (musicSource != null) musicSource.volume = musicVolume;
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume); // Guardar configuración
        UpdateVolumes();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume); // Guardar configuración
        UpdateVolumes();
    }
}
