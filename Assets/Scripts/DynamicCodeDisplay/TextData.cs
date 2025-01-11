using UnityEngine;

[CreateAssetMenu(menuName = "Dynamic Text/Text Data")]
public class TextData : ScriptableObject
{
    [TextArea(5, 10)] // Permite un área de texto grande en el Inspector
    public string content; // Contenido del texto
}
