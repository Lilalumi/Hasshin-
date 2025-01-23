using UnityEngine;

public class BallPower : MonoBehaviour
{
    public float coolDown = 5f; // Tiempo de reutilización entre usos del poder
    public BallPowerBase powerBehavior; // Referencia al ScriptableObject del poder

    private bool isOnCoolDown = false; // Indica si el poder está en tiempo de reutilización
    private float coolDownTimeRemaining = 0f; // Tiempo restante del cooldown

    void Update()
    {
        // Detecta la entrada según el modo de control activo
        if (ShouldActivatePower() && !isOnCoolDown)
        {
            ActivatePower();
        }
    }

    private bool ShouldActivatePower()
    {
        // No permitir activar el poder si el juego está en pausa
        if (PauseManager.IsPaused) 
        {
            return false;
        }

        switch (ControlSettings.GetCurrentMode())
        {
            case ControlMode.Keyboard:
                return Input.GetKeyDown(KeyCode.X); // Activar con tecla X
            case ControlMode.Mouse:
                return Input.GetMouseButtonDown(1); // Activar con clic derecho
            case ControlMode.Gamepad:
                // Aquí se puede implementar lógica para Gamepad en el futuro
                return false;
            default:
                return false;
        }
    }

    private void ActivatePower()
    {
        if (powerBehavior != null)
        {
            // Activa el poder a través del ScriptableObject
            powerBehavior.Activate(gameObject);
        }
        else
        {
            Debug.LogError("No se asignó un comportamiento de poder en el inspector.");
        }

        // Inicia el tiempo de reutilización
        StartCoroutine(CoolDownRoutine());
    }

    private System.Collections.IEnumerator CoolDownRoutine()
    {
        isOnCoolDown = true;
        coolDownTimeRemaining = coolDown;

        while (coolDownTimeRemaining > 0)
        {
            if (!PauseManager.IsPaused) // Solo reducir el cooldown si el juego no está pausado
            {
                coolDownTimeRemaining -= Time.deltaTime;
            }
            yield return null;
        }

        isOnCoolDown = false;
    }

    public bool IsOnCoolDown()
    {
        return isOnCoolDown;
    }

    public float GetCoolDownTimeRemaining()
    {
        return coolDownTimeRemaining;
    }

    // Método para obtener el ícono del poder
    public Sprite GetPowerIcon()
    {
        return powerBehavior != null ? powerBehavior.powerIcon : null;
    }
}
