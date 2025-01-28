using UnityEngine;
using System.Collections; // Asegúrate de incluir esto para usar IEnumerator

public class EnemyShield : MonoBehaviour
{
    private EnemyShieldAbility shieldConfig;
    private bool isShieldActive = true; // Estado del escudo
    private SpriteRenderer sprite2Renderer; // Referencia al SpriteRenderer del Sprite 2
    private Coroutine blinkCoroutine; // Para manejar el blinkeo

    void Start()
    {
        if (shieldConfig != null)
        {
            CreateChildSprite("ShieldSprite1", shieldConfig.sprite1, shieldConfig.sprite1Size, shieldConfig.sprite1Offset, shieldConfig.secondarySpriteColor1, shieldConfig.sprite1OrderInLayer);
            sprite2Renderer = CreateChildSprite("ShieldSprite2", shieldConfig.sprite2, shieldConfig.sprite2Size, shieldConfig.sprite2Offset, shieldConfig.secondarySpriteColor2, shieldConfig.sprite2OrderInLayer);

            if (sprite2Renderer != null)
            {
                // Sprite 2 comienza completamente transparente
                Color color = sprite2Renderer.color;
                color.a = 0;
                sprite2Renderer.color = color;
            }
        }
    }

    public void ConfigureShield(EnemyShieldAbility config)
    {
        shieldConfig = config;
    }

    private SpriteRenderer CreateChildSprite(string name, Sprite sprite, Vector2 size, Vector3 offset, Color color, int orderInLayer)
    {
        if (sprite == null) return null;

        GameObject child = new GameObject(name);
        child.transform.SetParent(transform);
        child.transform.localPosition = offset;

        SpriteRenderer childSpriteRenderer = child.AddComponent<SpriteRenderer>();
        childSpriteRenderer.sprite = sprite;
        childSpriteRenderer.color = color;
        childSpriteRenderer.sortingOrder = orderInLayer;

        child.transform.localScale = new Vector3(size.x, size.y, 1f);
        return childSpriteRenderer;
    }

    public bool IsShieldActive()
    {
        return isShieldActive;
    }

    public void DisableShield()
    {
        isShieldActive = false;
        Debug.Log("Escudo desactivado.");
    }

    public void BreakShield()
    {
        if (!isShieldActive) return;

        // Desactivar el escudo
        DisableShield();

        // Destruir todos los hijos con SpriteRenderer (representan el escudo)
        foreach (Transform child in transform)
        {
            if (child.GetComponent<SpriteRenderer>() != null)
            {
                Destroy(child.gameObject);
            }
        }

        Debug.Log("¡Escudo destruido por Ball en SYNC!");
    }

    public void BlinkSprite2()
    {
        if (sprite2Renderer == null || shieldConfig == null) return;

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }

        blinkCoroutine = StartCoroutine(BlinkCoroutine(shieldConfig.blinkFrequency, shieldConfig.blinkDuration));
    }

    private IEnumerator BlinkCoroutine(float frequency, float duration)
    {
        float elapsedTime = 0f;
        bool isVisible = false;

        while (elapsedTime < duration)
        {
            elapsedTime += 1f / frequency;

            // Alternar la visibilidad
            isVisible = !isVisible;
            Color color = sprite2Renderer.color;
            color.a = isVisible ? 1f : 0f; // Alternar entre opaco y transparente
            sprite2Renderer.color = color;

            yield return new WaitForSeconds(1f / frequency);
        }

        // Al finalizar el parpadeo, volver a alpha 0
        Color finalColor = sprite2Renderer.color;
        finalColor.a = 0f;
        sprite2Renderer.color = finalColor;

        blinkCoroutine = null;
    }
}
