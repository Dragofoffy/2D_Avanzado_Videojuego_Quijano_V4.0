using UnityEngine;

public class FenceController : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject objetoADesactivar;

    // Esta variable la llamarás desde tu sistema de inventario o trigger
    public void ActivarCerca()
    {
        // 1. Activa la animación en el Animator
        animator.SetTrigger("ActivarAnimacion");

        // 2. Desactiva el objeto enlazado
        if (objetoADesactivar != null)
        {
            objetoADesactivar.SetActive(false);
        }
    }
}