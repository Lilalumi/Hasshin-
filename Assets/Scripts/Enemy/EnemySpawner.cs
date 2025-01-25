using UnityEngine;
using System.Collections.Generic; // Para manejar listas de posiciones

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public EnemySpawnSetting spawnSettings; // Configuración de spawn basada en ScriptableObject

    private float waveTimer; // Temporizador para controlar las oleadas
    private int currentWave = 0; // Contador de oleadas completadas
    private List<Vector2> spawnedPositions = new List<Vector2>(); // Lista de posiciones de enemigos spawneados
    private bool spawningCompleted = false; // Bandera para detener el spawner

    void Start()
    {
        if (spawnSettings == null)
        {
            Debug.LogError("No se asignó un EnemySpawnSetting al spawner.");
            return;
        }

        SpawnWave(); // Inicia la primera oleada inmediatamente al iniciar la escena
        waveTimer = spawnSettings.timeBetweenWaves; // Configura el temporizador para la siguiente oleada
    }

    void Update()
    {
        if (PauseManager.IsPaused) return; // Detener la lógica de respawn
        if (spawningCompleted) return; // Detiene el spawner si se completaron las oleadas

        // Temporizador para iniciar la siguiente oleada
        waveTimer -= Time.deltaTime;
        if (waveTimer <= 0)
        {
            if (currentWave >= spawnSettings.maxWaves)
            {
                spawningCompleted = true; // Marca que las oleadas están completas
                return;
            }

            SpawnWave();
            waveTimer = spawnSettings.timeBetweenWaves; // Reinicia el temporizador
        }
    }

    void SpawnWave()
    {
        spawnedPositions.Clear(); // Limpia las posiciones de la oleada anterior

        foreach (var enemyEntry in spawnSettings.enemyWaveConfigs)
        {
            for (int i = 0; i < enemyEntry.count; i++)
            {
                SpawnEnemy(enemyEntry.enemyConfig);
            }
        }

        currentWave++; // Incrementa el contador de oleadas
    }

    void SpawnEnemy(EnemyConfig enemyConfig)
    {
        Vector2 spawnPosition;

        // Intenta spawnear en una posición válida respetando el espaciado mínimo
        int attempts = 0;
        do
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad; // Ángulo aleatorio en radianes
            Vector2 offset = new Vector2(
                Mathf.Cos(angle) * spawnSettings.spawnRadius,
                Mathf.Sin(angle) * spawnSettings.spawnRadius
            );

            spawnPosition = (Vector2)transform.position + offset;
            attempts++;
        }
        while (!IsPositionValid(spawnPosition) && attempts < 100); // Máximo 100 intentos para evitar bucles infinitos

        // Agrega la posición válida a la lista de posiciones
        spawnedPositions.Add(spawnPosition);

        // Instancia el enemigo en la posición calculada
        GameObject enemy = Instantiate(spawnSettings.enemyPrefab, spawnPosition, Quaternion.identity);

        // Configura el objeto enemigo
        EnemyBehavior enemyBehavior = enemy.GetComponent<EnemyBehavior>();
        if (enemyBehavior != null)
        {
            enemyBehavior.SetConfig(enemyConfig);
        }
        else
        {
            Debug.LogWarning("El prefab del enemigo no tiene un componente EnemyBehavior.");
        }

        // Configura el objeto enemigo como hijo del spawner
        enemy.transform.parent = this.transform;
    }

    bool IsPositionValid(Vector2 position)
    {
        foreach (Vector2 existingPosition in spawnedPositions)
        {
            if (Vector2.Distance(existingPosition, position) < spawnSettings.minSpawnSpacing)
            {
                return false; // Si la distancia es menor al espaciado mínimo, la posición no es válida
            }
        }

        return true; // Si todas las distancias son válidas, la posición es válida
    }

    public bool HasPendingWaves()
    {
        return currentWave < spawnSettings.maxWaves; // Devuelve true si aún hay oleadas pendientes
    }
}
