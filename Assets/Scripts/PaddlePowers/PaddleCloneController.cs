using UnityEngine;

public class PaddleCloneController : MonoBehaviour
{
    public GameObject originalPaddle; // Referencia al Paddle original
    public Transform core; // Referencia al núcleo alrededor del cual rota
    private float orbitRadius; // Radio de la órbita del clon
    private float initialOffsetAngle; // Ángulo inicial del clon respecto al Paddle original

    void Start()
    {
        if (core == null || originalPaddle == null)
        {
            Debug.LogError("Falta una referencia al núcleo o al Paddle original.");
            return;
        }

        // Calcula el radio de la órbita
        orbitRadius = Vector3.Distance(core.position, transform.position);

        // Calcula el ángulo inicial relativo al Paddle original
        Vector3 directionToCore = transform.position - core.position;
        Vector3 directionToPaddle = originalPaddle.transform.position - core.position;

        float angleToCore = Mathf.Atan2(directionToCore.y, directionToCore.x) * Mathf.Rad2Deg;
        float angleToPaddle = Mathf.Atan2(directionToPaddle.y, directionToPaddle.x) * Mathf.Rad2Deg;

        initialOffsetAngle = Mathf.DeltaAngle(angleToPaddle, angleToCore);
    }

    void Update()
    {
        if (originalPaddle != null && core != null)
        {
            // Calcula el ángulo del Paddle original respecto al núcleo
            Vector3 originalDirection = originalPaddle.transform.position - core.position;
            float paddleAngle = Mathf.Atan2(originalDirection.y, originalDirection.x) * Mathf.Rad2Deg;

            // Calcula el ángulo actual del clon basado en el movimiento del Paddle
            float currentAngle = paddleAngle + initialOffsetAngle;

            // Convierte el ángulo a radianes para calcular la posición
            float radian = currentAngle * Mathf.Deg2Rad;

            // Actualiza la posición del clon en la órbita
            transform.position = new Vector3(
                core.position.x + Mathf.Cos(radian) * orbitRadius,
                core.position.y + Mathf.Sin(radian) * orbitRadius,
                core.position.z
            );

            // Ajusta la rotación del clon para que apunte hacia afuera de la órbita
            Vector3 directionFromCore = transform.position - core.position;
            float angleToFaceOutward = Mathf.Atan2(directionFromCore.y, directionFromCore.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angleToFaceOutward - 90f);
        }
    }
}
