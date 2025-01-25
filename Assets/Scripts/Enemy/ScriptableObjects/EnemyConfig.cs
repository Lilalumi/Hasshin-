using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyConfig", menuName = "Enemy System/Enemy Config")]
public class EnemyConfig : ScriptableObject
{
    [Header("Basic Settings")]
    [Tooltip("Nombre del enemigo, utilizado para identificarlo.")]
    public string enemyName; // Nombre del enemigo

    [Tooltip("Sprite que representa visualmente al enemigo.")]
    public Sprite enemySprite; // Sprite del enemigo

    [Tooltip("Vida inicial del enemigo. Determina cuánto daño puede soportar.")]
    public float health = 100f; // Vida inicial del enemigo

    [Tooltip("Tamaño visual del enemigo en la escena (ancho y alto).")]
    public Vector2 size = Vector2.one; // Tamaño del enemigo

    [Tooltip("Daño que el enemigo causa al núcleo al alcanzarlo.")]
    public float damageToCore = 10f; // Daño al núcleo

    [Tooltip("Patrón de movimiento del enemigo, definido como un ScriptableObject.")]
    public MovementPattern movementPattern; // Patrón de movimiento del enemigo

    [Header("Collider Settings")]
    [Tooltip("¿Usar un collider personalizado? Si no, se usará uno predeterminado.")]
    public bool useCustomCollider = false; // Si el enemigo tiene un collider personalizado

    [Tooltip("Offset del collider personalizado (solo si está habilitado).")]
    public Vector2 colliderOffset = Vector2.zero; // Offset del collider

    [Tooltip("Tamaño del collider personalizado (solo si está habilitado).")]
    public Vector2 colliderSize = Vector2.one; // Tamaño del collider

    [Header("Abilities")]
    [Tooltip("¿El enemigo tiene habilidades especiales?")]
    public bool hasAbilities = false; // Si el enemigo tiene habilidades especiales

    [Tooltip("Lista de habilidades del enemigo (ScriptableObjects de poderes).")]
    public ScriptableObject[] abilities; // Lista de poderes del enemigo

    [Header("Visual Effects")]
    [Tooltip("Prefab para el efecto visual al morir.")]
    public GameObject deathEffectPrefab; // Prefab para el efecto al morir

    [Tooltip("Prefab para el efecto visual al recibir daño.")]
    public GameObject hitEffectPrefab; // Prefab para el efecto al recibir daño

    [Header("Audio Settings")]
    [Tooltip("Sonido que se reproduce cuando el enemigo aparece.")]
    public AudioClip spawnSound; // Sonido al aparecer

    [Tooltip("Sonido que se reproduce cuando el enemigo muere.")]
    public AudioClip deathSound; // Sonido al morir

    [Tooltip("Sonido que se reproduce cuando el enemigo recibe daño.")]
    public AudioClip hitSound; // Sonido al recibir daño
}
