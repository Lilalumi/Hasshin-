using UnityEngine;
using TMPro;

public class PercentageCounter : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI textMeshPro; // Asigna el componente TextMeshPro - Text (UI) desde el Inspector

    [Header("Counter Settings")]
    [SerializeField] private int framesToComplete = 300; // Cantidad de frames para ir de %00.00 a %99.99
    [SerializeField] private bool isCounting = false; // Toggle para activar/desactivar la cuenta

    private float currentValue = 0f; // Valor actual del contador
    private float incrementPerFrame; // Incremento por frame

    void Start()
    {
        // Calcula cuánto aumentar en cada frame
        incrementPerFrame = 99.99f / framesToComplete;

        // Inicializa el texto en 0%
        UpdateText();
    }

    void Update()
    {
        // Solo realiza la cuenta si el toggle está activo
        if (isCounting && currentValue < 99.99f)
        {
            // Incrementa el valor actual
            currentValue += incrementPerFrame;

            // Asegúrate de no superar el 99.99%
            currentValue = Mathf.Min(currentValue, 99.99f);

            // Actualiza el texto
            UpdateText();
        }
    }

    private void UpdateText()
    {
        // Actualiza el texto con el formato %00.00
        textMeshPro.text = $"%{currentValue:00.00}";
    }

    // Método para activar/desactivar la cuenta dinámicamente
    public void ToggleCounting(bool toggle)
    {
        isCounting = toggle;
    }
}
