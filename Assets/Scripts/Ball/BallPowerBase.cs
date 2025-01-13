using UnityEngine;

public abstract class BallPowerBase : ScriptableObject
{
    public string powerName; // Nombre descriptivo del poder
    public Sprite powerIcon; // Ícono del poder (PNG u otro formato)

    public abstract void Activate(GameObject ballController);
}
