using UnityEngine;

[CreateAssetMenu(fileName = "NewCoreData", menuName = "Core System/Core Data")]
public class CoreData : ScriptableObject
{
    [Header("Core Settings")]
    [Tooltip("La salud máxima del Core.")]
    public float coreHealth = 100f; // Salud máxima del Core
}
