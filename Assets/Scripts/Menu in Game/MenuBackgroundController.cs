using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBackgroundController : MonoBehaviour
{
    [Header("Configuración de Elevación")]
    [Tooltip("Posición Y inicial (oculto en el fondo de la pantalla).")]
    public float yInicial = -1800f;
    [Tooltip("Posición Y final (centrado en el Canvas).")]
    public float yFinal = -515f;
    [Tooltip("Duración en segundos de la animación de subida.")]
    public float duracionSubida = 0.5f;

    private RectTransform rectTransform;
    private List<IndividualParallaxElement> elementosHijos = new List<IndividualParallaxElement>();
    private bool elevacionCompletada = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // Buscamos y guardamos todos los scripts IndividualParallaxElement que estén en los hijos
        IndividualParallaxElement[] scriptsEncontrados = GetComponentsInChildren<IndividualParallaxElement>();
        elementosHijos.AddRange(scriptsEncontrados);
    }

    void OnEnable()
    {
        // Reiniciar la posición de elevación al fondo
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, yInicial);
        }

        elevacionCompletada = false;

        // Desactivamos el parallax en todos los hijos antes de subir
        foreach (IndividualParallaxElement elemento in elementosHijos)
        {
            elemento.SetParallaxActivo(false);
        }

        // Iniciamos la corrutina de elevación
        StartCoroutine(ElevarFondoCorrutina());
    }

    // --- CORRUTINA DE ELEVACIÓN (INDIFERENTE A TIME.TIMESCALE = 0) ---
    private IEnumerator ElevarFondoCorrutina()
    {
        float tiempoTranscurrido = 0f;
        Vector2 posInicial = new Vector2(rectTransform.anchoredPosition.x, yInicial);
        Vector2 posFinal = new Vector2(rectTransform.anchoredPosition.x, yFinal);

        while (tiempoTranscurrido < duracionSubida)
        {
            tiempoTranscurrido += Time.unscaledDeltaTime;
            float porcentaje = Mathf.Clamp01(tiempoTranscurrido / duracionSubida);

            // Suavizado suave
            float curvaSuave = Mathf.SmoothStep(0f, 1f, porcentaje);

            rectTransform.anchoredPosition = Vector2.Lerp(posInicial, posFinal, curvaSuave);
            yield return null;
        }

        rectTransform.anchoredPosition = posFinal;
        elevacionCompletada = true;

        // --- SEÑAL: Activamos el parallax independiente en todos los hijos ---
        foreach (IndividualParallaxElement elemento in elementosHijos)
        {
            elemento.SetParallaxActivo(true);
        }
    }
}