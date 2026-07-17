using UnityEngine;

public class ZombieDetectionZone : MonoBehaviour
{
    [Header("Configuración de Filtro")]
    [TagSelector] public string tagObjetivo = "Player";

    private ZombieController masterController;

    void Awake()
    {
        masterController = GetComponentInParent<ZombieController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(tagObjetivo) && masterController != null)
        {
            masterController.RegistrarJugadorDetectado(collision.transform);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(tagObjetivo) && masterController != null)
        {
            masterController.RegistrarJugadorDetectado(collision.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(tagObjetivo) && masterController != null)
        {
            masterController.RegistrarJugadorSalio();
        }
    }
}