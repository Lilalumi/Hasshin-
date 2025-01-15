using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class PaddleLightAnimations : MonoBehaviour
{
    [Header("Impact Settings")]
    public float impactPulseIntensity = 5f; // Intensidad del pulso de luz al impactar con la pelota
    public float impactPulseDuration = 0.3f; // Duración del pulso de impacto

    [Header("Movement Settings")]
    public float movementLightIntensity = 1f; // Intensidad de la luz cuando se mueve el Paddle
    public float fadeSpeed = 2f; // Velocidad de atenuación de la luz al detenerse

    private Light2D paddleLight; // Referencia al componente Light2D
    private bool isMoving = false; // Si el Paddle está en movimiento
    private float targetIntensity = 0f; // Intensidad objetivo de la luz
    private float currentInput = 0f; // Última entrada de movimiento registrada

    void Start()
    {
        paddleLight = GetComponent<Light2D>();
        if (paddleLight == null)
        {
            Debug.LogError("No se encontró un componente Light2D en el objeto Paddle.");
            enabled = false;
            return;
        }

        paddleLight.intensity = 0f; // Inicia con la luz apagada
    }

    void Update()
    {
        // Detecta movimiento
        float input = Input.GetAxis("Horizontal");

        if (Mathf.Abs(input) > 0.1f)
        {
            if (!isMoving)
            {
                isMoving = true;
                targetIntensity = movementLightIntensity; // Enciende la luz al moverse
            }
        }
        else if (isMoving)
        {
            isMoving = false;
            targetIntensity = 0f; // Apaga la luz al detenerse
        }

        // Transición suave hacia la intensidad objetivo
        paddleLight.intensity = Mathf.Lerp(paddleLight.intensity, targetIntensity, Time.deltaTime * fadeSpeed);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            TriggerImpactPulse();
        }
    }

    private void TriggerImpactPulse()
    {
        // Detiene cualquier pulso previo
        LeanTween.cancel(gameObject);

        // Anima la intensidad de la luz para simular un pulso
        LeanTween.value(gameObject, paddleLight.intensity, impactPulseIntensity, impactPulseDuration * 0.5f)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnUpdate((float value) => paddleLight.intensity = value)
            .setOnComplete(() =>
            {
                // Vuelve a la intensidad objetivo actual
                LeanTween.value(gameObject, paddleLight.intensity, targetIntensity, impactPulseDuration * 0.5f)
                    .setEase(LeanTweenType.easeInQuad)
                    .setOnUpdate((float value) => paddleLight.intensity = value);
            });
    }
}
