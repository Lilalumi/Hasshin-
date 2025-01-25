using UnityEngine;

[CreateAssetMenu(fileName = "NewMovementPattern", menuName = "Enemy System/Movement Pattern")]
public abstract class MovementPattern : ScriptableObject
{
    [Header("Movement Settings")]
    [Tooltip("Velocidad de movimiento del enemigo.")]
    public float speed = 2f;

    [Tooltip("Duración total del patrón de movimiento en segundos. Use -1 para infinito.")]
    public float duration = -1f;

    [Tooltip("¿El movimiento es en bucle?")]
    public bool isLooping = true;

    [Tooltip("El tipo de patrón de movimiento que este ScriptableObject describe.")]
    public MovementType movementType;

    [Header("Additional Parameters")]
    [Tooltip("Cualquier parámetro adicional necesario para este patrón de movimiento.")]
    public Vector2[] waypoints; // Por ejemplo, para movimientos basados en puntos

    // Método abstracto que las subclases deben implementar para calcular el movimiento.
    public abstract Vector2 CalculateMovement(Vector2 currentPosition, float timeElapsed, Vector2 directionToCore);
}

public enum MovementType
{
    Linear,
    ZigZag,
    Circular,
    Custom
}
