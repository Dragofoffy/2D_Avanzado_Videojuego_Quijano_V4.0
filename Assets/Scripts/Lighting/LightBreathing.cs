using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

[RequireComponent(typeof(Light2D))]
public class LightBreathing : MonoBehaviour
{
    [Header("Configuración de Color")]
    [SerializeField] private Color colorA = Color.white; // Color "apagado" o base
    [SerializeField] private Color colorB = Color.red;   // Color "encendido" o pico

    [Header("Configuración de Tiempo")]
    [SerializeField] private float duration = 2.0f; // Duración de un ciclo completo (A -> B -> A)

    [Header("Opciones")]
    [SerializeField] private bool startOnAwake = true;
    // AnimationCurve permite personalizar la suavidad (curva ease-in ease-out por defecto)
    [SerializeField] private AnimationCurve breathingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Light2D light2D;
    private Coroutine breathingCoroutine;

    void Awake()
    {
        light2D = GetComponent<Light2D>();
        light2D.color = colorA;
    }

    void Start()
    {
        if (startOnAwake)
        {
            StartBreathe();
        }
    }

    /// <summary>
    /// Inicia el ciclo de respiración.
    /// </summary>
    public void StartBreathe()
    {
        if (breathingCoroutine != null) StopBreathe();
        breathingCoroutine = StartCoroutine(BreathingCycle());
    }

    /// <summary>
    /// Detiene el ciclo de respiración actual.
    /// </summary>
    public void StopBreathe()
    {
        if (breathingCoroutine != null)
        {
            StopCoroutine(breathingCoroutine);
            breathingCoroutine = null;
        }
    }

    private IEnumerator BreathingCycle()
    {
        while (true) // Bucle infinito para el ciclo
        {
            // --- Fase 1: Ir de A a B (Inhalar) ---
            yield return StartCoroutine(Transition(colorA, colorB, duration / 2f));

            // --- Fase 2: Ir de B a A (Exhalar) ---
            yield return StartCoroutine(Transition(colorB, colorA, duration / 2f));
        }
    }

    // Función auxiliar privada para realizar una transición simple (una dirección)
    private IEnumerator Transition(Color startCol, Color endCol, float time)
    {
        float elapsed = 0f;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float percentage = elapsed / time;

            // Aplicamos la curva de animación para la suavidad (EaseInOut por defecto)
            float curvePercent = breathingCurve.Evaluate(percentage);

            light2D.color = Color.Lerp(startCol, endCol, curvePercent);
            yield return null;
        }

        light2D.color = endCol; // Asegura el color final exacto al terminar
    }
}