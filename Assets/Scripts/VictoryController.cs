using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Importante para IEnumerator

public class VictoryController : MonoBehaviour
{
    [Header("Dependencies")]
    public EnemySpawner enemySpawner;
    public GameObject victoryPrefab;
    public GameObject gameOverPrefab; // Prefab de Game Over
    public Core core; // Referencia al Core
    public string mainMenuSceneName = "MainMenu";

    private bool victoryTriggered = false;
    private bool gameOverTriggered = false;

    void Update()
    {
        if (!victoryTriggered && !gameOverTriggered)
        {
            CheckVictoryCondition();
            CheckGameOverCondition();
        }
    }

    private void CheckVictoryCondition()
    {
        GameObject[] activeEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (activeEnemies.Length > 0)
        {
            return;
        }

        if (enemySpawner != null && enemySpawner.HasPendingWaves())
        {
            return;
        }

        TriggerVictory();
    }

    private void CheckGameOverCondition()
    {
        if (core != null && core.currentHealth <= 0f)
        {
            TriggerGameOver();
        }
    }

    private void TriggerVictory()
    {
        victoryTriggered = true;
        Debug.Log("¡Victoria! No quedan más enemigos.");

        if (victoryPrefab != null)
        {
            Instantiate(victoryPrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogError("No se asignó el prefab de Victoria en el inspector.");
        }

        Time.timeScale = 0;

        StartCoroutine(ReturnToMainMenu());
    }

    private void TriggerGameOver()
    {
        gameOverTriggered = true;
        Debug.Log("¡Game Over! El Core ha sido destruido.");

        if (gameOverPrefab != null)
        {
            Instantiate(gameOverPrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogError("No se asignó el prefab de Game Over en el inspector.");
        }

        Time.timeScale = 0;

        StartCoroutine(ReturnToMainMenu());
    }

    private IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 1;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
