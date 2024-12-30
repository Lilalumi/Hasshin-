using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    private Transform core; // El núcleo hacia el cual el enemigo se moverá
    public float speed = 2f; // Velocidad del enemigo, editable desde el inspector
    public GameObject gameOverPrefab; // Prefab del Canvas de Game Over

    void Start()
    {
        // Busca el objeto con el tag "Core" en la escena
        GameObject coreObject = GameObject.FindGameObjectWithTag("Core");

        if (coreObject != null)
        {
            core = coreObject.transform;
        }
        else
        {
            Debug.LogError("No se encontró un objeto con el tag 'Core' en la escena.");
        }
    }

    void Update()
    {
        if (core == null) return; // Asegúrate de que el núcleo esté asignado

        // Dirección hacia el núcleo
        Vector2 direction = (core.position - transform.position).normalized;

        // Moverse hacia el núcleo
        transform.position += (Vector3)direction * speed * Time.deltaTime;

        // Ajustar la rotación para apuntar hacia el núcleo
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90); // Ajuste para sprites orientados hacia arriba
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica si colisiona con el Core
        if (collision.gameObject.CompareTag("Core"))
        {
            // Instanciar el prefab del Canvas de Game Over
            if (gameOverPrefab != null)
            {
                Instantiate(gameOverPrefab, Vector3.zero, Quaternion.identity);
            }
            else
            {
                Debug.LogError("No se asignó el prefab de Game Over en el inspector.");
            }

            Debug.Log("GAME OVER!"); // Imprime el mensaje en la consola
            Time.timeScale = 0; // Detiene el tiempo en el juego
        }
    }
}
