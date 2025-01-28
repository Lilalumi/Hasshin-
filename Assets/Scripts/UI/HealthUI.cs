using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("Health UI Settings")]
    public Transform[] healthBarSteps; 
    public string coreTag = "Core"; 
    public float animationDuration = 0.1f; 
    public float minHeight = 0.5f; 
    public float maxHeight = 1.5f; 
    public float deactivateDelay = 0.02f; 

    [Header("Wave Settings")]
    public float maxWaveSpeed = 3f; 
    public float minWaveSpeed = 1f; 
    public float maxWaveFrequency = 0.8f; 
    public float minWaveFrequency = 0.3f; 

    [Header("Color Settings")]
    public Color maxColor = Color.green; 
    public Color minColor = Color.red; 

    private Core coreReference; 
    private float currentWaveSpeed; 
    private float currentWaveFrequency; 
    private Color currentColor; 

    private void Start()
    {
        GameObject coreObject = GameObject.FindGameObjectWithTag(coreTag);
        if (coreObject == null)
        {
            Debug.LogError("No se encontró un objeto con el tag 'Core' en la escena.");
            return;
        }

        coreReference = coreObject.GetComponent<Core>();
        if (coreReference == null)
        {
            Debug.LogError("El objeto Core no tiene el script Core asociado.");
            return;
        }

        if (healthBarSteps == null || healthBarSteps.Length == 0)
        {
            Debug.LogError("HealthBarSteps no está asignado o está vacío. Asigne los pasos desde el Inspector.");
            return;
        }

        UpdateHealthBar(coreReference.currentHealth, true);
    }

    private void Update()
    {
        if (coreReference != null)
        {
            UpdateHealthBar(coreReference.currentHealth, false);
            UpdateDynamicValues();
        }
        ApplyWaveEffect();
    }

    private void UpdateHealthBar(float currentHealth, bool instant)
    {
        if (coreReference == null || healthBarSteps == null) return;

        float healthPercentage = Mathf.Clamp01(currentHealth / coreReference.maxHealth);
        int activeSteps = Mathf.RoundToInt(healthPercentage * healthBarSteps.Length);

        for (int i = healthBarSteps.Length - 1; i >= 0; i--)
        {
            if (healthBarSteps[i] == null) continue;

            bool shouldBeActive = i < activeSteps;

            if (shouldBeActive)
            {
                if (!healthBarSteps[i].gameObject.activeSelf)
                {
                    healthBarSteps[i].gameObject.SetActive(true);
                }
            }
            else
            {
                StartCoroutine(DeactivateStepWithDelay(healthBarSteps[i], (healthBarSteps.Length - 1 - i) * deactivateDelay));
            }
        }

        ApplyColorToSteps();
    }

    private void UpdateDynamicValues()
    {
        if (coreReference == null) return;

        float healthPercentage = Mathf.Clamp01(coreReference.currentHealth / coreReference.maxHealth);
        currentWaveSpeed = Mathf.Lerp(minWaveSpeed, maxWaveSpeed, healthPercentage);
        currentWaveFrequency = Mathf.Lerp(minWaveFrequency, maxWaveFrequency, healthPercentage);
        currentColor = Color.Lerp(minColor, maxColor, healthPercentage);
    }

    private void ApplyWaveEffect()
    {
        for (int i = 0; i < healthBarSteps.Length; i++)
        {
            if (healthBarSteps[i] == null || !healthBarSteps[i].gameObject.activeSelf) continue;

            float waveOffset = Mathf.Sin((Time.time * currentWaveSpeed) + (i * currentWaveFrequency));
            float targetHeight = Mathf.Lerp(minHeight, maxHeight, (waveOffset + 1f) / 2f);
            Vector3 newScale = healthBarSteps[i].localScale;
            newScale.y = targetHeight;
            healthBarSteps[i].localScale = newScale;
        }
    }

    private void ApplyColorToSteps()
    {
        foreach (Transform step in healthBarSteps)
        {
            if (step == null || !step.gameObject.activeSelf) continue;

            Image stepImage = step.GetComponent<Image>();
            if (stepImage != null)
            {
                stepImage.color = currentColor;
            }
        }
    }

    private System.Collections.IEnumerator DeactivateStepWithDelay(Transform step, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (step != null) step.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        foreach (Transform step in healthBarSteps)
        {
            if (step != null) LeanTween.cancel(step.gameObject);
        }
    }
}