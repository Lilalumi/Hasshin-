using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    public float initialSpeed = 5f; // Velocidad inicial de la pelota
    public float maxSpeed = 15f; // Velocidad máxima de la pelota
    public float speedAugment = 1f; // Incremento de velocidad al golpear la paleta
    public float accelerationRate = 0.1f; // Tasa de aceleración progresiva
    public int hitsToRedirect = 5; // Golpes consecutivos necesarios para redirigir al núcleo
    public int damage = 10; // Daño que la pelota inflige
    public bool destroyOnCoreCollision = false; // Toggle para habilitar/deshabilitar el comportamiento de desinstanciar

    private float currentSpeed; // Velocidad actual de la pelota
    private float targetSpeed; // Velocidad hacia la que se acelera progresivamente
    private Rigidbody2D rb;
    private Transform core; // Núcleo desde el cual la pelota se alejará
    private int borderHitCount = 0; // Contador de golpes al Border

    public GameObject impactEffectEnemyPrefab; // Prefab del efecto para impacto con enemigos
    public GameObject impactEffectPaddlePrefab; // Prefab del efecto para impacto con el paddle

    void Start()
    {
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

        rb = GetComponent<Rigidbody2D>();
        currentSpeed = initialSpeed;
        targetSpeed = initialSpeed;

        Vector2 direction = (transform.position - core.position).normalized;
        direction += new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f)).normalized * 0.1f;
        direction.Normalize();

        rb.velocity = direction * currentSpeed;
    }

    void FixedUpdate()
    {
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, accelerationRate);
        rb.velocity = rb.velocity.normalized * currentSpeed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector3 contactPoint = collision.contacts[0].point;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            SpawnImpactEffect(impactEffectEnemyPrefab, contactPoint);
        }
        else if (collision.gameObject.CompareTag("Paddle"))
        {
            targetSpeed += speedAugment;
            if (targetSpeed > maxSpeed) targetSpeed = maxSpeed;

            SpawnImpactEffect(impactEffectPaddlePrefab, contactPoint);
        }
        else if (collision.gameObject.CompareTag("Core"))
        {
            if (destroyOnCoreCollision)
            {
                Destroy(gameObject);
                return;
            }

            targetSpeed = initialSpeed;
        }
        else if (collision.gameObject.CompareTag("Border"))
        {
            SpawnImpactEffect(impactEffectEnemyPrefab, contactPoint);
            borderHitCount++;
            if (borderHitCount >= hitsToRedirect)
            {
                RedirectToCore();
                borderHitCount = 0;
            }
        }
        else
        {
            borderHitCount = 0;
        }

        Health targetHealth = collision.gameObject.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
        }
    }

    private void SpawnImpactEffect(GameObject prefab, Vector3 position)
    {
        if (prefab != null)
        {
            Instantiate(prefab, position, Quaternion.identity);
        }
    }

    private void RedirectToCore()
    {
        if (core == null) return;

        Vector2 directionToCore = (core.position - transform.position).normalized;
        rb.velocity = directionToCore * currentSpeed;
    }
}
