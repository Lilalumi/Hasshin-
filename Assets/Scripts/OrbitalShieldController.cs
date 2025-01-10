using UnityEngine;

public class OrbitalShieldController : MonoBehaviour
{
    public Transform core { get; set; } // Propiedad pública para asignar el núcleo
    public float orbitSpeed = 50f; // Velocidad orbital
    private PaddlePowerOrbitalShield controller; // Referencia al controlador

    [Header("Auto-Destruction Settings")]
    public bool autoDestruct = false; // Toggle para auto-destrucción
    public float lifeDuration = 5f; // Duración antes de auto-destrucción
    public float warningDuration = 1f; // Tiempo de parpadeo antes de la destrucción

    [Header("Effect Settings")]
    public float effectDuration = 0.5f; // Duración del efecto de aparición y desaparición
    public Color warningColor = Color.red; // Color de advertencia durante el parpadeo

    private Vector3 originalScale; // Escala original del prefab
    private SpriteRenderer spriteRenderer; // Referencia al SpriteRenderer para cambiar el color
    private Color originalColor; // Color original del escudo
    private bool isWarningActive = false; // Estado del parpadeo

    private void Awake()
    {
        // Captura la escala y el color original
        originalScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private void Start()
    {
        // Efecto de aparición
        transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, originalScale, effectDuration).setEaseOutBounce();

        if (autoDestruct)
        {
            // Inicia la cuenta regresiva para el parpadeo y la auto-destrucción
            Invoke(nameof(StartWarning), lifeDuration - warningDuration);
            Invoke(nameof(SelfDestruct), lifeDuration);
        }
    }

    void Update()
    {
        if (core != null)
        {
            // Calcula la posición actual en la órbita
            Vector3 direction = transform.position - core.position;
            float currentAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Incrementa el ángulo para moverse alrededor del núcleo
            currentAngle += orbitSpeed * Time.deltaTime;

            // Calcula la nueva posición en la órbita
            float radius = direction.magnitude;
            float radian = currentAngle * Mathf.Deg2Rad;
            transform.position = new Vector3(
                core.position.x + Mathf.Cos(radian) * radius,
                core.position.y + Mathf.Sin(radian) * radius,
                core.position.z
            );

            // Ajusta la rotación para apuntar hacia afuera
            transform.rotation = Quaternion.Euler(0f, 0f, currentAngle - 90f);
        }
    }

    public void SetController(PaddlePowerOrbitalShield orbitalShieldController)
    {
        controller = orbitalShieldController;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            // Notifica al controlador para eliminar este escudo
            if (controller != null)
            {
                controller.RemoveOrbitalShield(gameObject); // Elimina el escudo con efecto
            }
            else
            {
                Debug.LogWarning("El controlador de OrbitalShield no está asignado.");
                SelfDestruct(); // Elimina directamente si no hay controlador
            }
        }
    }

    private void StartWarning()
    {
        if (spriteRenderer != null)
        {
            isWarningActive = true;

            // Inicia el parpadeo entre originalColor y warningColor
            LeanTween.value(gameObject, 0f, 1f, 0.2f)
                .setLoopPingPong()
                .setOnUpdate((float t) =>
                {
                    spriteRenderer.color = Color.Lerp(originalColor, warningColor, t);
                });
        }
    }

    private void SelfDestruct()
    {
        if (isWarningActive)
        {
            LeanTween.cancel(gameObject); // Detiene el parpadeo
            spriteRenderer.color = originalColor; // Restaura el color original
        }

        // Efecto de desaparición
        LeanTween.scale(gameObject, Vector3.zero, effectDuration).setEaseInBack().setOnComplete(() =>
        {
            if (controller != null)
            {
                controller.RemoveOrbitalShield(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        });
    }
}
