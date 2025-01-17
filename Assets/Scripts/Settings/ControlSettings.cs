using UnityEngine;

public enum ControlMode
{
    Keyboard,
    Mouse,
    Gamepad
}

public class ControlSettings : MonoBehaviour
{
    public static ControlMode CurrentControlMode = ControlMode.Keyboard;

    [Header("Control Settings")]
    public bool automaticControlSwitch = true; // Toggle para cambio automático
    public ControlMode manualControlMode = ControlMode.Keyboard; // Modo de control manual

    private static float lastInputTime;
    private static float inputChangeCooldown = 0.5f; // Cooldown entre cambios de modo

    private ControlMode lastDetectedMode = ControlMode.Keyboard; // Último modo detectado

    void Update()
    {
        if (automaticControlSwitch)
        {
            DetectInput();
        }
        else
        {
            CurrentControlMode = manualControlMode; // Aplicar el modo manual si está deshabilitada la detección automática
        }
    }

    private void DetectInput()
    {
        if (Time.time - lastInputTime < inputChangeCooldown) return;

        // Prioridad 1: Detección de mouse
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Mathf.Abs(Input.GetAxis("Mouse X")) > 0 || Mathf.Abs(Input.GetAxis("Mouse Y")) > 0)
        {
            ChangeControlMode(ControlMode.Mouse);
            return;
        }

        // Prioridad 2: Detección de gamepad (excluir teclado)
        if (DetectGamepadInput())
        {
            ChangeControlMode(ControlMode.Gamepad);
            return;
        }

        // Prioridad 3: Detección de teclado (excluir gamepad)
        if (DetectKeyboardInput())
        {
            ChangeControlMode(ControlMode.Keyboard);
        }
    }

    private bool DetectKeyboardInput()
    {
        // Detectar teclas específicas del teclado (excluir joystick)
        return Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.anyKeyDown;
    }

    private bool DetectGamepadInput()
    {
        // Detectar inputs específicos del gamepad (ejes o botones)
        return Mathf.Abs(Input.GetAxis("JoystickHorizontal")) > 0.1f ||
               Mathf.Abs(Input.GetAxis("JoystickVertical")) > 0.1f ||
               Input.GetButtonDown("Submit");
    }

    private void ChangeControlMode(ControlMode newMode)
    {
        if (CurrentControlMode != newMode)
        {
            CurrentControlMode = newMode;
            lastDetectedMode = newMode;
            lastInputTime = Time.time;
        }
    }

    public static ControlMode GetCurrentMode()
    {
        return CurrentControlMode;
    }

    // Métodos públicos para ser asignados a los botones en el MainMenu
    public void SetKeyboardMode()
    {
        automaticControlSwitch = false;
        manualControlMode = ControlMode.Keyboard;
        ChangeControlMode(ControlMode.Keyboard);
    }

    public void SetMouseMode()
    {
        automaticControlSwitch = false;
        manualControlMode = ControlMode.Mouse;
        ChangeControlMode(ControlMode.Mouse);
    }
}
