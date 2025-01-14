using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Ball Power Section")]
    public Image ballPowerIcon; // Referencia al ícono seleccionado
    public BallPowerBase[] availableBallPowers; // Lista de poderes disponibles para la Ball

    [Header("Paddle Power Section")]
    public Image paddlePowerIcon; // Referencia al ícono seleccionado
    public PaddlePowerBase[] availablePaddlePowers; // Lista de poderes disponibles para el Paddle

    public MainMenuManager mainMenuManager; // Referencia al MainMenuManager
    private BallPowerBase selectedBallPower;
    private PaddlePowerBase selectedPaddlePower;

    public void SelectBallPower(int index)
    {
        if (index < 0 || index >= availableBallPowers.Length)
        {
            Debug.LogError("Índice de BallPower fuera de rango.");
            return;
        }

        selectedBallPower = availableBallPowers[index];
        ballPowerIcon.sprite = selectedBallPower.powerIcon;
        GameManager.Instance.SetBallPower(selectedBallPower);

        // Notificar al MainMenuManager que se seleccionó un Ball Power
        mainMenuManager.SetBallPowerSelected(true);
    }

    public void SelectPaddlePower(int index)
    {
        if (index < 0 || index >= availablePaddlePowers.Length)
        {
            Debug.LogError("Índice de PaddlePower fuera de rango.");
            return;
        }

        selectedPaddlePower = availablePaddlePowers[index];
        paddlePowerIcon.sprite = selectedPaddlePower.powerIcon;
        GameManager.Instance.SetPaddlePower(selectedPaddlePower);

        // Notificar al MainMenuManager que se seleccionó un Paddle Power
        mainMenuManager.SetPaddlePowerSelected(true);
    }

    public void PlayGame()
    {
        if (selectedBallPower == null || selectedPaddlePower == null)
        {
            Debug.LogError("No se han seleccionado todos los poderes.");
            return;
        }

        SceneManager.LoadScene("Level"); // Cambia a la escena del nivel
    }

}
