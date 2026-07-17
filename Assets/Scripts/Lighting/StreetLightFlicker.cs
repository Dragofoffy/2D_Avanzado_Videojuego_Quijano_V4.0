using UnityEngine;
using UnityEngine.Rendering.Universal; // Necesario para Light2D

public class StreetLightFlicker : MonoBehaviour
{
    public enum LightMode { Normal, Fallado }

    [Header("Configuración")]
    public LightMode modo = LightMode.Normal;

    [Range(0, 10)]
    public float intensidadNormal = 1f;

    [Header("Configuración de Fallo")]
    public float velocidadTitileo = 0.1f;
    public float intensidadMinima = 0.2f;
    public float intensidadMaxima = 1.2f;

    private Light2D luz2D;
    private float tiempoSiguienteTitileo;

    void Start()
    {
        luz2D = GetComponent<Light2D>();
    }

    void Update()
    {
        if (luz2D == null) return;

        switch (modo)
        {
            case LightMode.Normal:
                luz2D.intensity = intensidadNormal;
                break;

            case LightMode.Fallado:
                AplicarTitileo();
                break;
        }
    }

    void AplicarTitileo()
    {
        if (Time.time > tiempoSiguienteTitileo)
        {
            // Genera una intensidad aleatoria y un tiempo de espera aleatorio
            luz2D.intensity = Random.Range(intensidadMinima, intensidadMaxima);
            tiempoSiguienteTitileo = Time.time + Random.Range(0.05f, velocidadTitileo);
        }
    }
}