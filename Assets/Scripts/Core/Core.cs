using UnityEngine;

public class Core : MonoBehaviour
{
    [Header("Core Data")]
    public CoreData coreData; // Referencia al Scriptable Object CoreData

    [Header("Core Settings")]
    public float maxHealth; // Salud máxima del Core
    public float currentHealth; // Salud actual del Core

    [Header("Dissolve Effect Settings")]
    public Material dissolveMaterial; // Material que controla el efecto de disolución
    public string dissolveAmountProperty = "_DissolveAmount"; // Propiedad del material para disolución
    public string outlineColorProperty = "_OutlineColor"; // Propiedad del material para el color del contorno
    public AnimationCurve healthToDissolveCurve = AnimationCurve.Linear(0f, 0f, 1f, 1.1f); // Curva de disolución

    [ColorUsage(true, true)] 
    public Color outlineColor1 = Color.red; // Primer color HDR
    [ColorUsage(true, true)] 
    public Color outlineColor2 = Color.yellow; // Segundo color HDR
    public float outlineColorFadeSpeed = 1f; // Velocidad del fade in-out del contorno

    private Material instanceMaterial; // Material instanciado para el Core
    private SpriteRenderer spriteRenderer; // Referencia al SpriteRenderer
    private bool isFading = false; // Control para el efecto de fade in-out

    private void Start()
    {
        // Configurar la salud máxima desde el Scriptable Object
        if (coreData != null)
        {
            maxHealth = coreData.coreHealth;
        }
        else
        {
            Debug.LogWarning("CoreData no asignado. Usando valor predeterminado para maxHealth.");
        }

        // Configurar la salud actual al valor máximo
        currentHealth = maxHealth;

        // Inicializar el material de disolución
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && dissolveMaterial != null)
        {
            instanceMaterial = Instantiate(dissolveMaterial);
            spriteRenderer.material = instanceMaterial;
        }

        UpdateDissolveEffect(); // Actualizar el estado inicial del efecto

        // Iniciar el efecto de fade in-out del color del contorno
        StartOutlineColorFade();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verificar si el objeto que colisiona tiene el tag "Enemy"
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Intentar obtener el componente EnemyBehavior del objeto colisionado
            EnemyBehavior enemy = collision.gameObject.GetComponent<EnemyBehavior>();

            if (enemy != null)
            {
                // Reducir la salud actual del Core en base al daño del enemigo
                currentHealth -= enemy.damage;

                // Evitar que la salud actual del Core sea negativa
                currentHealth = Mathf.Max(currentHealth, 0f);

                // Actualizar el efecto de disolución
                UpdateDissolveEffect();

                // Debug.Log para verificar el daño recibido y la salud restante
                Debug.Log($"El Core recibió {enemy.damage} de daño. Salud restante: {currentHealth}/{maxHealth}");

                // Destruir al enemigo después del impacto
                Destroy(collision.gameObject);
            }
            else
            {
                // Debug.Log si el objeto con el tag "Enemy" no tiene EnemyBehavior
                Debug.LogWarning("El objeto con tag 'Enemy' no tiene el componente EnemyBehavior.");
            }
        }
    }

    private void UpdateDissolveEffect()
    {
        if (instanceMaterial == null || !instanceMaterial.HasProperty(dissolveAmountProperty)) return;

        // Normalizar la salud actual del Core (0 a 1)
        float normalizedHealth = currentHealth / maxHealth;

        // Invertir el rango para que 0 de salud corresponda a 1.1 y salud máxima corresponda a 0
        float dissolveAmount = healthToDissolveCurve.Evaluate(1f - normalizedHealth);

        // Aplicar el efecto de disolución mediante LeanTween
        LeanTween.value(gameObject, instanceMaterial.GetFloat(dissolveAmountProperty), dissolveAmount, 0.3f)
            .setOnUpdate((float value) =>
            {
                instanceMaterial.SetFloat(dissolveAmountProperty, value);
            });
    }

    private void StartOutlineColorFade()
    {
        if (instanceMaterial == null || !instanceMaterial.HasProperty(outlineColorProperty)) return;

        if (!isFading)
        {
            isFading = true;

            // Iniciar el efecto de fade in-out alternando entre los dos colores HDR
            FadeToColor(outlineColor1, outlineColor2, outlineColorFadeSpeed);
        }
    }

    private void FadeToColor(Color startColor, Color endColor, float duration)
    {
        LeanTween.value(gameObject, 0f, 1f, duration)
            .setOnUpdate((float t) =>
            {
                Color currentColor = Color.Lerp(startColor, endColor, t);
                instanceMaterial.SetColor(outlineColorProperty, currentColor);
            })
            .setOnComplete(() =>
            {
                // Alternar el ciclo de fade in-out
                FadeToColor(endColor, startColor, duration);
            });
    }
}
