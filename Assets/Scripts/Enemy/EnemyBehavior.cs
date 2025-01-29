using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public EnemyConfig config; // Configuración del enemigo

    private Vector2 targetPosition;
    private float timeElapsed;
    private MovementPattern movementPattern;
    private Transform coreTransform; // Referencia al objeto Core

    [Header("Basic Settings")]
    public float damage; // Daño del enemigo (referenciable por otros scripts)

    [Header("Health Settings")]
    private float maxHealth; // Salud máxima inicial
    private float health; // Salud actual
    public Material dissolveMaterial; // Material que controla el efecto de disolución
    public string dissolveAmountProperty = "_DissolveAmount"; // Propiedad del material
    public AnimationCurve healthToDissolveCurve = AnimationCurve.Linear(0f, 0f, 1f, 1.1f);
    public GameObject destructionEffectPrefab; // Prefab de efecto de muerte
    public float effectDuration = 1f; // Duración del efecto de muerte
    public float sizeAugment = 2f; // Velocidad de aumento de tamaño durante el efecto

    private bool isDying = false; // Bandera para evitar múltiples activaciones del efecto
    private SpriteRenderer spriteRenderer; // Referencia al SpriteRenderer
    private Material instanceMaterial; // Material instanciado para cada enemigo

    [Header("Data Shards Settings")]
    public int dataShardsOnDeath = 5; // Cantidad de Data Shards al morir
    public GameObject dataShardPrefab; // Prefab del objeto Data Shard
    public float floatRadius = 1f; // Radio de distribución aleatoria
    private bool isQuitting = false; // Para evitar generar Data Shards al cerrar la aplicación

    void Start()
    {
        InitializeFromConfig();
        InitializeHealth();
        InitializeMovement();
    }

    void Update()
    {
        UpdateMovement();
    }

    public void SetConfig(EnemyConfig newConfig)
    {
        config = newConfig;
        InitializeFromConfig(); // Llama a la inicialización para configurar el enemigo con el nuevo `EnemyConfig`
    }

    // Método para inicializar valores a partir de la configuración
    private void InitializeFromConfig()
    {
        if (config == null)
        {
            return;
        }

        // Configurar vida desde el EnemyConfig
        maxHealth = config.health;
        health = maxHealth;

        // Configurar daño desde el EnemyConfig
        damage = config.damageToCore;

        UpdateDissolveEffect(); // Inicializa el efecto visual

        // Obtener el patrón de movimiento desde la configuración
        movementPattern = config.movementPattern;

        if (movementPattern == null)
        {
            return;
        }

        // Inicializar valores específicos del enemigo
        targetPosition = transform.position;
        timeElapsed = 0f;

        // Configurar visuales del enemigo
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && config.enemySprite != null)
        {
            instanceMaterial = Instantiate(dissolveMaterial);
            spriteRenderer.sprite = config.enemySprite;
            UpdateDissolveEffect(); // Inicializa el efecto de salud visual
            AssignBackShadowSprite(spriteRenderer);
            AddPolygonCollider(spriteRenderer);
        }

        // Configurar tamaño del enemigo
        transform.localScale = config.size;

        // Configurar collider personalizado, si corresponde
        if (config.useCustomCollider)
        {
            ConfigureCustomCollider();
        }
        // Configurar habilidades especiales
        if (config.hasAbilities && config.abilities != null)
        {
            foreach (ScriptableObject ability in config.abilities)
            {
                if (ability is EnemyShieldAbility shieldAbility)
                {
                    shieldAbility.ActivateShield(gameObject);
                }
            }
        }
    }

    private void InitializeHealth()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && dissolveMaterial != null)
        {
            instanceMaterial = Instantiate(dissolveMaterial);
            spriteRenderer.material = instanceMaterial;
        }

        UpdateDissolveEffect();
    }

    private void InitializeMovement()
    {
        GameObject coreObject = GameObject.FindGameObjectWithTag("Core");
        if (coreObject == null)
        {
            return;
        }

        coreTransform = coreObject.transform;

        movementPattern = config?.movementPattern;
        targetPosition = transform.position;
        timeElapsed = 0f;
    }

    public void TakeDamage(int damage)
    {
        if (isDying)
        {
            return;
        }

        health -= damage;
        health = Mathf.Max(health, 0f);

        UpdateDissolveEffect();

        if (health <= 0)
        {
            Die();
        }
    }

    private void UpdateMovement()
    {
        if (movementPattern != null && coreTransform != null)
        {
            timeElapsed += Time.deltaTime;
            Vector2 directionToCore = (coreTransform.position - transform.position).normalized;
            Vector2 newPosition = movementPattern.CalculateMovement(transform.position, timeElapsed, directionToCore);
            transform.position = Vector2.Lerp(transform.position, newPosition, Time.deltaTime * movementPattern.speed);
        }
    }

    private void UpdateDissolveEffect()
    {
        if (instanceMaterial == null || !instanceMaterial.HasProperty(dissolveAmountProperty)) return;

        float normalizedHealth = health / maxHealth;
        float dissolveAmount = healthToDissolveCurve.Evaluate(1f - normalizedHealth);

        LeanTween.value(gameObject, instanceMaterial.GetFloat(dissolveAmountProperty), dissolveAmount, 0.3f)
            .setOnUpdate((float value) =>
            {
                instanceMaterial.SetFloat(dissolveAmountProperty, value);
            });
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private void Die()
    {
        if (isDying || isQuitting) return; // Evitar efectos si se está cerrando la aplicación

        isDying = true;

        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }

        if (destructionEffectPrefab != null)
        {
            Instantiate(destructionEffectPrefab, transform.position, Quaternion.identity);
        }

        StartCoroutine(DeathEffect());
    }

    private void AddPolygonCollider(SpriteRenderer spriteRenderer)
    {
        PolygonCollider2D existingCollider = GetComponent<PolygonCollider2D>();
        if (existingCollider != null)
        {
            Destroy(existingCollider);
        }

        PolygonCollider2D polygonCollider = gameObject.AddComponent<PolygonCollider2D>();
        polygonCollider.isTrigger = false;
    }

    private void ConfigureCustomCollider()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider is BoxCollider2D boxCollider)
        {
            boxCollider.offset = config.colliderOffset;
            boxCollider.size = config.colliderSize;
        }
    }

    private System.Collections.IEnumerator DeathEffect()
    {
        float elapsedTime = 0f;
        Vector3 initialScale = transform.localScale;

        while (elapsedTime < effectDuration)
        {
            elapsedTime += Time.deltaTime;
            float scaleMultiplier = 1f + (elapsedTime / effectDuration) * sizeAugment;
            transform.localScale = initialScale * scaleMultiplier;

            yield return null;
        }

        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Método de colisión sin logs
    }

    private void AssignBackShadowSprite(SpriteRenderer parentSpriteRenderer)
    {
        Transform backShadowTransform = transform.Find("BackShadow");
        if (backShadowTransform == null)
        {
            return;
        }

        SpriteRenderer backShadowRenderer = backShadowTransform.GetComponent<SpriteRenderer>();
        if (backShadowRenderer == null)
        {
            return;
        }

        backShadowRenderer.sprite = parentSpriteRenderer.sprite;
    }
}
