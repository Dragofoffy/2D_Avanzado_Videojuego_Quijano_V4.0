using UnityEngine;

public class HealingItem : MonoBehaviour
{
    [Header("Configuración de Curación")]
    [Tooltip("Cuántos corazones va a rellenar al tocarlo (0.5 = medio corazón, 1 = un corazón entero)")]
    public float cantidadCuracion = 1f;

    private bool yaFueRecogido = false; // Evita que se procese dos veces si hay un lag de frames

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Validamos que sea el jugador físicamente y que no haya sido recogido ya
        if (collision.CompareTag("Player") && !yaFueRecogido)
        {
            PlayerLife playerLife = collision.GetComponent<PlayerLife>();

            if (playerLife != null)
            {
                yaFueRecogido = true;
                playerLife.Curar(cantidadCuracion); // Aplica la curación en los corazones actuales

                // --- SOLUCIÓN: El objeto ahora se destruye a sí mismo inmediatamente ---
                Destroy(gameObject);
            }
        }
    }
}