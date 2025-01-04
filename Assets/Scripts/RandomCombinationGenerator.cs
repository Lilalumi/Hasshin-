using UnityEngine;
using TMPro;

public class RandomCombinationGenerator : MonoBehaviour
{
    [Header("UI Component")]
    public TextMeshProUGUI textMeshPro; // Asigna el TextMeshPro desde el Inspector

    [Header("Generation Settings")]
    [SerializeField] private int framesBetweenUpdates = 3; // Frames para generar una nueva combinación

    private int frameCounter = 0;

    void Update()
    {
        // Incrementar el contador de frames
        frameCounter++;

        // Generar una nueva combinación si se alcanza el número de frames
        if (frameCounter >= framesBetweenUpdates)
        {
            GenerateRandomCombination();
            frameCounter = 0; // Reiniciar el contador
        }
    }

    private void GenerateRandomCombination()
    {
        // Generar una letra aleatoria
        char randomLetter = (char)Random.Range('A', 'Z' + 1);

        // Generar una lista de números únicos (0-9)
        int[] uniqueNumbers = GenerateUniqueNumbers(5);

        // Convertir la combinación a una cadena
        string randomCombination = randomLetter.ToString();
        foreach (int num in uniqueNumbers)
        {
            randomCombination += num.ToString();
        }

        // Actualizar el texto del TextMeshPro
        textMeshPro.text = randomCombination;
    }

    private int[] GenerateUniqueNumbers(int count)
    {
        // Generar una lista de números únicos
        int[] numbers = new int[count];
        System.Collections.Generic.HashSet<int> usedNumbers = new System.Collections.Generic.HashSet<int>();

        for (int i = 0; i < count; i++)
        {
            int randomNumber;
            do
            {
                randomNumber = Random.Range(0, 10); // Generar un número aleatorio entre 0-9
            }
            while (usedNumbers.Contains(randomNumber)); // Asegurarse de que sea único

            usedNumbers.Add(randomNumber);
            numbers[i] = randomNumber;
        }

        return numbers;
    }
}
