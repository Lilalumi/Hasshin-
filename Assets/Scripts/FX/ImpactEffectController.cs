using UnityEngine;

public class ImpactEffectController : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private Light light2D;

    void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
        light2D = GetComponent<Light>();
    }

    public void PlayEffect(Vector3 position)
    {
        transform.position = position;
        if (particleSystem != null)
        {
            particleSystem.Play();
        }
        if (light2D != null)
        {
            light2D.enabled = true;
        }

        // Detener el efecto después de su duración
        Invoke(nameof(StopEffect), particleSystem.main.duration);
    }

    private void StopEffect()
    {
        if (particleSystem != null)
        {
            particleSystem.Stop();
        }
        if (light2D != null)
        {
            light2D.enabled = false;
        }

        Destroy(gameObject);
    }
}
