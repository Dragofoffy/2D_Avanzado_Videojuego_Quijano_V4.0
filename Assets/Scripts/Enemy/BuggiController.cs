using UnityEngine;
using System.Collections;

public class BuggiController : MonoBehaviour
{
    [Header("Configuraci�n")]
    public float speed = 3f;
    public int health = 4;
    public float knockbackForce = 2f;
    public Color damageColor = Color.red;

    private Transform player;
    private bool isAttached = false;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Rigidbody2D rb;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player != null && !isAttached)
        {
            // Perseguir al jugador
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detectar entrada al rango de detecci�n
        if (collision.CompareTag("Player") && !isAttached && !collision.isTrigger)
        {
            player = collision.transform;
        }

        if (collision.CompareTag("Player"))
        {
            PlayerController playerCtrl = collision.GetComponent<PlayerController>();
            if (playerCtrl != null)
            {
                // Cambia el multiplicador para que la velocidad sea 3
                // Si walkSpeed es 5, 3/5 = 0.6f
                playerCtrl.speedMultiplier = 0.1f;
            }
        }

        // Detectar contacto f�sico para inmovilizar
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            isAttached = true;
            rb.linearVelocity = Vector2.zero;
            transform.SetParent(player); // Se coloca "arriba" (como hijo)
            transform.localPosition = Vector3.zero; // Posici�n centrada en el jugador

            // Deshabilitar movimiento del jugador aqu� si tienes un script de control
            // Ejemplo: player.GetComponent<PlayerMovement>().enabled = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController playerCtrl = collision.GetComponent<PlayerController>();
            if (playerCtrl != null)
            {
                // Restaura la velocidad original
                playerCtrl.speedMultiplier = 1f;
            }
        }
    }

    // M�todo para recibir da�o (llamado desde el script del jugador)
    public void TakeDamage(Vector2 hitDirection)
    {
        if (health > 0)
        {
            health--;
            StartCoroutine(DamageFeedback());

            // Retroceso (Knockback)
            rb.AddForce(hitDirection * knockbackForce, ForceMode2D.Impulse);

            if (health <= 0) Die();
        }
    }

    private IEnumerator DamageFeedback()
    {
        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        transform.SetParent(null);
        Destroy(gameObject);
    }
}