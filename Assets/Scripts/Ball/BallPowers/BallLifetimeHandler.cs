using UnityEngine;

public class BallLifetimeHandler : MonoBehaviour
{
    private float lifetime = -1f; // Tiempo de vida de la pelota (-1 significa que no aplica)
    private int maxCollisions = -1; // Máximo número de colisiones (-1 significa que no aplica)
    private int currentCollisions = 0; // Colisiones actuales

    void Start()
    {
        if (lifetime > 0)
        {
            // Inicia la destrucción por tiempo si lifetime es mayor a 0
            Invoke(nameof(DestroyBall), lifetime);
        }
    }

    public void SetLifetime(float time)
    {
        lifetime = time;
    }

    public void SetMaxCollisions(int collisions)
    {
        maxCollisions = collisions;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (maxCollisions > 0)
        {
            currentCollisions++;
            if (currentCollisions >= maxCollisions)
            {
                DestroyBall();
            }
        }
    }

    private void DestroyBall()
    {
        Destroy(gameObject);
    }
}
