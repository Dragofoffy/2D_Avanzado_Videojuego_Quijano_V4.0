using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class ElectricLightPulse : MonoBehaviour
{
    [Header("Configuración de Radio")]
    public float minOuter = 10f;
    public float maxOuter = 20f;
    public float minInner = 7f;
    public float maxInner = 15f;
    public float pulseSpeed = 2f;

    [Header("Configuración Volumétrica")]
    public float pulseDuration = 5f; // Segundos para un ciclo completo

    private Light2D light2D;

    void Awake()
    {
        light2D = GetComponent<Light2D>();
    }

    void Update()
    {
        // 1. Cálculo de pulso para el Radio (Mathf.Sin)
        float time = Time.time * pulseSpeed;
        float pulse = (Mathf.Sin(time) + 1f) / 2f;

        light2D.pointLightOuterRadius = Mathf.Lerp(minOuter, maxOuter, pulse);
        light2D.pointLightInnerRadius = Mathf.Lerp(minInner, maxInner, pulse);

        // 2. Cálculo de pulso para Volumetric (0 a 1 en 5 segundos)
        // PingPong devuelve un valor que oscila entre 0 y pulseDuration
        float volumetricPulse = Mathf.PingPong(Time.time, pulseDuration) / pulseDuration;

        // Asignamos el valor al parámetro de intensidad volumétrica
        light2D.volumeIntensity = volumetricPulse;
    }
}