using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PaddleBounceAnimation : MonoBehaviour
{
    public float bounceScaleX = 1.2f; // Estiramiento horizontal
    public float bounceScaleY = 0.8f; // Compresión vertical
    public float bounceDuration = 0.3f; // Duración del estiramiento
    public float returnDuration = 0.2f; // Duración del retorno a la escala
    public float maxBounceTime = 0.8f; // Tiempo máximo permitido en la escala de Bounce antes de forzar la restauración
    public LeanTweenType bounceEaseType = LeanTweenType.easeOutBounce; // Easing para el estiramiento
    public LeanTweenType returnEaseType = LeanTweenType.easeInOutQuad; // Easing para el retorno

    private Vector3 currentScale; // Escala actual del Paddle (respetando Stretch)
    private Vector3 originalScale; // Escala inicial del Paddle
    private bool isAnimating = false; // Evita superposición de animaciones
    private float scaleResetTimer = 0f; // Temporizador para restaurar la escala

    void Start()
    {
        // Cancelar cualquier animación activa para evitar estados erróneos
        LeanTween.cancel(gameObject);

        // Reiniciar variables
        isAnimating = false;
        scaleResetTimer = 0f;

        // Guarda la escala original
        originalScale = transform.localScale;
        currentScale = originalScale;
    }

    void Update()
    {
        CheckScaleTimeout();
    }

    public void UpdateTargetScale(Vector3 newScale)
    {
        // Actualiza la escala actual para respetar cambios como Stretch
        currentScale = newScale;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            // Ignora colisiones con clones
            var cloneController = GetComponent<PaddleCloneController>();
            if (cloneController != null && cloneController.isClone)
            {
                return;
            }

            TriggerBounceAnimation();
        }
    }

    private void TriggerBounceAnimation()
    {
        // Cancelar cualquier animación residual antes de comenzar una nueva
        if (LeanTween.isTweening(gameObject))
        {
            LeanTween.cancel(gameObject);
            isAnimating = false;
        }

        // Evita iniciar una nueva animación si ya hay una en curso
        if (isAnimating)
        {
            return;
        }

        isAnimating = true;

        // Escala para el estiramiento
        Vector3 bounceScale = new Vector3(
            currentScale.x * bounceScaleX, // Estiramiento horizontal
            currentScale.y * bounceScaleY, // Compresión vertical
            currentScale.z
        );

        // Reinicia el temporizador cuando la animación comienza
        scaleResetTimer = 0f;

        // Animación de estiramiento
        LeanTween.scale(gameObject, bounceScale, bounceDuration)
            .setEase(bounceEaseType)
            .setOnComplete(() =>
            {
                // Retorno a la escala actual
                LeanTween.scale(gameObject, currentScale, returnDuration)
                    .setEase(returnEaseType)
                    .setOnComplete(() =>
                    {
                        isAnimating = false; // Animación completada
                        scaleResetTimer = 0f; // Reiniciar el temporizador
                    });
            });
    }

    private void CheckScaleTimeout()
    {
        // Si no hay animación en curso, no es necesario comprobar el temporizador
        if (!isAnimating && !LeanTween.isTweening(gameObject))
        {
            scaleResetTimer = 0f; // Reinicia el temporizador si no hay animación
            return;
        }

        scaleResetTimer += Time.deltaTime;

        if (scaleResetTimer >= maxBounceTime)
        {
            ResetScale();
        }
    }

    private void ResetScale()
    {
        LeanTween.cancel(gameObject); // Cancela cualquier animación en curso
        transform.localScale = currentScale; // Restaura la escala normal
        isAnimating = false;
        scaleResetTimer = 0f;
    }
}
