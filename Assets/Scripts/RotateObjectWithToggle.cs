using UnityEngine;

public class RotateObjectWithToggle : MonoBehaviour
{
    // Velocidad de rotación
    [SerializeField]
    private float rotationSpeed = 100f;

    // Tiempo en segundos para cambiar de dirección
    [SerializeField]
    private float directionChangeInterval = 2f;

    // Toggle para habilitar/deshabilitar el cambio de dirección
    [SerializeField]
    private bool enableDirectionChange = true;

    // Variable interna para almacenar el tiempo transcurrido
    private float timeElapsed = 0f;

    // Dirección actual de la rotación (1 para horario, -1 para antihorario)
    private int rotationDirection = 1;

    void Update()
    {
        // Incrementa el tiempo transcurrido
        timeElapsed += Time.deltaTime;

        // Cambia la dirección si está habilitado y el intervalo se cumple
        if (enableDirectionChange && timeElapsed >= directionChangeInterval)
        {
            rotationDirection *= -1; // Cambia la dirección de rotación
            timeElapsed = 0f; // Reinicia el tiempo transcurrido
        }

        // Calcula la rotación en base al tiempo, velocidad y dirección
        float rotationAmount = rotationSpeed * rotationDirection * Time.deltaTime;

        // Aplica la rotación al objeto
        transform.Rotate(0, 0, rotationAmount);
    }
}
