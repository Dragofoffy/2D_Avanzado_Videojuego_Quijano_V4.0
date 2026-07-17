using UnityEngine;

public class HeartContainerItem : MonoBehaviour
{
    private bool yaFueRecogido = false; // Evita que se active dos veces en el mismo frame

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !yaFueRecogido)
        {
            PlayerLife playerLife = collision.GetComponent<PlayerLife>();

            if (playerLife != null)
            {
                yaFueRecogido = true;
                playerLife.AñadirContenedorCorazon(); // Suma el corazón
                Destroy(gameObject); // Se autodestruye de forma segura tras hacer el efecto
            }
        }
    }
}