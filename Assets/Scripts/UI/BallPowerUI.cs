using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal; // Necesario para Light 2D

public class BallPowerUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image powerCDPortrait; // Imagen de retrato del cooldown
    public Image powerCDReady; // Imagen que indica que el poder está listo
    [SerializeField] private Image powerIcon; // Imagen donde se mostrará el ícono del poder
    public TextMeshProUGUI powerCDTimer; // Texto para mostrar el tiempo de cooldown restante
    public Light2D readyLight; // Light 2D para el efecto de parpadeo

    [Header("Power Settings")]
    public BallPower ballPower; // Referencia al script de BallPower

    private float coolDownTimeRemaining = 0f; // Tiempo restante del cooldown
    private bool isBlinking = false; // Bandera para evitar múltiples corrutinas de parpadeo

    void Start()
    {
        if (ballPower != null)
        {
            // Asignar ícono desde BallPower
            Sprite icon = ballPower.GetPowerIcon();
            if (icon != null)
            {
                powerIcon.sprite = icon; // Asigna el ícono al componente UI
                powerIcon.gameObject.SetActive(true); // Asegura que esté activo
            }
            else
            {
                Debug.LogWarning("El poder asignado no tiene un ícono definido.");
                powerIcon.gameObject.SetActive(false); // Oculta la imagen si no hay ícono
            }
        }
        else
        {
            Debug.LogWarning("No se asignó un BallPower al PowerUI.");
            powerIcon.gameObject.SetActive(false); // Oculta la imagen si no hay BallPower asignado
        }
    }

    void Update()
    {
        if (ballPower != null && ballPower.IsOnCoolDown())
        {
            UpdateUI(ballPower.GetCoolDownTimeRemaining());
        }
        else
        {
            // Si el poder está listo, muestra Ready
            powerCDPortrait.gameObject.SetActive(false);
            powerCDTimer.gameObject.SetActive(false);
            powerCDReady.gameObject.SetActive(true);

            // Inicia el parpadeo de la luz si no está ya activo
            if (readyLight != null && !isBlinking)
            {
                StartBlinking();
            }
        }
    }

    private void UpdateUI(float coolDownTime)
    {
        // Muestra el ícono y el temporizador, oculta Ready
        powerCDPortrait.gameObject.SetActive(true);
        powerCDTimer.gameObject.SetActive(true);
        powerCDReady.gameObject.SetActive(false);

        // Detiene el parpadeo de la luz si está activo
        if (readyLight != null && isBlinking)
        {
            StopBlinking();
        }

        // Actualiza el texto del temporizador
        coolDownTimeRemaining = coolDownTime;
        powerCDTimer.text = FormatTime(coolDownTimeRemaining);
    }

    private string FormatTime(float time)
    {
        // Convierte el tiempo a formato "00.00" (segundos.centésimas)
        int seconds = Mathf.FloorToInt(time);
        int centiseconds = Mathf.FloorToInt((time - seconds) * 100);
        return $"{seconds:00}.{centiseconds:00}";
    }

    private void StartBlinking()
    {
        isBlinking = true;
        StartCoroutine(BlinkLight());
    }

    private void StopBlinking()
    {
        isBlinking = false;
        if (readyLight != null)
        {
            readyLight.intensity = 0f; // Asegúrate de que la intensidad sea 0 al detener
        }
    }

    private System.Collections.IEnumerator BlinkLight()
    {
        while (isBlinking)
        {
            if (readyLight != null)
            {
                // Aumenta la intensidad de 0 a 5 rápidamente
                readyLight.intensity = Mathf.PingPong(Time.time * 10f, 5f);
            }
            yield return null;
        }
    }
}
