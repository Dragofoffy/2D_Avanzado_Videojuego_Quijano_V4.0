using UnityEngine;
using UnityEngine.UI;

public class FlashEffectController : MonoBehaviour
{
    [Header("Configuración del Destello")]
    [Tooltip("El gradiente que define los colores del destello (ej. Transparente -> Blanco -> Transparente)")]
    public Gradient colorDelDestello;

    [Tooltip("Tiempo en segundos que tarda en completarse un destello")]
    public float duracionDelDestello = 1.5f;

    [Tooltip("Tiempo de espera en segundos entre cada destello")]
    public float tiempoEntreDestellos = 3f;

    // Componentes del logo (el script detectará automáticamente cuál usas)
    private Image logoImage;
    private SpriteRenderer logoSprite;

    private float cronometro;
    private bool estaDestellando = true;
    private Color colorOriginal;

    void Start()
    {
        // Detectar si el logo está en el Canvas o en el mundo 2D
        logoImage = GetComponent<Image>();
        logoSprite = GetComponent<SpriteRenderer>();

        // Guardar el color inicial para no perderlo
        if (logoImage != null) colorOriginal = logoImage.color;
        else if (logoSprite != null) colorOriginal = logoSprite.color;
        else Debug.LogError("No se encontró un componente Image o SpriteRenderer en este objeto.");
    }

    void Update()
    {
        cronometro += Time.deltaTime;

        if (estaDestellando)
        {
            // Calcular el progreso del destello actual (de 0 a 1)
            float progreso = cronometro / duracionDelDestello;

            // Evaluar el color del gradiente según el progreso
            Color colorActual = colorDelDestello.Evaluate(progreso);

            // Aplicar el color evaluado multiplicándolo por el original para mantener las siluetas
            AplicarColor(colorOriginal * colorActual);

            // Al terminar el destello, iniciar el tiempo de espera
            if (progreso >= 1f)
            {
                estaDestellando = false;
                cronometro = 0f;
                AplicarColor(colorOriginal); // Devolver el logo a su estado normal
            }
        }
        else
        {
            // Esperar el tiempo configurado antes de lanzar el siguiente destello
            if (cronometro >= tiempoEntreDestellos)
            {
                estaDestellando = true;
                cronometro = 0f;
            }
        }
    }

    void AplicarColor(Color nuevoColor)
    {
        if (logoImage != null) logoImage.color = nuevoColor;
        if (logoSprite != null) logoSprite.color = nuevoColor;
    }
}