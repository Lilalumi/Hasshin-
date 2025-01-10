using UnityEngine;

[CreateAssetMenu(menuName = "Paddle Powers/Stretch Power")]
public class PaddlePowerStretch : PaddlePowerBase
{
    public float stretchMultiplier = 2f; // Cuánto se estira la paleta
    public float stretchDuration = 5f; // Duración del estiramiento
    public float tweenDuration = 0.5f; // Duración de la animación de estiramiento y contracción

    public override void Activate(GameObject paddle)
    {
        Debug.Log("Paddle Power Stretch activated!");
        paddle.GetComponent<MonoBehaviour>().StartCoroutine(StretchEffect(paddle));
    }

    private System.Collections.IEnumerator StretchEffect(GameObject paddle)
    {
        // Guarda el tamaño original del Paddle
        Vector3 originalScale = paddle.transform.localScale;
        Vector3 stretchedScale = new Vector3(originalScale.x * stretchMultiplier, originalScale.y, originalScale.z);

        // Fase 1: Estiramiento con LeanTween
        LeanTween.scale(paddle, stretchedScale, tweenDuration).setEaseOutBounce();

        // Espera mientras el Paddle está estirado
        yield return new WaitForSeconds(stretchDuration);

        // Fase 2: Retorno al tamaño original con LeanTween
        LeanTween.scale(paddle, originalScale, tweenDuration).setEaseInBounce();

        Debug.Log("Paddle Power Stretch completed!");
    }
}
