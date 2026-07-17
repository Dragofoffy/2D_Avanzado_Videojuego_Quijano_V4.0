using UnityEngine;
using UnityEngine.Rendering.Universal; // Necesario para Light2D

public class PulseDepthLight : MonoBehaviour
{
    [Header("Configuración de Impulso")]
    public float baseIntensity = 1.0f;
    public float pulseAmplitude = 0.5f;
    public float pulseSpeed = 2.0f;

    [Header("Color Amanecer Celeste")]
    public Color dawnBlue = new Color(0.6f, 0.8f, 1.0f);

    private Light2D _light2D;

    void Start()
    {
        _light2D = GetComponent<Light2D>();
        _light2D.color = dawnBlue;
    }

    void Update()
    {
        // Función seno para un pulso suave
        float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseAmplitude;
        _light2D.intensity = baseIntensity + pulse;
    }
}