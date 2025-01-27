using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PaddlePowerBase paddlePower;
    public BallPowerBase ballPower;
    public int dataShards;

    private PaddlePowerBase activePaddlePower; // Variable para almacenar el poder activo del Paddle
    private BallPowerBase activeBallPower; // Variable para almacenar el poder activo de la Ball

    void Awake()
    {
        // Implementa Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Vincula el evento de carga de escenas
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Aplica los valores a la escena cargada
        ApplyValuesToScene();
    }

    // Métodos para asignar poderes
    public void SetPaddlePower(PaddlePowerBase power)
    {
        if (power == null) return;

        paddlePower = Instantiate(power); // Crea una copia para la partida
    }

    public void SetBallPower(BallPowerBase power)
    {
        if (power == null) return;

        ballPower = Instantiate(power); // Crea una copia para la partida
    }

    // Métodos para obtener poderes
    public PaddlePowerBase GetPaddlePower()
    {
        return activePaddlePower;
    }

    public BallPowerBase GetBallPower()
    {
        return activeBallPower;
    }

    // Método para modificar DataShards
    public void AddDataShards(int amount)
    {
        dataShards += amount;
    }

    public int GetDataShards()
    {
        return dataShards;
    }

    public void ApplyValuesToScene()
    {
        // Configurar Paddle Power
        var paddle = FindObjectOfType<PaddlePower>();
        if (paddle != null && paddlePower != null)
        {
            paddle.powerBehavior = Instantiate(paddlePower);
        }

        // Configurar Ball Power
        var ball = FindObjectOfType<BallPower>();
        if (ball != null && ballPower != null)
        {
            ball.powerBehavior = Instantiate(ballPower);
        }

        // Configurar Data Shards
        var dataShardsController = FindObjectOfType<DataShardsController>();
        if (dataShardsController != null)
        {
            dataShardsController.SetDataShards(dataShards);
        }
    }
}
