using UnityEngine;

public class Health : MonoBehaviour
{
    public int health = 100; // Salud inicial del objeto
    public float effectDuration = 1f; // Duración del efecto de muerte
    public float sizeAugment = 2f; // Velocidad de aumento de tamaño durante el efecto

    private bool isDying = false; // Bandera para evitar múltiples activaciones del efecto

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0 && !isDying)
        {
            Die();
        }
    }

    private void Die()
    {
        isDying = true;

        // Deshabilitar los colliders
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }

        // Iniciar la animación de muerte
        StartCoroutine(DeathEffect());
    }

    private System.Collections.IEnumerator DeathEffect()
    {
        float elapsedTime = 0f;
        Vector3 initialScale = transform.localScale;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("El objeto no tiene un SpriteRenderer. No se puede realizar el efecto de fade out.");
            yield break;
        }

        // Color inicial para manejar la transparencia
        Color initialColor = spriteRenderer.color;

        while (elapsedTime < effectDuration)
        {
            elapsedTime += Time.deltaTime;

            // Aumentar el tamaño
            float scaleMultiplier = 1f + (elapsedTime / effectDuration) * sizeAugment;
            transform.localScale = initialScale * scaleMultiplier;

            // Hacer fade out
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / effectDuration);
            spriteRenderer.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);

            yield return null;
        }

        // Asegurarse de que el objeto sea completamente invisible y eliminarlo
        spriteRenderer.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);
        Destroy(gameObject);
    }
}
