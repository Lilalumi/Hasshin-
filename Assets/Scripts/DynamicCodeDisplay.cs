using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DynamicCodeDisplay : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI textMeshPro; // Referencia al TextMeshPro - Text (UI)

    [Header("Folder and File Settings")]
    #if UNITY_EDITOR
    [SerializeField] private DefaultAsset folder; // Carpeta seleccionable desde el Inspector
    #endif
    [SerializeField] private float typingSpeed = 0.05f; // Velocidad de escritura (segundos entre caracteres)
    [SerializeField] private float delayBetweenScripts = 2f; // Tiempo de espera entre scripts
    [SerializeField] private int maxLines = 10; // Máximo de líneas visibles en el texto

    private FileInfo[] scriptFiles; // Archivos .cs encontrados en la carpeta
    private Queue<string> visibleLines = new Queue<string>(); // Cola para manejar las líneas visibles
    private System.Random random = new System.Random(); // Generador de números aleatorios

    void Start()
    {
        // Obtener la ruta de la carpeta seleccionada
        string folderPath = GetFolderPath();

        if (!string.IsNullOrEmpty(folderPath) && Directory.Exists(folderPath))
        {
            DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
            scriptFiles = dirInfo.GetFiles("*.cs"); // Obtener todos los archivos .cs

            if (scriptFiles.Length > 0)
            {
                // Iniciar la escritura dinámica
                StartCoroutine(DisplayScripts());
            }
            else
            {
                Debug.LogError("No se encontraron archivos .cs en la carpeta especificada.");
            }
        }
        else
        {
            Debug.LogError("La carpeta especificada no existe o no es válida.");
        }
    }

    private IEnumerator DisplayScripts()
    {
        while (true)
        {
            // Elegir un archivo aleatorio
            int randomIndex = random.Next(0, scriptFiles.Length);
            string[] scriptLines = File.ReadAllLines(scriptFiles[randomIndex].FullName);

            // Limpiar todo el contenido antes de empezar con un nuevo script
            ClearVisibleLines();

            // Mostrar las líneas dinámicamente
            foreach (string line in scriptLines)
            {
                yield return StartCoroutine(TypeLine(line)); // Escribe una línea completa carácter por carácter
            }

            // Esperar antes de pasar al siguiente archivo
            yield return new WaitForSeconds(delayBetweenScripts);
        }
    }

    private IEnumerator TypeLine(string line)
    {
        string currentLine = ""; // Línea actual que se está escribiendo carácter por carácter

        foreach (char character in line)
        {
            currentLine += character; // Agregar el carácter actual
            UpdateVisibleLines(currentLine); // Actualizar las líneas visibles
            yield return new WaitForSeconds(typingSpeed); // Esperar el tiempo de escritura
        }

        // Una vez que se completa la línea, agregarla definitivamente
        AddLineToVisibleLines(currentLine);
    }

    private void AddLineToVisibleLines(string line)
    {
        // Agregar la línea completa a la cola de líneas visibles
        visibleLines.Enqueue(line);

        // Limitar el número de líneas visibles
        if (visibleLines.Count > maxLines)
        {
            visibleLines.Dequeue(); // Eliminar la línea más antigua
        }

        UpdateText(); // Actualizar el texto completo
    }

    private void UpdateVisibleLines(string inProgressLine)
    {
        // Crear una lista temporal con las líneas visibles más la que está en progreso
        var tempLines = new List<string>(visibleLines);
        tempLines.Add(inProgressLine);

        // Si excede el máximo de líneas visibles, eliminar las más antiguas
        while (tempLines.Count > maxLines)
        {
            tempLines.RemoveAt(0);
        }

        // Actualizar el texto temporalmente
        textMeshPro.text = string.Join("\n", tempLines);
    }

    private void ClearVisibleLines()
    {
        visibleLines.Clear(); // Limpiar la cola de líneas visibles
        textMeshPro.text = ""; // Limpiar el texto en pantalla
    }

    private void UpdateText()
    {
        // Actualiza el texto combinando las líneas visibles en la cola
        textMeshPro.text = string.Join("\n", visibleLines);
    }

    private string GetFolderPath()
    {
        #if UNITY_EDITOR
        if (folder != null)
        {
            string folderPath = AssetDatabase.GetAssetPath(folder);
            if (!string.IsNullOrEmpty(folderPath) && Directory.Exists(folderPath))
            {
                return folderPath;
            }
        }
        #endif
        return string.Empty;
    }
}
