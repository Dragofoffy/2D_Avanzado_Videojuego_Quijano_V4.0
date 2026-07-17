using UnityEngine;
using TMPro; // Asegúrate de tener esto para usar TextMeshPro

public class UIManager : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject contenedorMunicion; // El icono + texto
    public TextMeshProUGUI textoContador; // El número de balas

    [Header("Referencia Sistema")]
    public GunSystem gunSystem;

    void Update()
    {
        // 1. Mostrar/Ocultar según si tiene algún arma desbloqueada
        bool tieneArma = gunSystem.tienePistola || gunSystem.tieneRifle || gunSystem.tieneEscopeta;
        contenedorMunicion.SetActive(tieneArma);

        // 2. Actualizar contador solo si es visible
        if (tieneArma)
        {
            textoContador.text = gunSystem.municionActual.ToString();
        }
    }
}