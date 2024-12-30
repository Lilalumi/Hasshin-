using UnityEngine;
using TMPro;

public class EnemyCountUI : MonoBehaviour
{
    public TextMeshProUGUI enemyCountText; // Referencia al texto en el UI
    public Transform enemyController; // Referencia al objeto que contiene los enemigos

    private int lastEnemyCount = -1; // Último conteo registrado de enemigos

    void Update()
    {
        if (enemyController == null || enemyCountText == null) return;

        // Obtén la cantidad actual de enemigos
        int currentEnemyCount = enemyController.childCount;

        // Actualiza el texto solo si cambia el conteo
        if (currentEnemyCount != lastEnemyCount)
        {
            lastEnemyCount = currentEnemyCount;
            enemyCountText.text = $"{currentEnemyCount}";
        }
    }
}
