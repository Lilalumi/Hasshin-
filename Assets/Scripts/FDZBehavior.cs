using UnityEngine;

public class FDZBehavior : MonoBehaviour
{
    public float fadeSpeed = 2f; // Velocidad del efecto de fade in-fade out

    private SpriteRenderer spriteRenderer;
    private bool hasEnemies = false; // Indica si hay enemigos en la zona
    private float alphaValue = 0f; // Controla la transparencia actual

    void Start()
    {
        // Obtén el componente SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("El objeto FDZ no tiene un SpriteRenderer asignado.");
            return;
        }

        // Inicialmente oculta el sprite
        spriteRenderer.enabled = false;
    }

    void Update()
    {
        if (hasEnemies)
        {
            // Realiza el efecto de fade in-fade out
            spriteRenderer.enabled = true;
            alphaValue = Mathf.PingPong(Time.time * fadeSpeed, 1f); // Alterna entre 0 y 1
            Color color = spriteRenderer.color;
            color.a = alphaValue;
            spriteRenderer.color = color;
        }
        else
        {
            // Oculta el sprite si no hay enemigos
            spriteRenderer.enabled = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica si un enemigo ha ingresado al área
        if (other.CompareTag("Enemy"))
        {
            hasEnemies = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Verifica si un enemigo ha salido del área
        if (other.CompareTag("Enemy"))
        {
            hasEnemies = false;

            // Revisa si aún quedan enemigos dentro del área
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, GetComponent<CircleCollider2D>().radius);
            foreach (Collider2D col in colliders)
            {
                if (col.CompareTag("Enemy"))
                {
                    hasEnemies = true;
                    break;
                }
            }
        }
    }
}
