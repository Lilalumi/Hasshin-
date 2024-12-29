using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject ballPrefab; // Prefab de la pelota
    public Transform ballSpawnPosition; // Posición donde se instanciará la pelota
    public Transform ballController; // Objeto que será el padre de la pelota instanciada

    void Start()
    {
        // Verifica que las referencias necesarias estén asignadas
        if (ballPrefab == null || ballSpawnPosition == null || ballController == null)
        {
            Debug.LogError("BallSpawner: Falta una referencia al prefab, BallSpawnPosition o BallController.");
            return;
        }

        // Instancia la pelota en la posición de BallSpawnPosition
        GameObject spawnedBall = Instantiate(ballPrefab, ballSpawnPosition.position, Quaternion.identity);

        // Asigna el objeto BallController como el padre de la pelota instanciada
        spawnedBall.transform.parent = ballController;
    }
}
