using UnityEngine;

[CreateAssetMenu(menuName = "Paddle Powers/Bump Power")]
public class PaddlePowerBump : PaddlePowerBase
{
    public float bumpSpeed = 10f; // Velocidad del impulso
    public float bumpDistance = 2f; // Distancia que se aleja del Core

    public override void Activate(GameObject paddle)
    {
        Debug.Log("Paddle Power Bump activated!");
        paddle.GetComponent<MonoBehaviour>().StartCoroutine(BumpEffect(paddle));
    }

    private System.Collections.IEnumerator BumpEffect(GameObject paddle)
    {
        // Encuentra el Core
        GameObject core = GameObject.FindGameObjectWithTag("Core");
        if (core == null)
        {
            Debug.LogError("No se encontró un objeto con el tag 'Core'.");
            yield break;
        }

        // Guarda la posición y rotación originales del Paddle
        Vector3 originalPosition = paddle.transform.position;
        Quaternion originalRotation = paddle.transform.rotation;

        // Calcula la dirección del impulso
        Vector3 direction = (paddle.transform.position - core.transform.position).normalized;
        Vector3 targetPosition = originalPosition + direction * bumpDistance;

        // Fase 1: Mover hacia afuera
        float elapsedTime = 0f;
        float duration = bumpDistance / bumpSpeed;
        while (elapsedTime < duration)
        {
            float progress = elapsedTime / duration;
            paddle.transform.position = Vector3.Lerp(originalPosition, targetPosition, progress);
            paddle.transform.rotation = originalRotation; // Mantiene la rotación original
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        paddle.transform.position = targetPosition;
        paddle.transform.rotation = originalRotation;

        // Fase 2: Volver a la posición original
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float progress = elapsedTime / duration;
            paddle.transform.position = Vector3.Lerp(targetPosition, originalPosition, progress);
            paddle.transform.rotation = originalRotation; // Mantiene la rotación original
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        paddle.transform.position = originalPosition;
        paddle.transform.rotation = originalRotation;

        Debug.Log("Paddle Power Bump completed!");
    }
}
