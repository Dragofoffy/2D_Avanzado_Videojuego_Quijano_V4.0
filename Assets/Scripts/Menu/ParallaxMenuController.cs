using UnityEngine;

public class ParallaxMenuController : MonoBehaviour
{
    [System.Serializable]
    public struct ParallaxLayer
    {
        public Transform transformDeLaCapa; // El fondo físico (Sprite)
        [Range(0f, 50f)] public float velocidad; // Qué tanto se mueve (valores más altos = más rápido)
    }

    [Header("Configuración de Capas")]
    [Tooltip("Ordena de la más cercana (índice 0) a la más lejana (último índice). Dale más velocidad a las cercanas.")]
    public ParallaxLayer[] capasDeFondo;

    [Header("Límites de Movimiento")]
    [Tooltip("Máxima distancia en unidades de Unity que los fondos pueden moverse hacia los lados.")]
    public float limiteHorizontal = 2f;

    [Header("Suavizado")]
    [Tooltip("Qué tan suave es el movimiento. Valores bajos = más elástico y suave.")]
    public float suavizado = 5f;

    // Guardamos las posiciones iniciales de cada capa para no perder el centro
    private Vector3[] posicionesIniciales;

    void Start()
    {
        // Inicializamos el array de posiciones originales
        posicionesIniciales = new Vector3[capasDeFondo.Length];
        for (int i = 0; i < capasDeFondo.Length; i++)
        {
            if (capasDeFondo[i].transformDeLaCapa != null)
            {
                posicionesIniciales[i] = capasDeFondo[i].transformDeLaCapa.localPosition;
            }
        }
    }

    void Update()
    {
        // 1. Obtener la posición del mouse en un rango de -1 a 1 (0 es el centro de la pantalla)
        float mouseXNormalizado = (Input.mousePosition.x / Screen.width) * 2f - 1f;

        // 2. Recorrer cada capa y aplicar el movimiento
        for (int i = 0; i < capasDeFondo.Length; i++)
        {
            if (capasDeFondo[i].transformDeLaCapa == null) continue;

            // Calcular el destino basado en la posición del mouse, su velocidad y el límite
            float desplazamientoX = mouseXNormalizado * capasDeFondo[i].velocidad * 0.1f;

            // Limitamos el desplazamiento para que el fondo nunca deje ver los bordes vacíos
            desplazamientoX = Mathf.Clamp(desplazamientoX, -limiteHorizontal, limiteHorizontal);

            // Creamos la posición destino manteniendo la Y y Z originales de esa capa
            Vector3 posicionDestino = new Vector3(
                posicionesIniciales[i].x + desplazamientoX,
                posicionesIniciales[i].y,
                posicionesIniciales[i].z
            );

            // Aplicamos un movimiento suave (Interpolación lineal)
            capasDeFondo[i].transformDeLaCapa.localPosition = Vector3.Lerp(
                capasDeFondo[i].transformDeLaCapa.localPosition,
                posicionDestino,
                Time.deltaTime * suavizado
            );
        }
    }
}