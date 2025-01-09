using UnityEngine;

public class DataShardBehavior : MonoBehaviour
{
    private Transform coreTransform;

    [Header("Movement Settings")]
    public float moveSpeed = 5f; // Velocidad base hacia el núcleo
    public AnimationCurve speedCurve; // Curva para controlar la velocidad hacia el núcleo

    private bool isQuitting = false; // Para evitar buscar el núcleo al cerrar la aplicación

    public void Initialize()
    {
        // Marca si la aplicación se está cerrando
        GameObject core = GameObject.FindGameObjectWithTag("Core");

        if (core != null)
        {
            coreTransform = core.transform;
        }
        else if (!isQuitting)
        {
            Debug.LogError("No se encontró un objeto con el tag 'Core' en la escena.");
        }

        // Comienza el movimiento inmediato hacia el núcleo
        StartCoroutine(MoveToCore());
    }

    private void OnApplicationQuit()
    {
        isQuitting = true; // Marca que la aplicación está cerrándose
    }

    private System.Collections.IEnumerator MoveToCore()
    {
        float elapsedTime = 0f;
        float totalDuration = speedCurve.keys[speedCurve.length - 1].time; // Duración total de la curva

        while (coreTransform != null && Vector3.Distance(transform.position, coreTransform.position) > 0.1f)
        {
            // Calcula la velocidad usando la curva
            float curveValue = speedCurve.Evaluate(elapsedTime / totalDuration);
            float currentSpeed = moveSpeed * curveValue;

            transform.position = Vector3.MoveTowards(transform.position, coreTransform.position, currentSpeed * Time.deltaTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Destruye el Data Shard al llegar al núcleo o si no hay núcleo
        Destroy(gameObject);
    }
}
