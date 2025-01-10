using UnityEngine;

public class PaddleController : MonoBehaviour
{
    public Transform core; // Núcleo central alrededor del cual rotará
    public float speed = 200f; // Velocidad de rotación
    public float stretchAmount = 1.3f; // Factor de estiramiento del paddle
    public float animationDuration = 0.4f; // Duración de las animaciones
    public bool enableAnimations = true; // Toggle para activar/desactivar animaciones

    private Vector3 originalScale; // Escala original del paddle
    private bool isAnimating = false; // Bandera para evitar conflictos de animación
    private bool isMoving = false; // Bandera para detectar si el paddle está en movimiento
    private float movementTimer = 0f; // Temporizador para medir el tiempo de movimiento
    private float lastInput = 0f; // Última dirección del movimiento

    [Header("Effect Configurations")]
    public float minMoveTimeForStopEffect = 0.2f; // Tiempo mínimo para activar animaciones de frenado o cambio de dirección
    public float stopEffectScale = 1.5f; // Escala del efecto de frenado
    public float directionChangeEffectScale = 1.7f; // Escala del efecto de cambio de dirección abrupto

    void Start()
    {
        // Almacena la escala original
        originalScale = transform.localScale;
    }

    void Update()
    {
        float input = Input.GetAxis("Horizontal");

        if (input != 0)
        {
            // El paddle está en movimiento
            isMoving = true;
            movementTimer += Time.deltaTime;

            // Detectar cambio abrupto de dirección
            if (Mathf.Sign(input) != Mathf.Sign(lastInput) && Mathf.Abs(lastInput) > 0.1f && movementTimer > minMoveTimeForStopEffect)
            {
                Debug.Log("Cambio abrupto de dirección.");
                AnimateDirectionChange();
            }

            // Rota alrededor del núcleo
            transform.RotateAround(core.position, Vector3.forward, -input * speed * Time.deltaTime);

            // Activa la animación de estiramiento si está habilitada y no en curso
            if (enableAnimations && !isAnimating)
            {
                AnimateStretch();
            }

            lastInput = input;
        }
        else if (isMoving)
        {
            // El paddle se detiene
            isMoving = false;

            if (movementTimer > minMoveTimeForStopEffect)
            {
                Debug.Log("Frenado después de movimiento.");
                AnimateStop();
            }

            movementTimer = 0f;
            lastInput = 0f;

            if (enableAnimations)
            {
                ResetScale();
            }
        }
    }

    private void AnimateStretch()
    {
        isAnimating = true;

        // Define la nueva escala alargada
        Vector3 targetScale = new Vector3(
            originalScale.x * stretchAmount, // Alarga en la dirección x
            originalScale.y / stretchAmount, // Comprime en la dirección y
            originalScale.z
        );

        // Aplica la animación de estiramiento
        LeanTween.scale(gameObject, targetScale, animationDuration).setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() =>
            {
                isAnimating = false;

                // Si el paddle ha dejado de moverse, regresa a la escala original
                if (!isMoving)
                {
                    ResetScale();
                }
            });
    }

    private void AnimateStop()
    {
        isAnimating = true;

        // Define la nueva escala para el efecto de frenado
        Vector3 stopScale = new Vector3(
            originalScale.x * stopEffectScale,
            originalScale.y / stopEffectScale,
            originalScale.z
        );

        LeanTween.scale(gameObject, stopScale, animationDuration).setEase(LeanTweenType.easeOutElastic)
            .setOnComplete(() =>
            {
                ResetScale(); // Vuelve a la escala original tras el efecto de frenado
            });
    }

    private void AnimateDirectionChange()
    {
        isAnimating = true;

        // Define la nueva escala para el cambio abrupto de dirección
        Vector3 changeScale = new Vector3(
            originalScale.x * directionChangeEffectScale,
            originalScale.y / directionChangeEffectScale,
            originalScale.z
        );

        LeanTween.scale(gameObject, changeScale, animationDuration * 0.5f).setEase(LeanTweenType.easeOutBounce)
            .setOnComplete(() =>
            {
                ResetScale(); // Vuelve a la escala original tras el efecto
            });
    }

    private void ResetScale()
    {
        isAnimating = true;

        // Devuelve el paddle a su escala original
        LeanTween.scale(gameObject, originalScale, animationDuration).setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() => isAnimating = false);
    }
}
