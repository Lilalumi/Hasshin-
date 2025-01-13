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

        Vector3 originalPosition = paddle.transform.position; // Posición inicial del Paddle
        Vector3 direction = (paddle.transform.position - core.transform.position).normalized; // Dirección opuesta al Core
        Vector3 targetPosition = originalPosition + direction * bumpDistance; // Posición objetivo alejada del Core

        // Fase 1: Mover hacia afuera
        float elapsedTime = 0f;
        while (elapsedTime < bumpDistance / bumpSpeed)
        {
            paddle.transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / (bumpDistance / bumpSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        paddle.transform.position = targetPosition;

        // Fase 2: Volver a la posición original
        elapsedTime = 0f;
        while (elapsedTime < bumpDistance / bumpSpeed)
        {
            paddle.transform.position = Vector3.Lerp(targetPosition, originalPosition, elapsedTime / (bumpDistance / bumpSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        paddle.transform.position = originalPosition;

        Debug.Log("Paddle Power Bump completed!");
    }
}
