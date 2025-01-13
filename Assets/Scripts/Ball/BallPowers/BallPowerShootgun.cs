using UnityEngine;

[CreateAssetMenu(menuName = "Ball Powers/Shootgun Power")]
public class BallPowerShootgun : BallPowerBase
{
    public GameObject pelletPrefab; // Prefab del proyectil
    public int pelletCount = 5; // Cantidad de pellets a disparar
    public float spreadRadius = 30f; // Radio de dispersión en grados
    public float pelletSpeed = 10f; // Velocidad de los pellets
    public float maxDistance = 5f; // Distancia máxima que pueden alcanzar los pellets antes de desaparecer
    public float spawnDelayMin = 0.05f; // Tiempo mínimo entre cada spawn
    public float spawnDelayMax = 0.15f; // Tiempo máximo entre cada spawn

    public override void Activate(GameObject ballController)
    {
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
        if (balls.Length == 0)
        {
            Debug.LogWarning("No hay pelotas activas para disparar pellets.");
            return;
        }

        foreach (GameObject ball in balls)
        {
            ball.GetComponent<MonoBehaviour>().StartCoroutine(ShootPelletsFromBall(ball));
        }
    }

    private System.Collections.IEnumerator ShootPelletsFromBall(GameObject ball)
    {
        if (pelletPrefab == null)
        {
            Debug.LogError("No se asignó un prefab de pellet en el ScriptableObject.");
            yield break;
        }

        Rigidbody2D ballRb = ball.GetComponent<Rigidbody2D>();
        if (ballRb == null)
        {
            Debug.LogError("El objeto Ball no tiene un componente Rigidbody2D.");
            yield break;
        }

        Vector2 ballDirection = ballRb.velocity.normalized;

        for (int i = 0; i < pelletCount; i++)
        {
            float spreadAngle = Random.Range(-spreadRadius / 2f, spreadRadius / 2f);
            Vector2 spreadDirection = Quaternion.Euler(0, 0, spreadAngle) * ballDirection;

            GameObject pellet = Instantiate(pelletPrefab, ball.transform.position, Quaternion.identity);

            Rigidbody2D pelletRb = pellet.GetComponent<Rigidbody2D>();
            if (pelletRb != null)
            {
                pelletRb.velocity = spreadDirection * pelletSpeed;
            }

            Pellet pelletScript = pellet.GetComponent<Pellet>();
            if (pelletScript != null)
            {
                pelletScript.SetMaxDistance(maxDistance);
            }

            float spawnDelay = Random.Range(spawnDelayMin, spawnDelayMax);
            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
