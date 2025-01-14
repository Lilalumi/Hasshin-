using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class DataShardsController : MonoBehaviour
{
    private int dataShardsCollected = 0;
    private List<GameObject> activeDataShards = new List<GameObject>();

    private void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnSceneUnloaded(Scene scene)
    {
        Debug.Log("Escena cambiada. Eliminando todos los Data Shards activos.");
        DestroyAllDataShards();
    }
    public void SetDataShards(int amount)
    {
        dataShardsCollected = amount;
        Debug.Log($"Data Shards inicializados a: {dataShardsCollected}");
    }

    public void AddDataShards(int amount)
    {
        dataShardsCollected += amount;
        Debug.Log($"Data Shards recolectados: {dataShardsCollected}");
    }

    public int GetDataShards()
    {
        return dataShardsCollected;
    }

    public void RegisterDataShard(GameObject dataShard)
    {
        activeDataShards.Add(dataShard);
    }

    public void UnregisterDataShard(GameObject dataShard)
    {
        activeDataShards.Remove(dataShard);
    }

    public void DestroyAllDataShards()
    {
        foreach (GameObject dataShard in activeDataShards)
        {
            if (dataShard != null)
            {
                Destroy(dataShard);
            }
        }
        activeDataShards.Clear();
    }
}
