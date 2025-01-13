using UnityEngine;

[CreateAssetMenu(menuName = "Ball Powers/Orbital Power")]
public class BallPowerOrbital : BallPowerBase
{
    public GameObject asteroidPrefab; // Prefab del Asteroid
    public float range = 2f; // Distancia de la órbita al núcleo de la Ball
    public int amount = 3; // Cantidad de asteroides a instanciar
    public float orbitSpeed = 50f; // Velocidad de la órbita
    public float duration = 5f; // Duración del poder antes de desaparecer

    public override void Activate(GameObject ballController)
    {
        // Encuentra todas las pelotas
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
        if (balls.Length == 0)
        {
            Debug.LogWarning("No hay pelotas activas para instanciar asteroides.");
            return;
        }

        // Instancia los asteroides alrededor de cada pelota
        foreach (GameObject ball in balls)
        {
            ball.GetComponent<MonoBehaviour>().StartCoroutine(SpawnAsteroids(ball));
        }
    }

    private System.Collections.IEnumerator SpawnAsteroids(GameObject ball)
    {
        if (asteroidPrefab == null)
        {
            Debug.LogError("No se asignó un prefab de Asteroid en el ScriptableObject.");
            yield break;
        }

        for (int i = 0; i < amount; i++)
        {
            // Calcula el ángulo en radianes para distribuir equidistantemente
            float angle = (i * 360f / amount) * Mathf.Deg2Rad;

            // Calcula la posición inicial de cada asteroide
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * range;
            Vector3 spawnPosition = ball.transform.position + offset;

            GameObject asteroid = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);

            // Configura el asteroide
            Asteroid asteroidScript = asteroid.GetComponent<Asteroid>();
            if (asteroidScript != null)
            {
                asteroidScript.SetOrbit(ball.transform, range, orbitSpeed, angle); // Pasa el ángulo inicial
            }

            // Destruye el asteroide después de la duración del poder
            Destroy(asteroid, duration);
        }

        yield return null;
    }

}
