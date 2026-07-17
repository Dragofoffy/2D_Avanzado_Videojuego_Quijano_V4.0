using UnityEngine;

public class ZombieAttackZone : MonoBehaviour
{
    [Header("Configuración de Filtro")]
    [TagSelector] public string tagObjetivo = "Player";

    private ZombieAttack masterAttack;

    void Awake()
    {
        masterAttack = GetComponentInParent<ZombieAttack>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(tagObjetivo) && masterAttack != null)
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null) masterAttack.RegistrarJugadorEnRangoAtaque(player);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(tagObjetivo) && masterAttack != null)
        {
            masterAttack.RegistrarJugadorSalioAtaque();
        }
    }
}