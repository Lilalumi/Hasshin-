using UnityEngine;

public class PaddleController : MonoBehaviour
{
    public Transform core; // Núcleo central alrededor del cual rotará
    public float speed = 200f;

    void Update()
    {
        // Invertimos la dirección del movimiento multiplicando el input por -1
        float input = Input.GetAxis("Horizontal") * -1; 
        transform.RotateAround(core.position, Vector3.forward, input * speed * Time.deltaTime);
    }
}
