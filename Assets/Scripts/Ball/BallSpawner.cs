using UnityEngine;
using System; // Necesario para usar Action<>

public class BallSpawner : MonoBehaviour
{
    public GameObject ballPrefab; // Prefab de la pelota
    public Transform ballSpawnPosition; // Posición donde se instanciará la pelota
    public Transform ballController; // Objeto que será el padre de la pelota instanciada
    public event Action<GameObject> OnBallSpawned; // Evento que se activa al instanciar una pelota

    void Start()
    {
        // Verifica que las referencias necesarias estén asignadas
        if (ballPrefab == null || ballSpawnPosition == null || ballController == null)
        {
            return;
        }

        // Instancia la pelota en la posición de BallSpawnPosition
        GameObject spawnedBall = Instantiate(ballPrefab, ballSpawnPosition.position, Quaternion.identity);

        // Asigna el objeto BallController como el padre de la pelota instanciada
        spawnedBall.transform.parent = ballController;

        // Disparar el evento
        OnBallSpawned?.Invoke(spawnedBall);
    }
}
