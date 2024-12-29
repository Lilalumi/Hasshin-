using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    public float speed = 5f; // Velocidad de la pelota, editable desde el inspector
    public int hitsToRedirect = 5; // Golpes consecutivos necesarios para redirigir al núcleo

    private Rigidbody2D rb;
    private Transform core; // Núcleo desde el cual la pelota se alejará
    private int borderHitCount = 0; // Contador de golpes al Border

    void Start()
    {
        // Encuentra el núcleo automáticamente por su etiqueta
        GameObject coreObject = GameObject.FindGameObjectWithTag("Core");
        if (coreObject != null)
        {
            core = coreObject.transform;
        }
        else
        {
            Debug.LogError("No se encontró ningún objeto con la etiqueta 'Core'. Asegúrate de que exista en la escena.");
            return;
        }

        // Obtiene el Rigidbody2D del objeto
        rb = GetComponent<Rigidbody2D>();

        // Calcula la dirección desde el núcleo hacia la pelota
        Vector2 direction = (transform.position - core.position).normalized;

        // Aplica una velocidad inicial en esa dirección
        rb.velocity = direction * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Ajusta la velocidad después de una colisión para mantenerla constante
        rb.velocity = rb.velocity.normalized * speed;

        if (collision.gameObject.CompareTag("Border"))
        {
            // Incrementa el contador si el objeto es un Border
            borderHitCount++;

            if (borderHitCount >= hitsToRedirect)
            {
                RedirectToCore();
                borderHitCount = 0; // Reinicia el contador
            }
        }
        else
        {
            // Reinicia el contador si el objeto no es un Border
            if (borderHitCount > 0)
            {
                borderHitCount = 0;
            }
        }
    }

    private void RedirectToCore()
    {
        if (core == null) return; // Evita errores si no se encontró el núcleo

        // Calcula la dirección hacia el núcleo
        Vector2 directionToCore = (core.position - transform.position).normalized;

        // Aplica la nueva velocidad en esa dirección
        rb.velocity = directionToCore * speed;
    }
}
