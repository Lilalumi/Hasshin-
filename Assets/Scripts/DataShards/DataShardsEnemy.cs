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
        }

        // Genera copias de los Data Shards
        GenerateDataShards();
    }

    private void GenerateDataShards()
    {
        if (dataShardPrefab == null)
        {
            return;
        }

        DataShardsController controller = FindObjectOfType<DataShardsController>();

        if (controller == null)
        {
            return;
        }

        for (int i = 0; i < dataShardsOnDeath; i++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-floatRadius, floatRadius),
                Random.Range(-floatRadius, floatRadius),
                0
            );

            Vector3 spawnPosition = transform.position + randomOffset;

            GameObject dataShard = Instantiate(dataShardPrefab, spawnPosition, Quaternion.identity);

            // Registra el DataShard en el controlador
            controller.RegisterDataShard(dataShard);

            DataShardBehavior shardBehavior = dataShard.GetComponent<DataShardBehavior>();
            if (shardBehavior != null)
            {
                shardBehavior.Initialize();
            }
        }
    }
}
