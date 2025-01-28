using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MagneticLasso : MonoBehaviour
{
    [Header("Dependencies")]
    public SpriteRenderer paddleSprite; 
    public Light2D paddleLight; 
    public LineRenderer lineRenderer; 

    [Header("Settings")]
    public float pullStrength = 1f; 
    public float speedReductionRate = 0.1f; 
    public KeyCode activationKey = KeyCode.Space; 
    public Color activeColor = Color.cyan; 
    public float lightPulseIntensity = 1.5f; 
    public float lightPulseSpeed = 5f; 

    private BallBehavior mainBall; 
    private bool isActive = false; 
    private Color originalColor; 
    private float originalLightIntensity; 

    void Start()
    {
        InitializeFeedback();
        if (lineRenderer != null) lineRenderer.enabled = false;
    }

    void Update()
    {
        if (mainBall == null) AssignBall();
        if (mainBall == null) return;

        if (ControlSettings.GetCurrentMode() == ControlMode.Mouse) HandleMouseControl();
        else HandleKeyboardControl();
    }

    private void HandleKeyboardControl()
    {
        if (PauseManager.IsPaused) return;

        if (Input.GetKeyDown(activationKey)) ActivateLasso();
        if (Input.GetKey(activationKey) && isActive) MaintainLasso();
        if (Input.GetKeyUp(activationKey)) DeactivateLasso();
    }

    private void HandleMouseControl()
    {
        if (PauseManager.IsPaused) return;

        if (Input.GetMouseButtonDown(2)) ActivateLasso();
        if (Input.GetMouseButton(2) && isActive) MaintainLasso();
        if (Input.GetMouseButtonUp(2)) DeactivateLasso();
    }

    private void AssignBall()
    {
        GameObject ballObject = GameObject.FindWithTag("Ball");
        if (ballObject != null) mainBall = ballObject.GetComponent<BallBehavior>();
    }

    private void InitializeFeedback()
    {
        if (paddleSprite != null) originalColor = paddleSprite.color;
        if (paddleLight != null) originalLightIntensity = paddleLight.intensity;
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
        if (ballRb != null) ballRb.velocity = Vector2.Lerp(ballRb.velocity, direction * ballRb.velocity.magnitude, pullStrength * Time.deltaTime);
    }

    private void ActivateFeedback()
    {
        if (paddleSprite != null) paddleSprite.color = activeColor;
        if (paddleLight != null) paddleLight.intensity = lightPulseIntensity;
        if (lineRenderer != null) lineRenderer.enabled = true;
    }

    private void DeactivateFeedback()
    {
        if (paddleSprite != null) paddleSprite.color = originalColor;
        if (paddleLight != null) paddleLight.intensity = originalLightIntensity;
        DisableLineRenderer();
    }

    private void UpdateLightPulse()
    {
        if (paddleLight != null) paddleLight.intensity = originalLightIntensity + Mathf.PingPong(Time.time * lightPulseSpeed, lightPulseIntensity - originalLightIntensity);
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
        if (lineRenderer != null) lineRenderer.enabled = false;
    }

    void OnDrawGizmos()
    {
        if (isActive && mainBall != null) Gizmos.DrawLine(transform.position, mainBall.transform.position);
    }
}