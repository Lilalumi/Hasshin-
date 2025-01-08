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

    [Header("Light Settings")]
    public Light2D asteroidLight; // Componente Light 2D
    public float glowIntensity = 1f; // Intensidad de luz por defecto
    public float flashIntensity = 5f; // Intensidad de luz al colisionar
    public float flashDuration = 0.2f; // Duración del flash

    void Start()
    {
        // Configura la luz inicial
        if (asteroidLight != null)
        {
            asteroidLight.intensity = glowIntensity;
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
        if (collision.CompareTag("Enemy") && canDealDamage)
        {
            // Aplica daño al enemigo
            Health enemyHealth = collision.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            // Activa el flash de la luz
            if (asteroidLight != null)
            {
                StartCoroutine(FlashLight());
            }

            // Desactiva temporalmente la capacidad de hacer daño
            canDealDamage = false;
            Invoke(nameof(ResetDamageCooldown), flashDuration); // Permite daño después del flash
        }
    }

    private void ResetDamageCooldown()
    {
        canDealDamage = true;
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
}
