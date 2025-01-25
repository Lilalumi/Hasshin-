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
        if (pelletLight != null)
        {
            pelletLight.intensity = intensityDefault;
        }
    }

    void Update()
    {
        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }

        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (pelletLight != null)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                pelletLight.intensity = intensityEnemyCollision;
            }
            else
            {
                pelletLight.intensity = intensityCollision;
            }
        }

        if (impactEffectPrefab != null)
        {
            Instantiate(impactEffectPrefab, collision.contacts[0].point, Quaternion.identity);
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyBehavior enemyBehavior = collision.gameObject.GetComponent<EnemyBehavior>();
            if (enemyBehavior != null)
            {
                enemyBehavior.TakeDamage(damage); // Aplica daño al enemigo
            }
        }

        if (bounceCount >= maxBounce)
        {
            DestroyWithDelay();
        }
        else
        {
            bounceCount++;
        }
    }

    private void DestroyWithDelay()
    {
        Destroy(gameObject, destructionDelay);
    }
}
