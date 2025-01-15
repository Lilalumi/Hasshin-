using UnityEngine;

public class Health : MonoBehaviour
{
    public int health = 100; // Salud inicial del objeto
    public float effectDuration = 1f; // Duración del efecto de muerte
    public float sizeAugment = 2f; // Velocidad de aumento de tamaño durante el efecto

    [Header("Visual Effects")]
    public Material dissolveMaterial; // Material que controla el efecto de disolución
    public string dissolveAmountProperty = "_DissolveAmount"; // Nombre de la propiedad del material
    public GameObject destructionEffectPrefab; // Prefab de partículas para el efecto de destrucción
    public AnimationCurve healthToDissolveCurve = AnimationCurve.Linear(0f, 0f, 1f, 1.1f); // Curva de relación entre salud y disolución

    private bool isDying = false; // Bandera para evitar múltiples activaciones del efecto
    private SpriteRenderer spriteRenderer; // Referencia al SpriteRenderer
    private Material instanceMaterial; // Material instanciado para cada enemigo
    private int maxHealth; // Salud máxima inicial

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            // Crear una instancia del material para evitar modificar el original
            instanceMaterial = Instantiate(dissolveMaterial);
            spriteRenderer.material = instanceMaterial;
        }
        else
        {
            Debug.LogError("No se encontró un SpriteRenderer en el objeto.");
        }

        maxHealth = health; // Almacena la salud máxima inicial
        UpdateDissolveEffect(); // Inicializa el efecto visual
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        health = Mathf.Max(health, 0); // Evita valores negativos
        UpdateDissolveEffect();

        if (health <= 0 && !isDying)
        {
            Die();
        }
    }

    private void UpdateDissolveEffect()
    {
        if (instanceMaterial == null || !instanceMaterial.HasProperty(dissolveAmountProperty)) return;

        // Normaliza la salud entre 0 y 1
        float normalizedHealth = (float)health / maxHealth;

        // Calcula el valor usando la curva
        float dissolveAmount = healthToDissolveCurve.Evaluate(1f - normalizedHealth);

        // Aplica el valor de forma progresiva
        LeanTween.value(gameObject, 
                        instanceMaterial.GetFloat(dissolveAmountProperty), 
                        dissolveAmount, 
                        0.3f)
            .setOnUpdate((float value) => 
            {
                instanceMaterial.SetFloat(dissolveAmountProperty, value);
            });
    }

    private void Die()
    {
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

        // Iniciar la animación de muerte
        StartCoroutine(DeathEffect());
    }

    private System.Collections.IEnumerator DeathEffect()
    {
        float elapsedTime = 0f;
        Vector3 initialScale = transform.localScale;

        while (elapsedTime < effectDuration)
        {
            elapsedTime += Time.deltaTime;

            // Aumentar el tamaño durante la muerte
            float scaleMultiplier = 1f + (elapsedTime / effectDuration) * sizeAugment;
            transform.localScale = initialScale * scaleMultiplier;

            yield return null;
        }

        // Asegurarse de que el objeto sea completamente destruido
        Destroy(gameObject);
    }
}
