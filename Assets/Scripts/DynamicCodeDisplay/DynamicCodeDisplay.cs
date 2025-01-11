using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DynamicCodeDisplay : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI textMeshPro; // Referencia al TextMeshPro - Text (UI)
    [SerializeField] private float typingSpeed = 0.05f; // Velocidad de escritura (segundos entre caracteres)
    [SerializeField] private float delayBetweenTexts = 2f; // Tiempo de espera entre textos
    [SerializeField] private int maxLines = 10; // Máximo de líneas visibles en el texto

    [Header("Text Data Settings")]
    [SerializeField] private TextData[] textDataArray; // Array de ScriptableObjects con los textos
#if UNITY_EDITOR
    [SerializeField] private DefaultAsset folder; // Carpeta seleccionable desde el Inspector
#endif

    private Queue<string> visibleLines = new Queue<string>(); // Cola para manejar las líneas visibles
    private System.Random random = new System.Random(); // Generador de números aleatorios

    void Start()
    {
        if (textDataArray == null || textDataArray.Length == 0)
        {
            Debug.LogError("No se asignaron TextData en el Inspector.");
            return;
        }

        // Inicia la reproducción dinámica de los textos
        StartCoroutine(DisplayTexts());
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (folder != null)
        {
            LoadTextDataFromFolder();
        }
    }

    private void LoadTextDataFromFolder()
    {
        string folderPath = AssetDatabase.GetAssetPath(folder);

        if (string.IsNullOrEmpty(folderPath))
        {
            Debug.LogWarning("La carpeta seleccionada no es válida.");
            return;
        }

        // Busca todos los objetos TextData dentro de la carpeta
        string[] guids = AssetDatabase.FindAssets("t:TextData", new[] { folderPath });
        List<TextData> loadedTextData = new List<TextData>();

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            TextData textData = AssetDatabase.LoadAssetAtPath<TextData>(assetPath);
            if (textData != null)
            {
                loadedTextData.Add(textData);
            }
        }

        textDataArray = loadedTextData.ToArray(); // Actualiza el array con los resultados
        Debug.Log($"Se cargaron {textDataArray.Length} objetos TextData desde la carpeta.");
    }
#endif

    private IEnumerator DisplayTexts()
    {
        while (true)
        {
            // Selecciona un TextData aleatorio
            int randomIndex = random.Next(0, textDataArray.Length);
            string[] textLines = textDataArray[randomIndex].content.Split('\n'); // Divide el texto en líneas

            // Limpia las líneas visibles antes de mostrar el siguiente texto
            ClearVisibleLines();

            // Escribe línea por línea
            foreach (string line in textLines)
            {
                yield return StartCoroutine(TypeLine(line)); // Escribe una línea carácter por carácter
            }

            // Espera antes de pasar al siguiente texto
            yield return new WaitForSeconds(delayBetweenTexts);
        }
    }

    private IEnumerator TypeLine(string line)
    {
        string currentLine = ""; // Línea actual que se está escribiendo carácter por carácter

        foreach (char character in line)
        {
            currentLine += character; // Agrega el carácter actual
            UpdateVisibleLines(currentLine); // Actualiza las líneas visibles
            yield return new WaitForSeconds(typingSpeed); // Espera según la velocidad de escritura
        }

        // Una vez completada la línea, agrégala definitivamente
        AddLineToVisibleLines(currentLine);
    }

    private void AddLineToVisibleLines(string line)
    {
        // Agrega la línea completa a la cola de líneas visibles
        visibleLines.Enqueue(line);

        // Limita el número de líneas visibles
        if (visibleLines.Count > maxLines)
        {
            visibleLines.Dequeue(); // Elimina la línea más antigua
        }

        UpdateText(); // Actualiza el texto completo
    }

    private void UpdateVisibleLines(string inProgressLine)
    {
        // Crea una lista temporal con las líneas visibles más la línea en progreso
        var tempLines = new List<string>(visibleLines);
        tempLines.Add(inProgressLine);

        // Si excede el máximo de líneas visibles, elimina las más antiguas
        while (tempLines.Count > maxLines)
        {
            tempLines.RemoveAt(0);
        }

        // Actualiza el texto temporalmente
        textMeshPro.text = string.Join("\n", tempLines);
    }

    private void ClearVisibleLines()
    {
        visibleLines.Clear(); // Limpia la cola de líneas visibles
        textMeshPro.text = ""; // Limpia el texto en pantalla
    }

    private void UpdateText()
    {
        // Actualiza el texto combinando las líneas visibles en la cola
        textMeshPro.text = string.Join("\n", visibleLines);
    }
}
