using UnityEngine;

// Clase abstracta base para todos los poderes del Paddle
public abstract class PaddlePowerBase : ScriptableObject
{
    public string powerName; // Nombre descriptivo del poder
    public Sprite powerIcon; // Ícono del poder (PNG u otro formato)
    // Método abstracto que deben implementar todos los poderes
    public abstract void Activate(GameObject paddle);
}
