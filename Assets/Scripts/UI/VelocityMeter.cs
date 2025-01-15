using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VelocityMeter : MonoBehaviour
{
    [Header("Dependencies")]
    public BallSpawner ballSpawner; // Referencia al BallSpawner asignable desde el Inspector

    [Header("UI Elements")]
    public TextMeshProUGUI speedText; // Texto que mostrará la velocidad
    public Image speedBar; // Barra que representará la velocidad
    public Image speedLine; // Línea del velocímetro
    public Image speedmeterBorder; // Borde del velocímetro

    [Header("Settings")]
    public float maxSpeed = 15f; // Velocidad máxima predeterminada
    public bool useInspectorMaxSpeed = true; // Toggle para usar el valor del Inspector o la MaxSpeed de Ball

    [Header("Colors")]
    public Color baseColor = Color.white; // Color base
    public Color maxSpeedColor = Color.red; // Color cuando se alcanza la velocidad máxima
    public AnimationCurve colorTransitionCurve; // Curva para ajustar la transición de color

    private BallBehavior targetBall; // Referencia a la pelota original
    private bool targetAssigned = false; // Bandera para evitar cambios de referencia

    void Start()
    {
        // Valida si la referencia al BallSpawner está asignada
        if (ballSpawner == null)
        {
            return;
        }

        // Suscribirse al evento OnBallSpawned del BallSpawner
        ballSpawner.OnBallSpawned += AssignTargetBall;

        // Inicializa los colores base
        ApplyColor(baseColor);
    }

    void OnDestroy()
    {
        // Desuscribirse del evento para evitar errores si el objeto se destruye
        if (ballSpawner != null)
        {
            ballSpawner.OnBallSpawned -= AssignTargetBall;
        }
    }

    void Update()
    {
        if (targetBall == null) return;

        // Obtén la velocidad actual de la pelota
        float currentSpeed = targetBall.GetComponent<Rigidbody2D>().velocity.magnitude * 10;

        // Actualiza el texto
        if (speedText != null)
        {
            speedText.text = $"{Mathf.Clamp((int)currentSpeed, 0, 999):000}";
        }

        // Actualiza la barra
        if (speedBar != null)
        {
            float effectiveMaxSpeed = useInspectorMaxSpeed ? maxSpeed * 10 : targetBall.maxSpeed * 10;
            float normalizedSpeed = Mathf.Clamp01(currentSpeed / effectiveMaxSpeed);

            speedBar.fillAmount = normalizedSpeed;

            // Actualiza los colores según la velocidad
            Color targetColor = Color.Lerp(baseColor, maxSpeedColor, colorTransitionCurve.Evaluate(normalizedSpeed));
            ApplyColor(targetColor);
        }
    }

    private void AssignTargetBall(GameObject ball)
    {
        // Evita reasignar el objetivo si ya hay uno asignado
        if (targetAssigned) return;

        // Verifica que el objeto tenga el Tag correcto
        if (ball.CompareTag("Ball"))
        {
            targetBall = ball.GetComponent<BallBehavior>();
            if (targetBall != null)
            {
                targetAssigned = true; // Marca que el objetivo está asignado
            }
        }
    }

    private void ApplyColor(Color color)
    {
        // Aplica el color a los elementos relevantes
        if (speedBar != null) speedBar.color = color;
        if (speedmeterBorder != null) speedmeterBorder.color = color;
        if (speedText != null) speedText.color = color;
    }
}
