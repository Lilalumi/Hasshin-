using UnityEngine;
using UnityEngine.Rendering.Universal; // Para Light2D

public class MagneticLasso : MonoBehaviour
{
    [Header("Dependencies")]
    public SpriteRenderer paddleSprite; // Referencia al SpriteRenderer del Paddle
    public Light2D paddleLight; // Referencia al componente Light2D del Paddle
    public LineRenderer lineRenderer; // Referencia al LineRenderer

    [Header("Settings")]
    public float pullStrength = 1f; // Fuerza con la que se atrae la pelota
    public float speedReductionRate = 0.1f; // Tasa de reducción de velocidad por segundo
    public KeyCode activationKey = KeyCode.Space; // Tecla para activar el poder
    public Color activeColor = Color.cyan; // Color del Paddle cuando el poder está activo
    public float lightPulseIntensity = 1.5f; // Intensidad del pulso de luz
    public float lightPulseSpeed = 5f; // Velocidad del pulso de luz

    private BallBehavior mainBall; // Referencia a la pelota principal
    private bool isActive = false; // Indica si el poder está activo
    private Color originalColor; // Color original del Paddle
    private float originalLightIntensity; // Intensidad original de la luz

    void Start()
    {
        InitializeFeedback();

        // Deshabilitar la línea al inicio
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }

    void Update()
    {
        // Buscar y asignar la pelota principal si aún no está asignada
        if (mainBall == null)
        {
            AssignBall();
        }

        if (mainBall == null) return;

        // Detectar activación dependiendo del modo de control
        if (ControlSettings.GetCurrentMode() == ControlMode.Mouse)
        {
            HandleMouseControl();
        }
        else
        {
            HandleKeyboardControl();
        }
    }

    private void HandleKeyboardControl()
    {
        if (PauseManager.IsPaused) return; // No ejecutar si está en pausa

        if (Input.GetKeyDown(activationKey))
        {
            ActivateLasso();
        }

        if (Input.GetKey(activationKey) && isActive)
        {
            MaintainLasso();
        }

        if (Input.GetKeyUp(activationKey))
        {
            DeactivateLasso();
        }
    }

    private void HandleMouseControl()
    {
        if (PauseManager.IsPaused) return; // No ejecutar si está en pausa

        if (Input.GetMouseButtonDown(2)) // Botón del medio del mouse
        {
            ActivateLasso();
        }

        if (Input.GetMouseButton(2) && isActive)
        {
            MaintainLasso();
        }

        if (Input.GetMouseButtonUp(2))
        {
            DeactivateLasso();
        }
    }

    private void AssignBall()
    {
        GameObject ballObject = GameObject.FindWithTag("Ball");
        if (ballObject != null)
        {
            mainBall = ballObject.GetComponent<BallBehavior>();
            if (mainBall != null)
            {
                Debug.Log("MainBall asignada al MagneticLasso.");
            }
            else
            {
                Debug.LogError("El objeto con el tag 'Ball' no tiene el componente BallBehavior.");
            }
        }
    }

    private void InitializeFeedback()
    {
        if (paddleSprite != null)
        {
            originalColor = paddleSprite.color;
        }

        if (paddleLight != null)
        {
            originalLightIntensity = paddleLight.intensity;
        }
    }

    private void ActivateLasso()
    {
        isActive = true;
        mainBall.ReduceSpeed(0.5f);
        ActivateFeedback();
    }

    private void MaintainLasso()
    {
        PullBall();
        mainBall.ReduceSpeed(speedReductionRate * Time.deltaTime);
        UpdateLightPulse();
        UpdateLineRenderer();
    }

    private void DeactivateLasso()
    {
        isActive = false;
        DeactivateFeedback();
    }

    private void PullBall()
    {
        Vector2 direction = ((Vector2)transform.position - (Vector2)mainBall.transform.position).normalized;
        Rigidbody2D ballRb = mainBall.GetComponent<Rigidbody2D>();
        if (ballRb != null)
        {
            ballRb.velocity = Vector2.Lerp(ballRb.velocity, direction * ballRb.velocity.magnitude, pullStrength * Time.deltaTime);
        }
    }

    private void ActivateFeedback()
    {
        if (paddleSprite != null)
        {
            paddleSprite.color = activeColor;
        }

        if (paddleLight != null)
        {
            paddleLight.intensity = lightPulseIntensity;
        }

        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
        }
    }

    private void DeactivateFeedback()
    {
        if (paddleSprite != null)
        {
            paddleSprite.color = originalColor;
        }

        if (paddleLight != null)
        {
            paddleLight.intensity = originalLightIntensity;
        }

        DisableLineRenderer();
    }

    private void UpdateLightPulse()
    {
        if (paddleLight != null)
        {
            paddleLight.intensity = originalLightIntensity + Mathf.PingPong(Time.time * lightPulseSpeed, lightPulseIntensity - originalLightIntensity);
        }
    }

    private void UpdateLineRenderer()
    {
        if (lineRenderer != null && mainBall != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, mainBall.transform.position);
        }
    }

    private void DisableLineRenderer()
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }

    void OnDrawGizmos()
    {
        if (isActive && mainBall != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, mainBall.transform.position);
        }
    }
}
