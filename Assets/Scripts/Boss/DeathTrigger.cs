using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    [Header("Configuración de Peligro")]
    public PlayerLife playerLife;
    public string causaDeMuerte = "Trampa";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // CAMBIO: Eliminamos el signo '!' para que la muerte ocurra 
            // solo cuando Shift SÍ está siendo presionado.
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (playerLife != null)
                {
                    // Llamamos a la muerte si el jugador corre/usa shift
                    playerLife.TakeDamage(999, causaDeMuerte);
                }
            }
            else
            {
                Debug.Log("Jugador a salvo: Shift no está presionado.");
            }
        }
    }
}