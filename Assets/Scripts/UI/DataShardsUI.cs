using UnityEngine;
using TMPro;
using System.Collections;

public class DataShardsUI : MonoBehaviour
{
    public DataShardsController dataShardsController; // Referencia al controlador de Data Shards
    public TextMeshProUGUI dataShardsText; // Referencia al componente TextMeshPro

    [Header("Animation Settings")]
    public AnimationCurve speedCurve = AnimationCurve.Linear(0, 1, 1, 0); // Curva de velocidad de la animación
    public float totalAnimationDuration = 1f; // Duración total de la animación
    public Color changeColor = Color.yellow; // Color de transición durante la animación
    public float colorDuration = 0.2f; // Duración del cambio de color

    private int lastDataShards = -1; // Almacena la cantidad previa de Data Shards
    private Color originalColor; // Almacena el color original del texto
    private Coroutine activeCoroutine; // Referencia a la animación activa

    void Start()
    {
        if (dataShardsController == null)
        {
            Debug.LogError("DataShardsController no asignado en el inspector.");
        }

        if (dataShardsText == null)
        {
            Debug.LogError("TextMeshProUGUI no asignado en el inspector.");
        }

        // Almacena el color original
        originalColor = dataShardsText.color;

        // Inicializa el texto
        lastDataShards = dataShardsController.GetDataShards();
        dataShardsText.text = $"DATASHARDS: {lastDataShards:0000}";
    }

    void Update()
    {
        UpdateDataShardsUI();
    }

    void UpdateDataShardsUI()
    {
        if (dataShardsController != null && dataShardsText != null)
        {
            int dataShards = dataShardsController.GetDataShards();

            if (dataShards != lastDataShards) // Solo actualiza si hay un cambio
            {
                // Cancela la animación activa, si existe
                if (activeCoroutine != null)
                {
                    StopCoroutine(activeCoroutine);
                }

                // Inicia una nueva animación
                activeCoroutine = StartCoroutine(AnimateNumberIncrement(lastDataShards, dataShards));
                lastDataShards = dataShards;
            }
        }
    }

    private IEnumerator AnimateNumberIncrement(int oldNumber, int newNumber)
    {
        int currentNumber = oldNumber;
        float elapsedTime = 0f;
        float stepTime = totalAnimationDuration / Mathf.Max(1, newNumber - oldNumber); // Tiempo por paso basado en los incrementos

        while (currentNumber < newNumber)
        {
            currentNumber++;

            // Actualiza el texto y aplica el efecto de color
            UpdateTextWithColor(currentNumber);

            // Calcula el retraso basado en la curva
            float progress = (float)(currentNumber - oldNumber) / (newNumber - oldNumber); // Progreso normalizado
            float delay = speedCurve.Evaluate(progress) * stepTime;

            elapsedTime += delay;
            yield return new WaitForSeconds(delay);
        }

        // Asegura que el color vuelva al original
        dataShardsText.color = originalColor;
        activeCoroutine = null; // Limpia la referencia al completar
    }

    private void UpdateTextWithColor(int number)
    {
        // Cambia temporalmente el color
        LeanTween.value(gameObject, UpdateTextColor, changeColor, originalColor, colorDuration);
        dataShardsText.text = $"DATASHARDS: {number:0000}";
    }

    private void UpdateTextColor(Color color)
    {
        dataShardsText.color = color;
    }
}
