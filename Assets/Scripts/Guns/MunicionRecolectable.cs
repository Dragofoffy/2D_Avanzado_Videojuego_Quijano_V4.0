using UnityEngine;

public class MunicionRecolectable : MonoBehaviour
{
    [Header("Configuración de Carga")]
    [Tooltip("Cantidad de balas que otorgará este paquete al jugador.")]
    public int cantidadBalas = 30;

    // Este método lo llamará el script del Jugador al colisionar con el Tag "Pickable"
    public void ProcesarRecogidaDesdeJugador(GunSystem sistemaArmas)
    {
        if (sistemaArmas != null)
        {
            // Le sumamos las balas al almacén central
            sistemaArmas.RecargarMunicionGlobal(cantidadBalas);

            Debug.Log($"[MUNICIÓN] Añadidas {cantidadBalas} balas al inventario.");

            // Desaparece del suelo
            Destroy(gameObject);
        }
    }
}