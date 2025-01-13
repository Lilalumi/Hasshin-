using UnityEngine;
using UnityEngine.Rendering.Universal; // Para Light 2D

public class Pellet : MonoBehaviour
{
    private Vector3 startPosition;
    private float maxDistance;
    private int bounceCount = 0;

    [Header("Pellet Settings")]
    public int damage = 10; // Daño que hará el pellet al impactar
    public int maxBounce = 0; // Cantidad máxima de rebotes antes de desaparecer
    public float rotationSpeed = 360f; // Velocidad de rotación mientras se mueve
    public float destructionDelay = 0.1f; // Tiempo antes de destruir el pellet tras colisionar

    [Header("Light Settings")]
    public Light2D pelletLight; // Componente Light 2D
    public float intensityDefault = 1f; // Intensidad por defecto de la luz
    public float intensityCollision = 3f; // Intensidad de la luz al colisionar con algo
    public float intensityEnemyCollision = 5f; // Intensidad de la luz al colisionar con un enemigo
    public GameObject impactEffectPrefab; // Prefab del efecto de impacto
    public void SetMaxDistance(float distance)
    {
        maxDistance = distance;
        startPosition = transform.position;
    }

    void Start()
    {
        // Asegura que la luz tenga la intensidad por defecto al inicio
        if (pelletLight != null)
        {
            pelletLight.intensity = intensityDefault;
        }
    }

    void Update()
    {
        // Verifica si el pellet ha alcanzado la distancia máxima
        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }

        // Gira el pellet mientras se mueve
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (pelletLight != null)
        {
            // Ajusta la intensidad de la luz según el tipo de colisión
            if (collision.gameObject.CompareTag("Enemy"))
            {
                pelletLight.intensity = intensityEnemyCollision;
            }
            else
            {
                pelletLight.intensity = intensityCollision;
            }
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Aplica daño al enemigo
            Health enemyHealth = collision.gameObject.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            // Verifica si el pellet tiene rebotes restantes
            if (bounceCount >= maxBounce)
            {
                DestroyWithDelay();
            }
            else
            {
                bounceCount++;
            }
            return;
            if (impactEffectPrefab != null)
            {
                Instantiate(impactEffectPrefab, collision.contacts[0].point, Quaternion.identity);
            }
        }

        // Verifica si el pellet puede rebotar tras colisionar con otro objeto
        if (bounceCount < maxBounce)
        {
            bounceCount++;
        }
        else
        {
            // Destruye el pellet si alcanza el número máximo de rebotes
            DestroyWithDelay();
        }
    }

    private void DestroyWithDelay()
    {
        // Destruye el pellet con un retraso para mostrar el flash
        Destroy(gameObject, destructionDelay);
    }
}
