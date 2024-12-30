using UnityEngine;

public class PaddlePower : MonoBehaviour
{
    public float coolDown = 5f; // Tiempo de reutilización entre usos del poder
    public PaddlePowerBase powerBehavior; // Referencia al ScriptableObject del poder

    private bool isOnCoolDown = false; // Indica si el poder está en tiempo de reutilización
    private float coolDownTimeRemaining = 0f; // Tiempo restante del cooldown

    void Update()
    {
        // Detecta la tecla "Z" para activar el poder
        if (Input.GetKeyDown(KeyCode.Z) && !isOnCoolDown)
        {
            ActivatePower();
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
            Debug.LogError("No se asignó un poder en el inspector.");
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
            coolDownTimeRemaining -= Time.deltaTime;
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
}
