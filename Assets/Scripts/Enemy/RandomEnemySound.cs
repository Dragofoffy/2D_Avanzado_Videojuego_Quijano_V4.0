using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomEnemySound : MonoBehaviour
{
    [Header("Configuración de Sonidos")]
    [Tooltip("Arrastra aquí los 2 (o más) clips de sonido")]
    public AudioClip[] soundOptions;

    [Header("Configuración de Distancia")]
    [Range(0f, 5f)] public float minDistance = 2f;
    [Range(5f, 50f)] public float maxDistance = 15f;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // --- Configuración 3D ---
        audioSource.spatialBlend = 1.0f;       // 1.0 = Sonido 3D, 0.0 = Sonido 2D
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;

        audioSource.loop = true;               // Mantener sonido en bucle
        audioSource.playOnAwake = false;       // Evitar que suene antes de elegir el clip
    }

    void Start()
    {
        if (soundOptions != null && soundOptions.Length > 0)
        {
            // Elegir un sonido aleatorio de la lista
            int randomIndex = Random.Range(0, soundOptions.Length);

            // Asignar el clip
            audioSource.clip = soundOptions[randomIndex];

            // Reproducir
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("No hay sonidos asignados en: " + gameObject.name);
        }
    }
}