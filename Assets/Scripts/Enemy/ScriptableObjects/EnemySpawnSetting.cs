using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemySpawnSetting", menuName = "Enemy System/Enemy Spawn Setting")]
public class EnemySpawnSetting : ScriptableObject
{
    [Header("Basic Settings")]
    [Tooltip("Prefab base del enemigo que será instanciado.")]
    public GameObject enemyPrefab;

    [Tooltip("Número máximo de oleadas que se generarán.")]
    public int maxWaves = 5;

    [Tooltip("Tiempo en segundos entre cada oleada.")]
    public float timeBetweenWaves = 10f;

    [Header("Spawn Area")]
    [Tooltip("Radio de la zona en la que los enemigos pueden aparecer.")]
    public float spawnRadius = 5f;

    [Tooltip("Distancia mínima entre enemigos al spawnear.")]
    public float minSpawnSpacing = 1.5f;

    [Header("Wave Configurations")]
    [Tooltip("Configuración de las oleadas, incluyendo tipos de enemigos y cantidad por oleada.")]
    public EnemyWaveConfig[] enemyWaveConfigs;
}

[System.Serializable]
public class EnemyWaveConfig
{
    [Tooltip("Configuración del enemigo (ScriptableObject).")]
    public EnemyConfig enemyConfig;

    [Tooltip("Cantidad de enemigos de este tipo que aparecerán en esta oleada.")]
    public int count;
}
