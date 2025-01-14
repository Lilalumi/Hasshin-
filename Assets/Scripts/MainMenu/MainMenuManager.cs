using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public string levelSceneName = "Level"; // Nombre de la escena del nivel
    public Button playButton; // Referencia al botón Play
    private bool isBallPowerSelected = false;
    private bool isPaddlePowerSelected = false;

    void Start()
    {
        // Asegurarse de que el botón Play esté deshabilitado al inicio
        UpdatePlayButtonState();
    }

    // Método para cargar la escena del nivel
    public void OnPlayButtonPressed()
    {
        SceneManager.LoadScene(levelSceneName);
    }

    // Métodos para registrar las selecciones de poderes
    public void SetBallPowerSelected(bool selected)
    {
        isBallPowerSelected = selected;
        UpdatePlayButtonState();
    }

    public void SetPaddlePowerSelected(bool selected)
    {
        isPaddlePowerSelected = selected;
        UpdatePlayButtonState();
    }

    // Actualiza el estado del botón Play
    private void UpdatePlayButtonState()
    {
        playButton.interactable = isBallPowerSelected && isPaddlePowerSelected;
    }
}
