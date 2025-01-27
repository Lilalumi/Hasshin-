using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("Health UI Settings")]
    public Transform[] healthBarSteps; // Referencia directa a los pasos de HealthBarStep en el orden correcto
    public string coreTag = "Core"; // Tag del Core para localizarlo en la escena
    public float animationDuration = 0.1f; // Duración de la animación al cambiar la vida
    public float minHeight = 0.5f; // Altura mínima del paso
    public float maxHeight = 1.5f; // Altura máxima del paso
    public float deactivateDelay = 0.02f; // Retraso constante entre la desactivación de cada barra

    [Header("Wave Settings")]
    public float maxWaveSpeed = 3f; // Velocidad máxima de propagación de la onda
    public float minWaveSpeed = 1f; // Velocidad mínima de propagación de la onda
    public float maxWaveFrequency = 0.8f; // Frecuencia máxima de la onda
    public float minWaveFrequency = 0.3f; // Frecuencia mínima de la onda

    [Header("Color Settings")]
    public Color maxColor = Color.green; // Color máximo cuando la salud está al 100%
    public Color minColor = Color.red; // Color mínimo cuando la salud está al 0%

    private Core coreReference; // Referencia al script Core del objeto en la escena
    private float currentWaveSpeed; // Velocidad actual de la onda
    private float currentWaveFrequency; // Frecuencia actual de la onda
    private Color currentColor; // Color actual de los HealthBarStep

    private void Start()
    {
        // Validar Core
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

        // Validar HealthBarSteps
        if (healthBarSteps == null || healthBarSteps.Length == 0)
        {
            Debug.LogError("HealthBarSteps no está asignado o está vacío. Asigne los pasos desde el Inspector.");
            return;
        }

        // Configurar el estado inicial de la barra de salud
        UpdateHealthBar(coreReference.currentHealth, true);
    }

    private void Update()
    {
        if (coreReference != null)
        {
            // Actualizar la barra de salud basándose en la vida actual del Core
            UpdateHealthBar(coreReference.currentHealth, false);

            // Actualizar los valores dinámicos de Wave Speed, Wave Frequency y Color
            UpdateDynamicValues();
        }

        // Aplicar el efecto de ola en tiempo real
        ApplyWaveEffect();
    }

    private void UpdateHealthBar(float currentHealth, bool instant)
    {
        if (coreReference == null || healthBarSteps == null) return;

        // Calcular el porcentaje de vida y el número de pasos a mostrar
        float healthPercentage = Mathf.Clamp01(currentHealth / coreReference.maxHealth);
        int activeSteps = Mathf.RoundToInt(healthPercentage * healthBarSteps.Length);

        // Actualizar la visibilidad de los pasos en secuencia
        for (int i = healthBarSteps.Length - 1; i >= 0; i--) // Recorre de derecha a izquierda
        {
            if (healthBarSteps[i] == null) continue;

            bool shouldBeActive = i < activeSteps;

            if (shouldBeActive)
            {
                // Asegurarse de que el paso esté activo
                if (!healthBarSteps[i].gameObject.activeSelf)
                {
                    healthBarSteps[i].gameObject.SetActive(true);
                }
            }
            else
            {
                // Desactivar pasos de derecha a izquierda con un retraso fijo basado en el índice
                StartCoroutine(DeactivateStepWithDelay(healthBarSteps[i], (healthBarSteps.Length - 1 - i) * deactivateDelay));
            }
        }

        // Aplicar el color actualizado a los pasos activos
        ApplyColorToSteps();
    }




    private void UpdateDynamicValues()
    {
        if (coreReference == null) return;

        // Calcular el porcentaje de salud del Core
        float healthPercentage = Mathf.Clamp01(coreReference.currentHealth / coreReference.maxHealth);

        // Interpolar los valores de Wave Speed y Wave Frequency en función de la salud
        currentWaveSpeed = Mathf.Lerp(minWaveSpeed, maxWaveSpeed, healthPercentage);
        currentWaveFrequency = Mathf.Lerp(minWaveFrequency, maxWaveFrequency, healthPercentage);

        // Interpolar el color en función de la salud
        currentColor = Color.Lerp(minColor, maxColor, healthPercentage);

        // Log cuando se asigna el color Max Color
        if (currentColor == maxColor)
        {
            Debug.Log("El color asignado es el Max Color: " + maxColor);
        }
    }

    private void ApplyWaveEffect()
    {
        for (int i = 0; i < healthBarSteps.Length; i++)
        {
            if (healthBarSteps[i] == null || !healthBarSteps[i].gameObject.activeSelf) continue;

            // Calcular el valor de la onda para este paso basado en el índice y el tiempo
            float waveOffset = Mathf.Sin((Time.time * currentWaveSpeed) + (i * currentWaveFrequency));
            float targetHeight = Mathf.Lerp(minHeight, maxHeight, (waveOffset + 1f) / 2f); // Mapear [-1, 1] a [minHeight, maxHeight]

            // Aplicar la altura al paso
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

        if (step != null)
        {
            step.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        // Cancela todos los tweens de los pasos
        foreach (Transform step in healthBarSteps)
        {
            if (step != null)
            {
                LeanTween.cancel(step.gameObject);
            }
        }
    }
}
