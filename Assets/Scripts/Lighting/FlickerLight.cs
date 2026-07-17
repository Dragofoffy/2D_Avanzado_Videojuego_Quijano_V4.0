using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal; // Necesario para Light2D

public class FlickerLight : MonoBehaviour
{
    private Light2D _light2D; // Cambiamos el tipo a Light2D

    [Header("Configuración de Fuego")]
    public Color[] coloresFuego = {
        new Color(1f, 0.5f, 0f),    // Naranja
        new Color(1f, 0.2f, 0f),    // Rojo intenso
        new Color(1f, 0.8f, 0.2f)   // Amarillo/Blanco
    };

    public float minIntensidad = 0.5f;
    public float maxIntensidad = 1.5f;
    public float velocidadCambio = 0.2f;

    void Start()
    {
        // Buscamos el componente Light2D en el objeto
        _light2D = GetComponent<Light2D>();

        if (_light2D != null)
        {
            StartCoroutine(EfectoFuego());
        }
        else
        {
            Debug.LogError("No se encontró el componente Light2D en este objeto.");
        }
    }

    IEnumerator EfectoFuego()
    {
        while (true)
        {
            _light2D.color = coloresFuego[Random.Range(0, coloresFuego.Length)];
            _light2D.intensity = Random.Range(minIntensidad, maxIntensidad);

            yield return new WaitForSeconds(velocidadCambio);
        }
    }
}