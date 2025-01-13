using UnityEngine;

[CreateAssetMenu(menuName = "Ball Powers/Split Power")]
public class BallPowerSplit : BallPowerBase
{
    public GameObject ballPrefab; // Prefab de la pelota
    public int numberOfBalls = 2; // Cantidad de pelotas adicionales
    public float directionRange = 45f; // Rango de amplitud para la dirección aleatoria en grados
    public float spawnOffset = 0.5f; // Distancia de separación entre las pelotas instanciadas

    public bool disappearAfterTime = false; // Toggle para desaparición por tiempo
    public float lifetime = 5f; // Tiempo de vida de las pelotas

    public bool disappearAfterCollisions = false; // Toggle para desaparición por colisiones
    public int maxCollisions = 3; // Máxima cantidad de colisiones antes de desaparecer

    public override void Activate(GameObject ballController)
    {
        // Encuentra todas las pelotas activas
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");

        if (balls.Length == 0)
        {
            Debug.LogWarning("No se encontraron pelotas activas para aplicar el poder.");
            return;
        }

        foreach (GameObject ball in balls)
        {
            for (int i = 0; i < numberOfBalls; i++)
            {
                // Calcula un offset circular para distribuir las pelotas alrededor de la pelota actual
                float angleOffset = (360f / numberOfBalls) * i; // Espaciado angular entre pelotas
                Vector3 offset = Quaternion.Euler(0, 0, angleOffset) * Vector3.up * spawnOffset;

                // Instancia nuevas pelotas alrededor de la posición de la pelota actual
                Vector3 spawnPosition = ball.transform.position + offset;
                GameObject newBall = Instantiate(ballPrefab, spawnPosition, Quaternion.identity);

                // Asigna la nueva pelota como hija del BallController
                newBall.transform.parent = ballController.transform;

                // Calcula una dirección aleatoria dentro del rango definido
                float randomAngle = Random.Range(-directionRange / 2f, directionRange / 2f);
                Vector2 direction = Quaternion.Euler(0, 0, randomAngle) * Vector2.up;

                // Asigna la dirección aleatoria como velocidad inicial de la pelota
                Rigidbody2D newBallRb = newBall.GetComponent<Rigidbody2D>();
                if (newBallRb != null)
                {
                    newBallRb.velocity = direction * newBallRb.velocity.magnitude;
                }

                // Agregar el comportamiento de desaparición
                BallLifetimeHandler lifetimeHandler = newBall.AddComponent<BallLifetimeHandler>();
                if (disappearAfterTime)
                {
                    lifetimeHandler.SetLifetime(lifetime);
                }

                if (disappearAfterCollisions)
                {
                    lifetimeHandler.SetMaxCollisions(maxCollisions);
                }
            }
        }
    }
}
