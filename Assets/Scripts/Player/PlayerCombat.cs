using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private BuggiController currentTarget;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Buggi")) // Asegúrate de que el cuerpo de Buggi tenga este tag
            currentTarget = collision.GetComponent<BuggiController>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Buggi"))
            currentTarget = null;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && currentTarget != null)
        {
            // Vector de retroceso hacia atrás respecto al jugador
            Vector2 knockbackDir = (currentTarget.transform.position - transform.position).normalized;
            currentTarget.TakeDamage(knockbackDir);
        }
    }
}