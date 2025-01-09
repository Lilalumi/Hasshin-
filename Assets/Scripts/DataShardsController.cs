using UnityEngine;

public class DataShardsController : MonoBehaviour
{
    private int dataShardsCollected = 0; // Total de Data Shards recolectados

    // Método para agregar Data Shards
    public void AddDataShards(int amount)
    {
        dataShardsCollected += amount;
        Debug.Log($"Data Shards recolectados: {dataShardsCollected}");
    }

    // Método para obtener la cantidad actual de Data Shards
    public int GetDataShards()
    {
        return dataShardsCollected;
    }
}
