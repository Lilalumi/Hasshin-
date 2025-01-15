using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Importante para IEnumerator

public class EnemyBehavior : MonoBehaviour
{
    private Transform core;
    public float speed = 2f;
    public GameObject gameOverPrefab;
    public string mainMenuSceneName = "MainMenu";

    void Start()
    {
        GameObject coreObject = GameObject.FindGameObjectWithTag("Core");

        if (coreObject != null)
        {
            core = coreObject.transform;
        }
    }

    void Update()
    {
        if (core == null) return;

        Vector2 direction = (core.position - transform.position).normalized;
        transform.position += (Vector3)direction * speed * Time.deltaTime;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Core"))
        {
            if (gameOverPrefab != null)
            {
                Instantiate(gameOverPrefab, Vector3.zero, Quaternion.identity);
            }

            Time.timeScale = 0;
            StartCoroutine(ReturnToMainMenu());
        }
    }

    private IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 1;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
