using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;


public class Missile : MonoBehaviour
{
    [Header("Missile Settings")]
    public float speed = 5f; // Velocidad del misil
    [Range(0f, 1f)] public float turnSpeed = 0.5f; // Velocidad de giro (0: sin curva, 1: rastreo instantáneo)
    public float initialFlightTime = 1f; // Tiempo en el que el misil vuela recto antes de rastrear
    public float spinSpeed = 360f; // Velocidad de rotación en la fase inicial
    public int damage = 10; // Daño que hará el misil al enemigo
    public float lifetime = 5f; // Tiempo de vida del misil antes de desaparecer
    public bool useAreaCollider = true; // Toggle para usar el AreaCollider
    private float initialFlightTimer = 0f;

   [Header("Targeting Settings")]
    public bool trackClosestTarget = true; // Variable que controla si se rastrea el objetivo más cercano

    [Header("Light Settings")]
    public Light2D missileLight; // Componente Light 2D
    public float lightIntensity = 1f; // Intensidad de la luz

    private Transform target; // Referencia al enemigo actual
    private bool isTracking = false; // Controla si el misil está rastreando
    private bool hasTarget = false; // Indica si el misil encontró un objetivo
    private Vector2 currentDirection; // Dirección actual del misil
    private List<Transform> detectedEnemies = new List<Transform>(); // Lista de enemigos detectados
    private TrailRenderer trailRenderer; // Referencia al Trail Renderer

    void Start()
    {
        // Configura la luz del misil
        if (missileLight != null)
        {
            missileLight.intensity = 0; // Luz apagada al inicio
        }

        // Obtén el Trail Renderer y desactívalo al inicio
        trailRenderer = GetComponent<TrailRenderer>();
        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;
        }

        // Inicia la fase inicial (rotación en su lugar)
        Invoke(nameof(StartTracking), initialFlightTime);

        // Destruye el misil después del tiempo de vida
        Destroy(gameObject, lifetime);

        // Establece una dirección inicial aleatoria
        float randomAngle = Random.Range(0f, 360f);
        currentDirection = Quaternion.Euler(0, 0, randomAngle) * Vector2.up;

        // Inicia el efecto de luz parpadeante
        if (missileLight != null)
        {
            InvokeRepeating(nameof(FlickerLight), 0, 0.1f);
        }
    }

    void Update()
    {
        if (isTracking)
        {
            if (hasTarget)
            {
                // Si hay un objetivo, rastrearlo
                TrackTarget();
                if (missileLight != null) missileLight.intensity = lightIntensity;
            }
            else
            {
                // Si no hay objetivo, continuar moviéndose aleatoriamente
                MoveInCurrentDirection();
                if (missileLight != null) missileLight.intensity = 0;
            }
        }
        else
        {
            // Rotación en su lugar durante la fase inicial
            Spin();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Aplica daño al enemigo
            Health enemyHealth = collision.gameObject.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            // Destruye el misil
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (useAreaCollider && other.CompareTag("Enemy") && !detectedEnemies.Contains(other.transform))
        {
            detectedEnemies.Add(other.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (useAreaCollider && other.CompareTag("Enemy") && detectedEnemies.Contains(other.transform))
        {
            detectedEnemies.Remove(other.transform);
        }
    }

    private void StartTracking()
    {
        if (useAreaCollider)
        {
            if (detectedEnemies.Count > 0)
            {
                FindTargetFromDetected();
            }
        }
        else
        {
            FindTargetFromAll();
        }

        isTracking = true;

        // Activa el Trail Renderer
        if (trailRenderer != null)
        {
            trailRenderer.enabled = true;
        }

        if (hasTarget)
        {
            Debug.Log($"Missile at {transform.position} started tracking target at {target.position}");
        }
        else
        {
            Debug.Log("Missile has no valid target and will move randomly.");
        }
    }

    private void TrackTarget()
    {
        if (target == null) return;

        // Dirección hacia el objetivo
        Vector2 directionToTarget = ((Vector2)target.position - (Vector2)transform.position).normalized;

        // Calcula el ángulo hacia el objetivo
        float angleToTarget = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;

        // Rotación actual del misil
        float currentAngle = transform.eulerAngles.z;

        // Calcula el nuevo ángulo basado en Turn Speed
        float newAngle = Mathf.LerpAngle(currentAngle, angleToTarget, turnSpeed);

        // Aplica la nueva rotación
        transform.rotation = Quaternion.Euler(0, 0, newAngle);

        // Actualiza la dirección actual del misil
        currentDirection = transform.up.normalized;

        // Mueve el misil hacia adelante
        transform.position += (Vector3)currentDirection * speed * Time.deltaTime;
    }

    private void MoveInCurrentDirection()
    {
        // Movimiento en la dirección actual
        transform.position += (Vector3)currentDirection * speed * Time.deltaTime;
    }

    private void Spin()
    {
        // Rota el misil en su lugar
        transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
    }

    private void FindTargetFromDetected()
    {
        if (detectedEnemies.Count == 0) return;

        if (trackClosestTarget)
        {
            float closestDistance = Mathf.Infinity;
            foreach (Transform enemy in detectedEnemies)
            {
                float distance = Vector2.Distance(transform.position, enemy.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    target = enemy;
                }
            }
        }
        else
        {
            int randomIndex = Random.Range(0, detectedEnemies.Count);
            target = detectedEnemies[randomIndex];
        }

        hasTarget = target != null;
    }

    private void FindTargetFromAll()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0) return;

        if (trackClosestTarget)
        {
            float closestDistance = Mathf.Infinity;
            foreach (GameObject enemy in enemies)
            {
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    target = enemy.transform;
                }
            }
        }
        else
        {
            int randomIndex = Random.Range(0, enemies.Length);
            target = enemies[randomIndex].transform;
        }

        hasTarget = target != null;
    }

    private void FlickerLight()
    {
        if (missileLight != null && !hasTarget)
        {
            missileLight.intensity = Random.Range(0.2f, 0.8f);
        }
    }

        public void SetInitialFlightTime(float time)
    {
        initialFlightTimer = time;
        Invoke(nameof(StartTracking), initialFlightTimer);
    }
}
