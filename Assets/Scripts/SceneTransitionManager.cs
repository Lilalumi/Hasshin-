using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Importante para IEnumerator

public class SceneTransitionManager : MonoBehaviour
{
    public CanvasGroup fadeCanvasGroup; // CanvasGroup para el fade in/out
    public float fadeDuration = 1f; // Duración del fade

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    private IEnumerator FadeIn()
    {
        float timer = fadeDuration;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(timer / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 0f;
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(timer / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f;
        SceneManager.LoadScene(sceneName);
    }
}
