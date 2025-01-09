using UnityEngine;

public class DataShardsEnemy : MonoBehaviour
{
    public int dataShardsOnDeath = 5; // Cantidad de Data Shards que otorga este enemigo al morir
    public GameObject dataShardPrefab; // Prefab del objeto Data Shard
    public float floatRadius = 1f; // Radio de distribución aleatoria alrededor del enemigo

    private bool isQuitting = false; // Para evitar generar Data Shards cuando se cierra la escena

    private void OnApplicationQuit()
    {
        isQuitting = true; // Marca que la aplicación está cerrándose
    }

    private void OnDestroy()
    {
        // Evita generar Data Shards si se está cerrando la aplicación
        if (isQuitting) return;

        // Encuentra el DataShardsController en la escena
        DataShardsController dataShardsController = FindObjectOfType<DataShardsController>();

        if (dataShardsController != null)
        {
            // Agrega los Data Shards al contador global
            dataShardsController.AddDataShards(dataShardsOnDeath);
            Debug.Log($"Enemy destruido. {dataShardsOnDeath} Data Shards otorgados.");
        }
        else
        {
            Debug.LogWarning("No se encontró un DataShardsController en la escena. Asegúrate de que esté presente y activo.");
        }

        // Genera copias de los Data Shards
        GenerateDataShards();
    }

    private void GenerateDataShards()
    {
        if (dataShardPrefab == null)
        {
            Debug.LogError("No se asignó un prefab de Data Shard en el inspector.");
            return;
        }

        for (int i = 0; i < dataShardsOnDeath; i++)
        {
            // Genera una posición aleatoria dentro del radio
            Vector3 randomOffset = new Vector3(
                Random.Range(-floatRadius, floatRadius),
                Random.Range(-floatRadius, floatRadius),
                0
            );

            Vector3 spawnPosition = transform.position + randomOffset;

            // Instancia el Data Shard
            GameObject dataShard = Instantiate(dataShardPrefab, spawnPosition, Quaternion.identity);

            // Configura el comportamiento del Data Shard
            DataShardBehavior shardBehavior = dataShard.GetComponent<DataShardBehavior>();
            if (shardBehavior != null)
            {
                shardBehavior.Initialize(); // Llama al método Initialize sin parámetros
            }
        }
    }
}
