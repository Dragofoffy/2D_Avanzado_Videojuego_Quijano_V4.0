using UnityEngine;

public class ItemTutorialUno : MonoBehaviour
{
    [Header("Referencia a la Cerca")]
    [Tooltip("Arrastra aquí el objeto Fence_Closed que contiene el script FenceController")]
    [SerializeField] private FenceController cerca;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificamos si el objeto que toca el trigger tiene el tag "Player"
        if (collision.CompareTag("Player"))
        {
            // Activamos la cerca si la referencia está asignada
            if (cerca != null)
            {
                cerca.ActivarCerca();
            }

            // Destruimos el objeto recolectado
            Destroy(gameObject);
        }
    }
}