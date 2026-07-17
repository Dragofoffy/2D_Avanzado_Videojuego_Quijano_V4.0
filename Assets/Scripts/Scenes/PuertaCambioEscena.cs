using UnityEngine;

public class PuertaCambioEscena : MonoBehaviour
{
    public string nombreEscenaDestino;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Buscamos al jugador para obtener sus componentes
            PlayerLife vida = collision.GetComponent<PlayerLife>();
            GunSystem armas = collision.GetComponent<GunSystem>();

            // Cambiamos de escena
            UnityEngine.SceneManagement.SceneManager.LoadScene(nombreEscenaDestino);
        }
    }
}