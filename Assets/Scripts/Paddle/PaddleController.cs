using UnityEngine;

public class PaddleController : MonoBehaviour
{
    public Transform core; // Núcleo central alrededor del cual rotará
    public float speed = 200f; // Velocidad de rotación
    public float lerpSpeed = 5f; // Velocidad de interpolación para el movimiento con mouse

    void Update()
    {
        switch (ControlSettings.GetCurrentMode())
        {
            case ControlMode.Keyboard:
                HandleKeyboardInput();
                break;
            case ControlMode.Mouse:
                HandleMouseInput();
                break;
            // Se puede agregar el caso para Gamepad en el futuro
        }
    }

    private void HandleKeyboardInput()
    {
        float input = Input.GetAxis("Horizontal");
        if (input != 0)
        {
            transform.RotateAround(core.position, Vector3.forward, -input * speed * Time.deltaTime);
        }
    }

    private void HandleMouseInput()
    {
        // Obtiene la posición del mouse en el mundo
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0; // Ignora la profundidad

        // Calcula el ángulo hacia el mouse desde el núcleo
        Vector3 direction = (mouseWorldPosition - core.position).normalized;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Calcula el ángulo actual del Paddle
        Vector3 paddleDirection = transform.position - core.position;
        float currentAngle = Mathf.Atan2(paddleDirection.y, paddleDirection.x) * Mathf.Rad2Deg;

        // Interpola el ángulo actual hacia el ángulo objetivo
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, speed * Time.deltaTime);

        // Actualiza la posición del Paddle
        float distanceToCore = Vector3.Distance(core.position, transform.position);
        transform.position = core.position + new Vector3(
            Mathf.Cos(newAngle * Mathf.Deg2Rad),
            Mathf.Sin(newAngle * Mathf.Deg2Rad),
            0
        ) * distanceToCore;

        // Ajusta la rotación del Paddle para que mire hacia afuera
        transform.rotation = Quaternion.Euler(0f, 0f, newAngle - 90f);
    }
}
