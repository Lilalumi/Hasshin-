using UnityEngine;

[CreateAssetMenu(menuName = "Paddle Powers/Magnet Power")]
public class PaddlePowerMagnet : PaddlePowerBase
{
    public float magnetDuration = 3f; // Duración del efecto magnético

    public override void Activate(GameObject paddle)
    {
        Debug.Log("Paddle Power Magnet activated!");
        paddle.GetComponent<MonoBehaviour>().StartCoroutine(MagnetEffect(paddle));
    }

    private System.Collections.IEnumerator MagnetEffect(GameObject paddle)
    {
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
        Transform paddleTransform = paddle.transform;

        foreach (GameObject ball in balls)
        {
            Rigidbody2D ballRigidbody = ball.GetComponent<Rigidbody2D>();
            if (ballRigidbody != null)
            {
                // Calcula la dirección hacia el Paddle
                Vector2 directionToPaddle = (paddleTransform.position - ball.transform.position).normalized;

                // Cambia la dirección de la pelota
                ballRigidbody.velocity = directionToPaddle * ballRigidbody.velocity.magnitude;
            }
        }

        // Espera el tiempo de duración del magnetismo
        yield return new WaitForSeconds(magnetDuration);

        Debug.Log("Paddle Power Magnet deactivated.");
    }
}
