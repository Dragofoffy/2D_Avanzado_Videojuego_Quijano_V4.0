using UnityEngine;

public class BalaProyectil : MonoBehaviour
{
    [Header("Ajustes")]
    public float tiempoDeVida = 3f;
    private float dañoBala;
    private Rigidbody2D rb;

    void Awake() { rb = GetComponent<Rigidbody2D>(); }

    // El arma nos dice qué velocidad usar al nacer
    public void Inicializar(float velocidad, float nuevoDaño)
    {
        dañoBala = nuevoDaño;
        if (rb != null)
        {
            rb.linearVelocity = transform.right * velocidad;
        }
    }

    void Start() { Destroy(gameObject, tiempoDeVida); }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Pickable")) return;

        // Buscamos el componente ZombieLife
        ZombieLife zombie = collision.GetComponent<ZombieLife>();

        if (zombie != null)
        {
            // Aquí llamamos al puente que acabamos de crear
            zombie.RecibirDaño(dañoBala);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Zombie")) // Por si acaso tienes otros zombies sin este script
        {
            Destroy(gameObject);
        }
    }
}