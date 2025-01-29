using UnityEngine;
using UnityEngine.Events;

public class SyncStatus : MonoBehaviour
{
    [Header("Sync Settings")]
    public int impactThreshold = 3; // Número de impactos necesarios para activar SYNC
    private int currentImpacts = 0; // Contador de impactos actuales
    private bool isSyncActive = false; // Estado SYNC

    [Header("Visual Feedback")]
    public SpriteRenderer spriteRenderer; // Referencia al SpriteRenderer
    public Color syncColor = Color.cyan; // Color al entrar en SYNC
    public Color pulseColor = Color.magenta; // Color del pulso
    public float pulseSpeed = 2f; // Velocidad del pulso de color

    [Header("Audio Feedback")]
    public AudioClip syncSound; // Sonido al entrar en SYNC

    [Header("Particle Settings")]
    public GameObject particleTrailPrefab; // Prefab del sistema de partículas
    private GameObject activeParticleTrail; // Instancia activa del sistema de partículas

    [Header("Events")]
    public UnityEvent OnSyncActivated; // Evento disparado al activar SYNC

    private Rigidbody2D ballRigidbody; // Referencia al Rigidbody2D de la pelota
    private Color originalColor; // Color original del SpriteRenderer
    private float pulseTime = 0f; // Temporizador para el pulso de color

    private void Start()
    {
        ballRigidbody = GetComponent<Rigidbody2D>();
        if (ballRigidbody == null)
        {
            Debug.LogError("No se encontró un Rigidbody2D en el objeto Ball.");
        }

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color; // Guardar el color original
        }
    }

    private void Update()
    {
        if (isSyncActive && spriteRenderer != null)
        {
            PulseColor();
        }

        if (isSyncActive && activeParticleTrail != null && ballRigidbody != null)
        {
            UpdateParticleTrailDirection();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle"))
        {
            HandlePaddleImpact();
        }
        else if (collision.gameObject.CompareTag("Core"))
        {
            ResetSync(); // Reinicia el contador si golpea el núcleo
        }
    }

    private void HandlePaddleImpact()
    {
        if (!isSyncActive)
        {
            currentImpacts++;

            if (currentImpacts >= impactThreshold)
            {
                ActivateSync();
            }
        }
    }

    private void ActivateSync()
    {
        isSyncActive = true;

        // Activar partículas
        if (particleTrailPrefab != null)
        {
            activeParticleTrail = Instantiate(particleTrailPrefab, transform);
            activeParticleTrail.transform.localPosition = Vector3.zero; // Centrar en la pelota
        }

        // Reproducir sonido
        if (syncSound != null)
        {
            SoundManager.Instance.PlaySFX(syncSound);
        }

        OnSyncActivated?.Invoke(); // Dispara el evento si está suscrito
    }

    public void ResetSync()
    {
        isSyncActive = false;
        currentImpacts = 0;

        // Desactivar partículas
        if (activeParticleTrail != null)
        {
            Destroy(activeParticleTrail);
        }

        // Restaurar color original
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    private void UpdateParticleTrailDirection()
    {
        if (ballRigidbody.velocity != Vector2.zero)
        {
            Vector2 direction = ballRigidbody.velocity.normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            activeParticleTrail.transform.rotation = Quaternion.Euler(0, 0, angle - 90); // Ajustar rotación de la estela
        }
    }

    private void PulseColor()
    {
        pulseTime += Time.deltaTime * pulseSpeed;
        if (spriteRenderer != null)
        {
            // Alternar entre el color SYNC y el color del pulso
            spriteRenderer.color = Color.Lerp(syncColor, pulseColor, Mathf.PingPong(pulseTime, 1f));
        }
    }

    public bool IsSyncActive()
    {
        return isSyncActive;
    }

    public int GetCurrentImpacts()
    {
        return currentImpacts;
    }
}
