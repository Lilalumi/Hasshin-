using UnityEngine;

public class SyncUI : MonoBehaviour
{
    [Header("References")]
    public BallSpawner ballSpawner; 
    public RectTransform syncBox; 
    public RectTransform syncR; 
    public RectTransform syncB; 

    [Header("Position Settings")]
    public Vector2 syncRStartPosition; 
    public Vector2 syncBStartPosition; 
    public Vector2 syncTargetPosition; 

    [Header("Color Settings")]
    public Color syncRStartColor = Color.white; 
    public Color syncREndColor = Color.green; 
    public Color syncBStartColor = Color.white; 
    public Color syncBEndColor = Color.green; 
    public Color syncBoxBlinkColor1 = Color.white; 
    public Color syncBoxBlinkColor2 = Color.cyan; 

    [Header("Settings")]
    public float animationDuration = 0.5f; 
    public float blinkSpeed = 0.5f; 

    private GameObject ballReference; 
    private SyncStatus syncStatus; 
    private int maxImpactsForSync = 3; 
    private Vector2 lastSyncRPosition; 
    private Vector2 lastSyncBPosition; 
    private bool isSyncActive = false; 
    private Color originalSyncBoxColor; 

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
            originalSyncBoxColor = syncBox.GetComponent<UnityEngine.UI.Image>().color;
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
                maxImpactsForSync = syncStatus.impactThreshold;
            }
        }
    }

    private void Update()
    {
        if (syncStatus == null) return;

        float progress = Mathf.Clamp01((float)syncStatus.GetCurrentImpacts() / maxImpactsForSync);

        Vector2 newSyncRPosition = Vector2.Lerp(syncRStartPosition, syncTargetPosition, progress);
        Vector2 newSyncBPosition = Vector2.Lerp(syncBStartPosition, syncTargetPosition, progress);

        if (syncR != null && newSyncRPosition != lastSyncRPosition)
        {
            LeanTween.cancel(syncR.gameObject);
            LeanTween.move(syncR, newSyncRPosition, animationDuration).setEaseOutQuad();
            lastSyncRPosition = newSyncRPosition;
        }

        if (syncB != null && newSyncBPosition != lastSyncBPosition)
        {
            LeanTween.cancel(syncB.gameObject);
            LeanTween.move(syncB, newSyncBPosition, animationDuration).setEaseOutQuad();
            lastSyncBPosition = newSyncBPosition;
        }

        if (progress >= 1f && !isSyncActive)
        {
            ActivateSyncMode();
        }

        if (progress < 1f && isSyncActive)
        {
            DeactivateSyncMode();
        }
    }

    private void ActivateSyncMode()
    {
        isSyncActive = true;

        if (syncR != null)
        {
            var syncRImage = syncR.GetComponent<UnityEngine.UI.Image>();
            if (syncRImage != null)
            {
                syncRImage.color = syncREndColor;
            }
        }

        if (syncB != null)
        {
            var syncBImage = syncB.GetComponent<UnityEngine.UI.Image>();
            if (syncBImage != null)
            {
                syncBImage.color = syncBEndColor;
            }
        }

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