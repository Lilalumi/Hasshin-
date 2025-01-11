using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    public float initialSpeed = 5f; // Velocidad inicial de la pelota, editable desde el inspector
    public float maxSpeed = 15f; // Velocidad máxima de la pelota
    public float speedAugment = 1f; // Incremento de velocidad al golpear la paleta
    public float accelerationRate = 0.1f; // Tasa de aceleración progresiva
    public int hitsToRedirect = 5; // Golpes consecutivos necesarios para redirigir al núcleo
    public int damage = 10; // Daño que la pelota inflige, editable desde el inspector
    public bool destroyOnCoreCollision = false; // Toggle para habilitar/deshabilitar el comportamiento de desinstanciar

    private float currentSpeed; // Velocidad actual de la pelota
    private float targetSpeed; // Velocidad hacia la que se acelera progresivamente
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

        // Inicializa la velocidad actual y el objetivo de velocidad
        currentSpeed = initialSpeed;
        targetSpeed = initialSpeed;

        // Calcula la dirección inicial asegurando que no sea tangencial
        Vector2 direction = (transform.position - core.position).normalized;

        // Agrega un pequeño offset aleatorio para evitar trayectorias predecibles
        direction += new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f)).normalized * 0.1f;
        direction.Normalize();

        // Aplica la velocidad inicial en esa dirección
        rb.velocity = direction * currentSpeed;
    }

    void FixedUpdate()
    {
        // Ajusta la velocidad progresivamente hacia la velocidad objetivo
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, accelerationRate);
        rb.velocity = rb.velocity.normalized * currentSpeed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
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
        else if (collision.gameObject.CompareTag("Paddle"))
        {
            // Ajusta inmediatamente el objetivo de velocidad al golpear la paleta
            targetSpeed += speedAugment;
            if (targetSpeed > maxSpeed)
            {
                targetSpeed = maxSpeed; // Limita la velocidad al máximo permitido
            }
        }
        else if (collision.gameObject.CompareTag("Core"))
        {
            // Verifica si debe desinstanciar la pelota
            if (destroyOnCoreCollision)
            {
                Destroy(gameObject);
                return; // Salir del método para evitar acciones posteriores
            }

            // Reinicia la velocidad al golpear el núcleo
            targetSpeed = initialSpeed;
        }
        else
        {
            // Reinicia el contador si el objeto no es un Border
            if (borderHitCount > 0)
            {
                borderHitCount = 0;
            }
        }

        // Aplica daño si el objeto colisionado tiene un script de salud
        Health targetHealth = collision.gameObject.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
        }
    }

    private void RedirectToCore()
    {
        if (core == null) return; // Evita errores si no se encontró el núcleo

        // Calcula la dirección hacia el núcleo
        Vector2 directionToCore = (core.position - transform.position).normalized;

        // Aplica la nueva velocidad en esa dirección
        rb.velocity = directionToCore * currentSpeed;
    }
}
