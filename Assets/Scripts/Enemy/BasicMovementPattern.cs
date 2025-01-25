using UnityEngine;

[CreateAssetMenu(fileName = "LinearMovementPattern", menuName = "Enemy System/Movement Pattern/Linear")]
public class LinearMovementPattern : MovementPattern
{
    public override Vector2 CalculateMovement(Vector2 currentPosition, float timeElapsed, Vector2 directionToCore)
    {
        // Movimiento lineal hacia el Core
        return currentPosition + directionToCore * speed * Time.deltaTime;
    }
}
