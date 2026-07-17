using UnityEngine;

public class ArmaRecolectable : MonoBehaviour
{
    public enum TipoArma { Pistol, Rifle, Shotgun }

    [Header("Configuración del Recolectable")]
    [Tooltip("Selecciona qué arma se activará en el jugador al pisar este objeto.")]
    public TipoArma armaAOtorgar;

    // Este método lo llamará el script del Jugador al colisionar
    public void ProcesarRecogidaDesdeJugador(GunSystem sistemaArmas)
    {
        if (sistemaArmas == null) return;

        // Mandamos una señal directa al GunSystem dependiendo de lo elegido en el Inspector
        switch (armaAOtorgar)
        {
            case TipoArma.Pistol:
                sistemaArmas.RecolectarPistola();
                break;

            case TipoArma.Rifle:
                sistemaArmas.RecolectarRifle();
                break;

            case TipoArma.Shotgun:
                sistemaArmas.RecolectarEscopeta();
                break;
        }

        Debug.Log($"[RECOLECTABLE] Objeto {armaAOtorgar} procesado. Destruyendo objeto del suelo.");

        // El objeto del suelo desaparece inmediatamente
        Destroy(gameObject);
    }
}