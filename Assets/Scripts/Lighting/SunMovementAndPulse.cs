using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SunMovementAndPulse : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float movementRange = 5.0f; // Qué tan lejos se mueve a cada lado
    public float moveSpeed = 1.0f;     // Velocidad de traslación

    [Header("Configuración de Intensidad")]
    public float minIntensity = 0.5f;
    public float maxIntensity = 2.0f;
    public float pulseSpeed = 1.5f;    // Velocidad del pulso de luz

    private Light2D _light2D;
    private Vector3 _startPosition;

    void Start()
    {
        _light2D = GetComponent<Light2D>();
        _startPosition = transform.position;
    }

    void Update()
    {
        // 1. Movimiento Suave de lado a lado (PingPong)
        float xOffset = Mathf.PingPong(Time.time * moveSpeed, movementRange * 2) - movementRange;
        transform.position = _startPosition + new Vector3(xOffset, 0, 0);

        // 2. Pulso de Intensidad (Sinusoidal)
        // Usamos (Mathf.Sin + 1) / 2 para mapear el rango de -1 a 1 en 0 a 1
        float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1.0f) / 2.0f;
        _light2D.intensity = Mathf.Lerp(minIntensity, maxIntensity, pulse);
    }
}