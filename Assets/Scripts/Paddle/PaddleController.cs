using UnityEngine;

public class PaddleController : MonoBehaviour
{
    public Transform core; // Núcleo central alrededor del cual rotará
    public float speed = 200f; // Velocidad de rotación

    void Update()
    {
        float input = Input.GetAxis("Horizontal");
        if (input != 0)
        {
            transform.RotateAround(core.position, Vector3.forward, -input * speed * Time.deltaTime);
        }
    }
}
