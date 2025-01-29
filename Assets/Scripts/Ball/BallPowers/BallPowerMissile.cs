using UnityEngine;

[CreateAssetMenu(menuName = "Ball Powers/Missile Power")]
public class BallPowerMissile : BallPowerBase
{
    public GameObject missilePrefab; // Prefab del misil
    public int missileCount = 1; // Cantidad de misiles a instanciar
    public float initialFlightTime = 1f; // Tiempo durante el cual los misiles vuelan en una dirección aleatoria
    public float initialSpeed = 5f; // Velocidad inicial de los misiles

    public override void Activate(GameObject ballController)
    {
        // Encuentra todas las pelotas
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
        if (balls.Length == 0)
        {
            return; // No hay pelotas activas para lanzar el misil.
        }

        // Instancia los misiles desde cada pelota
        foreach (GameObject ball in balls)
        {
            InstantiateMissilesFromBall(ball);
        }
    }

    private void InstantiateMissilesFromBall(GameObject ball)
    {
        if (missilePrefab == null)
        {
            return; // No se asignó un prefab de misil en el ScriptableObject.
        }

        for (int i = 0; i < missileCount; i++)
        {
            // Genera una dirección aleatoria
            float randomAngle = Random.Range(0f, 360f);
            Vector2 randomDirection = Quaternion.Euler(0, 0, randomAngle) * Vector2.up;

            // Instancia el misil en el centro de la pelota
            GameObject missile = Instantiate(missilePrefab, ball.transform.position, Quaternion.identity);

            // Configura el movimiento inicial del misil
            Rigidbody2D missileRb = missile.GetComponent<Rigidbody2D>();
            if (missileRb != null)
            {
                missileRb.velocity = randomDirection * initialSpeed;
            }

            // Configura el tiempo inicial de vuelo antes de rastrear objetivos
            Missile missileScript = missile.GetComponent<Missile>();
            if (missileScript != null)
            {
                missileScript.SetInitialFlightTime(initialFlightTime);
            }
        }
    }
}
