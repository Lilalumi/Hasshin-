using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyShieldAbility", menuName = "Enemy System/Abilities/Shield Ability")]
public class EnemyShieldAbility : ScriptableObject
{
    [Header("Configuración del Sprite 1")]
    public Sprite sprite1;
    public Vector2 sprite1Size = Vector2.one;
    public Vector3 sprite1Offset = Vector3.zero;
    public Color secondarySpriteColor1 = Color.white;
    public int sprite1OrderInLayer = 0;

    [Header("Configuración del Sprite 2")]
    public Sprite sprite2;
    public Vector2 sprite2Size = Vector2.one;
    public Vector3 sprite2Offset = Vector3.zero;
    public Color secondarySpriteColor2 = Color.white;
    public int sprite2OrderInLayer = 0;

    [Header("Efectos de Blink")]
    [Tooltip("Frecuencia del blinkeo del Sprite 2 (parpadeos por segundo).")]
    public float blinkFrequency = 2f;

    [Tooltip("Duración del efecto de blinkeo (en segundos).")]
    public float blinkDuration = 1f;

    /// <summary>
    /// Activa el escudo en el enemigo dado.
    /// </summary>
    /// <param name="enemy">El objeto enemigo en el que se activará el escudo.</param>
    public void ActivateShield(GameObject enemy)
    {
        if (enemy == null) return;

        // Asegurarse de que el enemigo tiene el componente EnemyShield
        if (!enemy.TryGetComponent(out EnemyShield enemyShield))
        {
            enemyShield = enemy.AddComponent<EnemyShield>();
        }

        // Configurar el escudo con los valores del Scriptable Object
        enemyShield.ConfigureShield(this);
    }
}
