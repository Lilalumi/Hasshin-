using UnityEngine;
using UnityEngine.Rendering.Universal; // Para Light 2D

public class BallShockwave : MonoBehaviour
{
    public GameObject shockwavePrefab; // Prefab del Shockwave
    public float waveDuration = 1f; // Duración de la onda en segundos
    public float waveSpeed = 5f; // Velocidad de expansión de la onda
    public float minLight = 0.5f; // Intensidad mínima de la luz
    public float maxLight = 2f; // Intensidad máxima de la luz

    private Light2D ballLight; // Referencia a la luz 2D del objeto Ball
    private float targetLightIntensity; // Valor objetivo de la luz
    private float currentLightIntensity; // Intensidad actual de la luz

    void Start()
    {
        ballLight = GetComponent<Light2D>();
        if (ballLight == null)
        {
            Debug.LogError("El objeto Ball no tiene un componente Light2D.");
            return;
        }

        // Inicializa la luz al valor mínimo
        ballLight.intensity = minLight;
        currentLightIntensity = minLight;
        targetLightIntensity = minLight;
    }

    void Update()
    {
        // Gradualmente ajusta la intensidad actual hacia la intensidad objetivo
        if (ballLight != null)
        {
            ballLight.intensity = Mathf.Lerp(ballLight.intensity, targetLightIntensity, Time.deltaTime / waveDuration);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Instancia el prefab de la onda de choque
        if (shockwavePrefab != null)
        {
            GameObject shockwave = Instantiate(shockwavePrefab, collision.contacts[0].point, Quaternion.identity);

            // Asigna el Shockwave como hijo de la pelota que lo generó
            shockwave.transform.SetParent(transform);

            // Configura el Shockwave para destruirse al finalizar la animación
            StartCoroutine(HandleShockwave(shockwave));
        }

        // Configura el objetivo de la intensidad de la luz al valor máximo
        if (ballLight != null)
        {
            targetLightIntensity = maxLight;
            StartCoroutine(HandleLightPulse());
        }
    }

    private System.Collections.IEnumerator HandleShockwave(GameObject shockwave)
    {
        Renderer renderer = shockwave.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError("Shockwave prefab does not have a Renderer component!");
            yield break;
        }

        Material material = renderer.material;
        float elapsedTime = 0f;

        while (elapsedTime < waveDuration)
        {
            elapsedTime += Time.deltaTime;
            float waveDistance = elapsedTime * waveSpeed;
            material.SetFloat("_WaveDistanceFromCenter", waveDistance);
            yield return null;
        }

        // Asegura que el objeto Shockwave se destruya al finalizar la animación
        if (shockwave != null)
        {
            Destroy(shockwave);
        }
    }

    private System.Collections.IEnumerator HandleLightPulse()
    {
        float elapsedTime = 0f;

        // Incrementa la luz hasta alcanzar el máximo durante la duración de la onda
        while (elapsedTime < waveDuration)
        {
            elapsedTime += Time.deltaTime;
            currentLightIntensity = Mathf.Lerp(minLight, maxLight, elapsedTime / waveDuration);
            ballLight.intensity = currentLightIntensity;
            yield return null;
        }

        // Configura el objetivo de la intensidad de la luz para regresar al mínimo
        targetLightIntensity = minLight;
    }
}
