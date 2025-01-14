using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public PaddlePowerBase[] availablePaddlePowers; // Lista de poderes disponibles para el Paddle
    public BallPowerBase[] availableBallPowers;     // Lista de poderes disponibles para la Ball

    public void SelectPaddlePower(int index)
    {
        if (GameManager.Instance != null && index < availablePaddlePowers.Length)
        {
            GameManager.Instance.SetPaddlePower(availablePaddlePowers[index]);
        }
    }

    public void SelectBallPower(int index)
    {
        if (GameManager.Instance != null && index < availableBallPowers.Length)
        {
            GameManager.Instance.SetBallPower(availableBallPowers[index]);
        }
    }
}
