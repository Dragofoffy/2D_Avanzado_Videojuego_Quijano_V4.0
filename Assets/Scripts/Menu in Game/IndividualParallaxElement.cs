using UnityEngine;

public class IndividualParallaxElement : MonoBehaviour
{
    [Header("Configuración de Profundidad")]
    [Tooltip("Fuerza del movimiento respecto al cursor. Valores negativos mueven la imagen en dirección opuesta (más lejano), valores positivos la mueven hacia el cursor (más cercano).")]
    public float intensidadParallax = 10f;
    [Tooltip("Suavizado del movimiento del parallax para evitar tirones.")]
    public float suavizadoMouse = 5f;

    private RectTransform rectTransform;
    private Vector2 posicionOriginal;
    private bool parallaxHabilitado = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        // Guardamos la posición neutra original que tiene la imagen en el diseño del Canvas
        posicionOriginal = rectTransform.anchoredPosition;
    }

    // Método público que llama el padre (MenuBackgroundController) para encender/apagar el movimiento
    public void SetParallaxActivo(bool estado)
    {
        parallaxHabilitado = estado;

        // Si se desactiva, aseguramos que regrese a su posición original
        if (!parallaxHabilitado && rectTransform != null)
        {
            rectTransform.anchoredPosition = posicionOriginal;
        }
    }

    void Update()
    {
        // El parallax solo se ejecuta si el padre lo ha habilitado tras la elevación
        if (!parallaxHabilitado) return;

        AplicarParallaxMouseIndependiente();
    }

    private void AplicarParallaxMouseIndependiente()
    {
        // 1. Posición del mouse normalizada de -1 a 1
        Vector2 posicionMouseNormalizada = new Vector2(
            (Input.mousePosition.x / Screen.width) - 0.5f,
            (Input.mousePosition.y / Screen.height) - 0.5f
        );

        // 2. Calculamos el desplazamiento de este elemento específico
        Vector2 offsetParallax = new Vector2(
            posicionMouseNormalizada.x * intensidadParallax,
            posicionMouseNormalizada.y * intensidadParallax
        );

        // 3. Sumamos el desfase a la posición neutra original de la imagen
        Vector2 destinoFinal = posicionOriginal + offsetParallax;

        // 4. Interpolamos de forma suave hacia el destino usando unscaledDeltaTime por la pausa
        rectTransform.anchoredPosition = Vector2.Lerp(
            rectTransform.anchoredPosition,
            destinoFinal,
            Time.unscaledDeltaTime * suavizadoMouse
        );
    }
}