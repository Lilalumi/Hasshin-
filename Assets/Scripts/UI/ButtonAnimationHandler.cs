using UnityEngine;
using UnityEngine.EventSystems; // Necesario para trabajar con eventos del mouse

public class ButtonAnimationHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Animator animator; // Referencia al Animator

    private void Awake()
    {
        // Asegúrate de que el botón tenga un Animator asignado
        animator = GetComponent<Animator>();
    }

    // Este método se ejecutará cuando el mouse pase por encima del botón
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (animator != null)
        {
            animator.Play("MouseEnterAnimation"); // Reemplaza con el nombre de tu animación
        }
    }

    // Este método se ejecutará cuando el mouse salga del botón
    public void OnPointerExit(PointerEventData eventData)
    {
        if (animator != null)
        {
            animator.Play("MouseExitAnimation"); // Reemplaza con el nombre de tu animación
        }
    }
}
