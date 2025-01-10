using UnityEngine;

[CreateAssetMenu(menuName = "Paddle Powers/Clone Power")]
public class PaddlePowerClone : PaddlePowerBase
{
    public float cloneDuration = 5f; // Duración de los clones
    public Color cloneColor = Color.green; // Color de los clones
    public float tweenDuration = 0.5f; // Duración de los efectos de aparición y desaparición
    public int cloneCount = 1; // Número de clones

    public override void Activate(GameObject paddle)
    {
        Debug.Log("Paddle Power Clone activated!");

        // Encuentra el núcleo
        PaddleController paddleController = paddle.GetComponent<PaddleController>();
        if (paddleController == null || paddleController.core == null)
        {
            Debug.LogError("No se encontró el núcleo o el PaddleController.");
            return;
        }

        Transform core = paddleController.core;

        // Calcula la distancia entre el núcleo y el Paddle original
        float radius = Vector3.Distance(core.position, paddle.transform.position);

        // Obtén el ángulo inicial del Paddle original respecto al núcleo
        float initialAngle = GetAngleFromCore(core.position, paddle.transform.position);

        // Distribuir los clones uniformemente en la órbita
        float angleStep = 360f / (cloneCount + 1); // Espaciado angular entre clones

        for (int i = 1; i <= cloneCount; i++)
        {
            // Calcula el ángulo para el clon actual
            float cloneAngle = initialAngle + (angleStep * i) % 360; // Asegura que el ángulo esté dentro de [0, 360]

            // Calcula la posición del clon en la órbita
            Vector3 clonePosition = GetPositionOnOrbit(core.position, cloneAngle, radius);

            // Instancia el clon
            GameObject clone = Instantiate(paddle, clonePosition, paddle.transform.rotation);

            // Configura el clon
            SetCloneVisuals(clone, paddle.transform.localScale);

            // Agrega efectos de aparición
            Vector3 originalScale = paddle.transform.localScale; // Escala del Paddle original
            clone.transform.localScale = Vector3.zero; // Comienza invisible
            LeanTween.scale(clone, originalScale, tweenDuration).setEaseOutBounce();

            // Sincroniza el movimiento del clon con el original
            PaddleCloneController cloneController = clone.AddComponent<PaddleCloneController>();
            cloneController.originalPaddle = paddle;
            cloneController.core = core;

            // Destruye el clon después de la duración configurada con un efecto de desaparición
            LeanTween.scale(clone, Vector3.zero, tweenDuration)
                     .setEaseInBack()
                     .setDelay(cloneDuration)
                     .setOnComplete(() => Destroy(clone));
        }
    }

    private float GetAngleFromCore(Vector3 corePosition, Vector3 paddlePosition)
    {
        // Calcula el ángulo actual del Paddle respecto al núcleo
        Vector3 direction = paddlePosition - corePosition;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    private Vector3 GetPositionOnOrbit(Vector3 corePosition, float angle, float radius)
    {
        // Calcula una posición en la órbita del núcleo
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(
            corePosition.x + Mathf.Cos(radian) * radius,
            corePosition.y + Mathf.Sin(radian) * radius,
            corePosition.z
        );
    }

    private void SetCloneVisuals(GameObject clone, Vector3 originalScale)
    {
        // Cambia el color del clon
        SpriteRenderer spriteRenderer = clone.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = cloneColor;

            // Espeja la imagen del Paddle
            spriteRenderer.flipX = true;
        }

        // Desactiva el componente PaddlePower para que los clones no puedan activar poderes
        PaddlePower paddlePower = clone.GetComponent<PaddlePower>();
        if (paddlePower != null)
        {
            paddlePower.enabled = false;
        }

        // Asegura que la escala inicial del clon coincida con la del original
        clone.transform.localScale = originalScale;
    }
}
