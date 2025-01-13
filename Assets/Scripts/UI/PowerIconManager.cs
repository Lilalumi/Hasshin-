using UnityEngine;
using UnityEngine.UI;

public class PowerIconManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Image ballPowerIcon; // Ícono del poder activo de la Ball
    public Image paddlePowerIcon; // Ícono del poder activo del Paddle

    private void Start()
    {
        // Limpia los íconos al inicio
        ClearIcons();
    }

    public void SetBallPowerIcon(Sprite icon)
    {
        if (ballPowerIcon != null)
        {
            ballPowerIcon.sprite = icon;
            ballPowerIcon.enabled = icon != null; // Activa o desactiva el ícono según corresponda
        }
    }

    public void SetPaddlePowerIcon(Sprite icon)
    {
        if (paddlePowerIcon != null)
        {
            paddlePowerIcon.sprite = icon;
            paddlePowerIcon.enabled = icon != null; // Activa o desactiva el ícono según corresponda
        }
    }

    public void ClearIcons()
    {
        if (ballPowerIcon != null)
        {
            ballPowerIcon.sprite = null;
            ballPowerIcon.enabled = false;
        }

        if (paddlePowerIcon != null)
        {
            paddlePowerIcon.sprite = null;
            paddlePowerIcon.enabled = false;
        }
    }
}
