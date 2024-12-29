using UnityEngine;
using System.Collections.Generic; // Para manejar listas de posiciones

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Prefab del enemigo a spawnear
    public Transform border; // Objeto Border con la forma circular
    public int enemiesPerWave = 5; // Cantidad de enemigos por oleada
    public float timeBetweenWaves = 5f; // Tiempo entre oleadas en segundos
    public float spawnRadius = 5f; // Radio de spawn alrededor de Border
    public float minSpawnSpacing = 1.5f; // Espaciado mínimo entre enemigos
    public int maxWaves = 5; // Cantidad máxima de oleadas

    private float waveTimer; // Temporizador para controlar las oleadas
    private int currentWave = 0; // Contador de oleadas completadas
    private List<Vector2> spawnedPositions = new List<Vector2>(); // Lista de posiciones de enemigos spawneados
    private bool spawningCompleted = false; // Bandera para detener el spawner

    void Start()
    {
        // Inicia la primera oleada inmediatamente al iniciar la escena
        SpawnWave();
        waveTimer = timeBetweenWaves; // Configura el temporizador para la siguiente oleada
    }

    void Update()
    {
        if (spawningCompleted) return; // Detiene el spawner si se completaron las oleadas

        // Temporizador para iniciar la siguiente oleada
        waveTimer -= Time.deltaTime;
        if (waveTimer <= 0)
        {
            if (currentWave >= maxWaves)
            {
                spawningCompleted = true; // Marca que las oleadas están completas
                return;
            }

            SpawnWave();
            waveTimer = timeBetweenWaves; // Reinicia el temporizador
        }
    }

    void SpawnWave()
    {
        spawnedPositions.Clear(); // Limpia las posiciones de la oleada anterior

        for (int i = 0; i < enemiesPerWave; i++)
        {
            SpawnEnemy();
        }

        currentWave++; // Incrementa el contador de oleadas
    }

    void SpawnEnemy()
    {
        Vector2 spawnPosition;

        // Intenta spawnear en una posición válida respetando el espaciado mínimo
        int attempts = 0;
        do
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad; // Ángulo aleatorio en radianes
            Vector2 offset = new Vector2(
                Mathf.Cos(angle) * spawnRadius,
                Mathf.Sin(angle) * spawnRadius
            );

            spawnPosition = (Vector2)border.position + offset;
            attempts++;
        }
        while (!IsPositionValid(spawnPosition) && attempts < 100); // Máximo 100 intentos para evitar bucles infinitos

        // Agrega la posición válida a la lista de posiciones
        spawnedPositions.Add(spawnPosition);

        // Instancia el enemigo en la posición calculada
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        // Configura el objeto enemigo como hijo del spawner
        enemy.transform.parent = this.transform;
    }

    bool IsPositionValid(Vector2 position)
    {
        foreach (Vector2 existingPosition in spawnedPositions)
        {
            if (Vector2.Distance(existingPosition, position) < minSpawnSpacing)
            {
                return false; // Si la distancia es menor al espaciado mínimo, la posición no es válida
            }
        }

        return true; // Si todas las distancias son válidas, la posición es válida
    }
}
