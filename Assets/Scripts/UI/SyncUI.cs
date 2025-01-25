using UnityEngine;

public class SyncUI : MonoBehaviour
{
    [Header("References")]
    public BallSpawner ballSpawner; // Referencia al BallSpawner
    public RectTransform syncBox; // Referencia al objeto SyncBox
    public RectTransform syncR; // Referencia al objeto SyncR
    public RectTransform syncB; // Referencia al objeto SyncB

    [Header("Position Settings")]
    public Vector2 syncRStartPosition; // Posición inicial de SyncR
    public Vector2 syncBStartPosition; // Posición inicial de SyncB
    public Vector2 syncTargetPosition; // Posición objetivo compartida por SyncR y SyncB

    [Header("Color Settings")]
    public Color syncRStartColor = Color.white; // Color inicial de SyncR
    public Color syncREndColor = Color.green; // Color en modo SYNC de SyncR
    public Color syncBStartColor = Color.white; // Color inicial de SyncB
    public Color syncBEndColor = Color.green; // Color en modo SYNC de SyncB
    public Color syncBoxBlinkColor1 = Color.white; // Primer color de parpadeo de SyncBox
    public Color syncBoxBlinkColor2 = Color.cyan; // Segundo color de parpadeo de SyncBox

    [Header("Settings")]
    public float animationDuration = 0.5f; // Duración de la animación de movimiento
    public float blinkSpeed = 0.5f; // Velocidad del parpadeo de SyncBox

    private GameObject ballReference; // Referencia a la pelota
    private SyncStatus syncStatus; // Referencia al script SyncStatus de la pelota
    private int maxImpactsForSync = 3; // Cantidad máxima de impactos necesarios para SYNC
    private Vector2 lastSyncRPosition; // Última posición de SyncR
    private Vector2 lastSyncBPosition; // Última posición de SyncB
    private bool isSyncActive = false; // Indica si se está en modo SYNC
    private Color originalSyncBoxColor; // Color original de SyncBox

    private void Start()
    {
        if (ballSpawner == null)
        {
            ballSpawner = FindObjectOfType<BallSpawner>();
        }

        if (ballSpawner != null)
        {
            ballSpawner.OnBallSpawned += AssignBall;
        }
        else
        {
            Debug.LogError("No se encontró un BallSpawner en la escena.");
        }

        // Configurar posiciones y colores iniciales
        if (syncR != null)
        {
            syncR.anchoredPosition = syncRStartPosition;
            syncR.GetComponent<UnityEngine.UI.Image>().color = syncRStartColor;
            lastSyncRPosition = syncRStartPosition;
        }

        if (syncB != null)
        {
            syncB.anchoredPosition = syncBStartPosition;
            syncB.GetComponent<UnityEngine.UI.Image>().color = syncBStartColor;
            lastSyncBPosition = syncBStartPosition;
        }

        if (syncBox != null)
        {
            originalSyncBoxColor = syncBox.GetComponent<UnityEngine.UI.Image>().color; // Guarda el color original
        }
    }

    private void OnDestroy()
    {
        if (ballSpawner != null)
        {
            ballSpawner.OnBallSpawned -= AssignBall;
        }
    }

    private void AssignBall(GameObject ball)
    {
        if (ball.CompareTag("Ball") && ballReference == null)
        {
            ballReference = ball;
            syncStatus = ball.GetComponent<SyncStatus>();

            if (syncStatus != null)
            {
                maxImpactsForSync = syncStatus.impactThreshold; // Asigna automáticamente el valor del umbral de Sync
                Debug.Log("SyncStatus asignado correctamente al SyncUI.");
            }
            else
            {
                Debug.LogError("El objeto Ball no tiene un componente SyncStatus.");
            }
        }
    }

    private void Update()
    {
        if (syncStatus == null) return;

        // Calcula el progreso actual como un valor entre 0 y 1
        float progress = Mathf.Clamp01((float)syncStatus.GetCurrentImpacts() / maxImpactsForSync);

        // Calcula las nuevas posiciones para SyncR y SyncB
        Vector2 newSyncRPosition = Vector2.Lerp(syncRStartPosition, syncTargetPosition, progress);
        Vector2 newSyncBPosition = Vector2.Lerp(syncBStartPosition, syncTargetPosition, progress);

        // Animar SyncR
        if (syncR != null && newSyncRPosition != lastSyncRPosition)
        {
            LeanTween.cancel(syncR.gameObject);
            LeanTween.move(syncR, newSyncRPosition, animationDuration).setEaseOutQuad();
            lastSyncRPosition = newSyncRPosition;
        }

        // Animar SyncB
        if (syncB != null && newSyncBPosition != lastSyncBPosition)
        {
            LeanTween.cancel(syncB.gameObject);
            LeanTween.move(syncB, newSyncBPosition, animationDuration).setEaseOutQuad();
            lastSyncBPosition = newSyncBPosition;
        }

        // Si alcanza el 100%, activa SYNC
        if (progress >= 1f && !isSyncActive)
        {
            ActivateSyncMode();
        }

        // Si se sale del estado SYNC
        if (progress < 1f && isSyncActive)
        {
            DeactivateSyncMode();
        }
    }

    private void ActivateSyncMode()
    {
        isSyncActive = true;

        // Cambiar colores instantáneamente al entrar en modo SYNC
        if (syncR != null)
        {
            var syncRImage = syncR.GetComponent<UnityEngine.UI.Image>();
            if (syncRImage != null)
            {
                syncRImage.color = syncREndColor; // Cambia al color de SYNC
            }
        }

        if (syncB != null)
        {
            var syncBImage = syncB.GetComponent<UnityEngine.UI.Image>();
            if (syncBImage != null)
            {
                syncBImage.color = syncBEndColor; // Cambia al color de SYNC
            }
        }

        // Comienza el parpadeo de SyncBox
        if (syncBox != null)
        {
            LeanTween.cancel(syncBox.gameObject);
            LeanTween.value(syncBox.gameObject, UpdateSyncBoxColor, 0f, 1f, blinkSpeed)
                     .setEaseInOutQuad()
                     .setLoopPingPong();
        }
    }

    private void DeactivateSyncMode()
    {
        isSyncActive = false;

        // Restaurar los colores originales
        if (syncR != null)
        {
            var syncRImage = syncR.GetComponent<UnityEngine.UI.Image>();
            if (syncRImage != null)
            {
                syncRImage.color = syncRStartColor;
            }
        }

        if (syncB != null)
        {
            var syncBImage = syncB.GetComponent<UnityEngine.UI.Image>();
            if (syncBImage != null)
            {
                syncBImage.color = syncBStartColor;
            }
        }

        // Detiene el parpadeo y restaura el color original de SyncBox
        if (syncBox != null)
        {
            LeanTween.cancel(syncBox.gameObject);
            var syncBoxImage = syncBox.GetComponent<UnityEngine.UI.Image>();
            if (syncBoxImage != null)
            {
                syncBoxImage.color = originalSyncBoxColor;
            }
        }
    }

    private void UpdateSyncBoxColor(float t)
    {
        if (syncBox != null)
        {
            var syncBoxImage = syncBox.GetComponent<UnityEngine.UI.Image>();
            if (syncBoxImage != null)
            {
                syncBoxImage.color = Color.Lerp(syncBoxBlinkColor1, syncBoxBlinkColor2, t);
            }
        }
    }
}
