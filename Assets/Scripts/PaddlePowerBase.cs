using UnityEngine;

// Clase abstracta base para todos los poderes del Paddle
public abstract class PaddlePowerBase : ScriptableObject
{
    // Método abstracto que deben implementar todos los poderes
    public abstract void Activate(GameObject paddle);
}
