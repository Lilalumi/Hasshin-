using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public EnemyConfig config; // Configuración del enemigo

    private Vector2 targetPosition;
    private float timeElapsed;
    private MovementPattern movementPattern;
    private Transform coreTransform; // Referencia al objeto Core

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
            Debug.LogError("EnemyConfig no está asignado en EnemyBehavior.");
            return;
        }

        // Configurar vida desde el EnemyConfig
        maxHealth = config.health;
        health = maxHealth;

        Debug.Log($"Enemy {name} configurado con {maxHealth} de vida."); // Log para verificar vida inicial

        UpdateDissolveEffect(); // Inicializa el efecto visual

        // Obtener el patrón de movimiento desde la configuración
        movementPattern = config.movementPattern;

        if (movementPattern == null)
        {
            Debug.LogError("MovementPattern no está asignado en el EnemyConfig.");
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
            // Asignar el sprite al BackShadow
            AssignBackShadowSprite(spriteRenderer);
            // Generar PolygonCollider2D basado en el sprite
            AddPolygonCollider(spriteRenderer);
        }

        // Configurar tamaño del enemigo
        transform.localScale = config.size;

        // Configurar collider personalizado, si corresponde
        if (config.useCustomCollider)
        {
            ConfigureCustomCollider();
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
        // Buscar el objeto con el tag "Core"
        GameObject coreObject = GameObject.FindGameObjectWithTag("Core");
        if (coreObject == null)
        {
            Debug.LogError("No se encontró un objeto con el tag 'Core'. Asegúrate de que exista en la escena.");
            return;
        }

        coreTransform = coreObject.transform;

        // Configurar movimiento
        movementPattern = config?.movementPattern;
        targetPosition = transform.position;
        timeElapsed = 0f;
    }

    public void TakeDamage(int damage)
    {
        if (isDying)
        {
            Debug.Log($"Enemy {name} ya está muriendo. No puede recibir más daño.");
            return;
        }

        health -= damage;
        health = Mathf.Max(health, 0f); // Evita valores negativos
        Debug.Log($"Enemy {name} recibió {damage} de daño. Salud restante: {health}/{maxHealth}");

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

    private void Die()
    {
        if (isDying) return;

        Debug.Log($"Enemy {name} ha muerto.");
        isDying = true;

        // Deshabilitar los colliders
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }

        // Instanciar efecto de partículas si existe
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
            Destroy(existingCollider); // Elimina cualquier collider existente
        }

        PolygonCollider2D polygonCollider = gameObject.AddComponent<PolygonCollider2D>();
        polygonCollider.isTrigger = false; // Ajustar según las necesidades del gameplay
    }

    private void ConfigureCustomCollider()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider is BoxCollider2D boxCollider)
        {
            boxCollider.offset = config.colliderOffset;
            boxCollider.size = config.colliderSize;
        }
        else
        {
            Debug.LogWarning("El collider personalizado requiere un BoxCollider2D.");
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
        // Depuración para identificar el objeto que colisiona
        Debug.Log($"Enemy {name} colisionó con {collision.gameObject.name}");

    }
    private void AssignBackShadowSprite(SpriteRenderer parentSpriteRenderer)
    {
        // Buscar el objeto hijo BackShadow
        Transform backShadowTransform = transform.Find("BackShadow");
        if (backShadowTransform == null)
        {
            Debug.LogWarning("No se encontró el objeto hijo 'BackShadow' en el prefab del enemigo.");
            return;
        }

        // Obtener el SpriteRenderer del objeto hijo
        SpriteRenderer backShadowRenderer = backShadowTransform.GetComponent<SpriteRenderer>();
        if (backShadowRenderer == null)
        {
            Debug.LogWarning("El objeto 'BackShadow' no tiene un componente SpriteRenderer.");
            return;
        }

        // Asignar el mismo sprite del padre
        backShadowRenderer.sprite = parentSpriteRenderer.sprite;
    }
}

