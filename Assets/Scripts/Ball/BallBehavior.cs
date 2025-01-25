using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    public float initialSpeed = 5f;
    public float maxSpeed = 15f;
    public float speedAugment = 1f;
    public float accelerationRate = 0.1f;
    public int hitsToRedirect = 5;
    public int damage = 10;
    public bool destroyOnCoreCollision = false;

    public float currentSpeed { get; set; }
    private float targetSpeed;
    private Rigidbody2D rb;
    private Transform core;
    private int borderHitCount = 0;

    [Header("Impact Effects")]
    public GameObject impactEffectEnemyPrefab;
    public GameObject impactEffectPaddlePrefab;

    [Header("Audio Clips")]
    public AudioClip bleep02;
    public AudioClip bleep03;
    public AudioClip bleep04;
    public AudioClip click04;

    void Start()
    {
        GameObject coreObject = GameObject.FindGameObjectWithTag("Core");
        if (coreObject != null)
        {
            core = coreObject.transform;
        }
        else
        {
            Debug.LogError("No se encontró ningún objeto con la etiqueta 'Core'.");
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
        if (PauseManager.IsPaused) return; // Detener el movimiento cuando el juego está pausado

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, accelerationRate);
        rb.velocity = rb.velocity.normalized * currentSpeed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector3 contactPoint = collision.contacts[0].point;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            PlaySound(click04);
            SpawnImpactEffect(impactEffectEnemyPrefab, contactPoint);

            // Buscar el EnemyBehavior en lugar de Health
            EnemyBehavior enemyBehavior = collision.gameObject.GetComponent<EnemyBehavior>();
            if (enemyBehavior != null)
            {
                enemyBehavior.TakeDamage(damage); // Aplica daño al enemigo
            }
        }
        else if (collision.gameObject.CompareTag("Paddle"))
        {
            targetSpeed += speedAugment;
            if (targetSpeed > maxSpeed) targetSpeed = maxSpeed;

            PlaySound(bleep03);
            SpawnImpactEffect(impactEffectPaddlePrefab, contactPoint);
        }
        else if (collision.gameObject.CompareTag("Core"))
        {
            PlaySound(bleep04);

            if (destroyOnCoreCollision)
            {
                Destroy(gameObject);
                return;
            }

            targetSpeed = initialSpeed;
        }
        else if (collision.gameObject.CompareTag("Border"))
        {
            PlaySound(bleep02);
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
    }

    private void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
        {
            SoundManager.Instance.PlaySFX(clip, volume);
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

    public void ReduceSpeed(float reductionRate)
    {
        targetSpeed = Mathf.Max(0, targetSpeed - reductionRate * Time.deltaTime);
    }
}
