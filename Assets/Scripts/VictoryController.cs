using UnityEngine;

public class VictoryController : MonoBehaviour
{
    [Header("Dependencies")]
    public EnemySpawner enemySpawner; // Referencia al script EnemySpawner
    public GameObject victoryPrefab; // Prefab del Canvas de Victoria

    private bool victoryTriggered = false; // Bandera para evitar que la victoria se procese múltiples veces

    void Update()
    {
        if (!victoryTriggered)
        {
            CheckVictoryCondition();
        }
    }

    private void CheckVictoryCondition()
    {
        // Verifica si quedan enemigos activos
        GameObject[] activeEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (activeEnemies.Length > 0)
        {
            return; // Todavía hay enemigos activos
        }

        // Verifica si quedan oleadas por instanciarse
        if (enemySpawner != null && enemySpawner.HasPendingWaves())
        {
            return; // Todavía hay oleadas pendientes
        }

        // Si no hay enemigos activos ni pendientes, muestra la pantalla de victoria
        TriggerVictory();
    }

    private void TriggerVictory()
    {
        victoryTriggered = true; // Marca la victoria como procesada
        Debug.Log("¡Victoria! No quedan más enemigos.");

        // Instanciar el prefab de Victoria
        if (victoryPrefab != null)
        {
            Instantiate(victoryPrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogError("No se asignó el prefab de Victoria en el inspector.");
        }

        // Detener el tiempo del juego
        Time.timeScale = 0;
    }
}
