using UnityEngine;
using UnityEngine.Rendering.Universal; // Para Light 2D

public class Asteroid : MonoBehaviour
{
    private Transform orbitCenter;
    private float orbitSpeed;
    private float orbitRadius;
    private float angle; // Ángulo actual de la órbita

    [Header("Damage Settings")]
    public int damage = 10; // Daño que hará al impactar con un enemigo
    private bool canDealDamage = true; // Controla si puede hacer daño nuevamente
    private bool isContactingEnemy = false; // Controla si ya está en contacto con un enemigo

    [Header("Light Settings")]
    public Light2D asteroidLight; // Componente Light 2D
    public float glowIntensity = 1f; // Intensidad de luz por defecto
    public float flashIntensity = 5f; // Intensidad de luz al colisionar
    public float flashDuration = 0.2f; // Duración del flash

    [Header("Particle Settings")]
    public ParticleSystem impactParticles; // Prefab de partículas para el impacto

    void Start()
    {
        // Configura la luz inicial
        if (asteroidLight != null)
        {
            asteroidLight.intensity = glowIntensity;
        }

        // Asegúrate de que las partículas estén desactivadas al inicio
        if (impactParticles != null && impactParticles.isPlaying)
        {
            impactParticles.Stop();
        }
    }

    void Update()
    {
        // Actualiza la posición para mantener la órbita
        if (orbitCenter != null)
        {
            angle += orbitSpeed * Time.deltaTime;
            float x = orbitCenter.position.x + Mathf.Cos(angle) * orbitRadius;
            float y = orbitCenter.position.y + Mathf.Sin(angle) * orbitRadius;
            transform.position = new Vector3(x, y, transform.position.z);
        }
    }

    public void SetOrbit(Transform center, float radius, float speed, float initialAngle)
    {
        orbitCenter = center;
        orbitRadius = radius;
        orbitSpeed = speed;
        angle = initialAngle; // Establece el ángulo inicial único
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !isContactingEnemy && canDealDamage)
        {
            // Marca que está en contacto con un enemigo
            isContactingEnemy = true;

            // Aplica daño al enemigo
            EnemyBehavior enemyBehavior = collision.gameObject.GetComponent<EnemyBehavior>();
            if (enemyBehavior != null)
            {
                enemyBehavior.TakeDamage(damage); // Aplica daño al enemigo
            }

            // Desactiva la capacidad de hacer daño nuevamente
            canDealDamage = false;

            // Activa el efecto de partículas
            PlayImpactParticles();

            // Activa el flash de la luz
            if (asteroidLight != null)
            {
                StartCoroutine(FlashLight());
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // Resetea el estado al salir del contacto
            isContactingEnemy = false;

            // Reinicia la capacidad de hacer daño nuevamente después de un tiempo
            Invoke(nameof(ResetDamage), 1f);
        }
    }

    private void PlayImpactParticles()
    {
        if (impactParticles != null)
        {
            // Reproduce las partículas en la posición actual del asteroide
            impactParticles.transform.position = transform.position;
            impactParticles.Play();
        }
    }

    private System.Collections.IEnumerator FlashLight()
    {
        if (asteroidLight != null)
        {
            asteroidLight.intensity = flashIntensity;
            yield return new WaitForSeconds(flashDuration);
            asteroidLight.intensity = glowIntensity;
        }
    }

    private void ResetDamage()
    {
        canDealDamage = true;
    }
}
