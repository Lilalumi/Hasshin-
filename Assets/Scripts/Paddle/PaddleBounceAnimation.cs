using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PaddleBounceAnimation : MonoBehaviour
{
    public float bounceScaleX = 1.2f; // Estiramiento horizontal
    public float bounceScaleY = 0.8f; // Compresión vertical
    public float bounceDuration = 0.3f; // Duración del estiramiento
    public float returnDuration = 0.2f; // Duración del retorno a la escala
    public LeanTweenType bounceEaseType = LeanTweenType.easeOutBounce; // Easing para el estiramiento
    public LeanTweenType returnEaseType = LeanTweenType.easeInOutQuad; // Easing para el retorno

    private Vector3 currentScale; // Escala actual del Paddle (respetando Stretch)
    private Vector3 originalScale; // Escala inicial del Paddle
    private bool isAnimating = false; // Evita superposición de animaciones

    void Start()
    {
        // Guarda la escala original
        originalScale = transform.localScale;
        currentScale = originalScale;
    }

    public void UpdateTargetScale(Vector3 newScale)
    {
        // Actualiza la escala actual para respetar cambios como Stretch
        currentScale = newScale;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball") && !isAnimating)
        {
            // Ignora colisiones con clones
            var cloneController = GetComponent<PaddleCloneController>();
            if (cloneController != null && cloneController.isClone)
            {
                Debug.Log("Skipping bounce animation for clone.");
                return;
            }

            TriggerBounceAnimation();
        }
    }

    private void TriggerBounceAnimation()
    {
        isAnimating = true;

        // Escala para el estiramiento
        Vector3 bounceScale = new Vector3(
            currentScale.x * bounceScaleX, // Estiramiento horizontal
            currentScale.y * bounceScaleY, // Compresión vertical
            currentScale.z
        );

        // Animación de estiramiento
        LeanTween.scale(gameObject, bounceScale, bounceDuration)
            .setEase(bounceEaseType)
            .setOnComplete(() =>
            {
                // Retorno a la escala actual
                LeanTween.scale(gameObject, currentScale, returnDuration)
                    .setEase(returnEaseType)
                    .setOnComplete(() => isAnimating = false);
            });
    }
}
