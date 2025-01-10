using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Importante para IEnumerator

public class VictoryController : MonoBehaviour
{
    [Header("Dependencies")]
    public EnemySpawner enemySpawner;
    public GameObject victoryPrefab;
    public string mainMenuSceneName = "MainMenu";

    private bool victoryTriggered = false;

    void Update()
    {
        if (!victoryTriggered)
        {
            CheckVictoryCondition();
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

    private IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 1;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
