using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLife : MonoBehaviour
{
    [Header("Configuración de Vida")]
    public int corazonesIniciales = 1; // Empieza con 1 corazón (Vida = 1.0)
    public int maxCorazonesPosibles = 7; // Límite físico del Canvas de tu UI

    public float currentHealth;
    private int contenedoresActuales;

    [Header("Sprites de Corazones (UI)")]
    public Sprite heartFull;
    public Sprite heartHalf;
    public Sprite heartEmpty;
    [Tooltip("Arrastra aquí los 7 GameObjects de tus corazones en orden del 1 al 7")]
    public List<GameObject> heartImages;

    // Propiedad pública que lee el ZombieAttack para saber si detener su bucle de agresión
    public bool IsDead { get; private set; } = false;

    private Animator animator;
    private PlayerController playerController;
    private PlayerAttack playerAttack;
    private Rigidbody2D rb;

    void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        playerAttack = GetComponent<PlayerAttack>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void InicializarVida(float saludGuardada, int contenedoresGuardados)
    {
        // Si tenemos datos guardados, los usamos; si no, ponemos valores por defecto
        if (saludGuardada != -1)
        {
            currentHealth = saludGuardada;
            contenedoresActuales = contenedoresGuardados;
        }
        else
        {
            currentHealth = corazonesIniciales;
            contenedoresActuales = corazonesIniciales;
        }
        ActualizarInterfazCorazones();
    }

    // --- MÉTODO PARA ACTUALIZAR LOS SPRITES DE LOS CORAZONES EN PANTALLA ---
    public void ActualizarInterfazCorazones()
    {
        for (int i = 0; i < heartImages.Count; i++)
        {
            if (i >= contenedoresActuales)
            {
                // Si el jugador aún no ha desbloqueado este contenedor, se oculta por completo del UI
                heartImages[i].SetActive(false);
            }
            else
            {
                // El contenedor está desbloqueado, lo encendemos y evaluamos su sprite correspondiente
                heartImages[i].SetActive(true);
                Image img = heartImages[i].GetComponent<Image>();

                if (img == null) continue;

                float vidaEnEsteCorazon = currentHealth - i;

                if (vidaEnEsteCorazon >= 1f)
                {
                    img.sprite = heartFull;
                }
                else if (vidaEnEsteCorazon >= 0.5f)
                {
                    img.sprite = heartHalf;
                }
                else
                {
                    img.sprite = heartEmpty;
                }
            }
        }
    }

    // --- LÓGICA DE RECIBIR DAÑO ---
    public void TakeDamage(float amount, string tagAgresor)
    {
        if (IsDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, contenedoresActuales);

        ActualizarInterfazCorazones();

        if (currentHealth <= 0)
        {
            Die(tagAgresor);
        }
    }

    // --- LÓGICA DE CURACIÓN (Rellena corazones existentes) ---
    public void Curar(float cantidad)
    {
        if (IsDead) return;

        currentHealth += cantidad;
        // No puede curarse más allá de los contenedores que tenga desbloqueados actualmente
        currentHealth = Mathf.Clamp(currentHealth, 0f, contenedoresActuales);

        ActualizarInterfazCorazones();
    }

    // --- LÓGICA DE AGREGAR CONTENEDOR (Objeto Corazón del suelo) ---
    public void AñadirContenedorCorazon()
    {
        if (IsDead) return;

        if (contenedoresActuales >= maxCorazonesPosibles) return;

        contenedoresActuales++;
        currentHealth = contenedoresActuales; // Al conseguir uno nuevo, lo rellenamos por comodidad

        ActualizarInterfazCorazones();
    }

    // --- LÓGICA DE MUERTE Y SU SINCRO DE ANIMACIONES ---
    public void Die(string tagAgresor)
    {
        IsDead = true;

        // Apagar controles y mecánicas para congelar al jugador
        if (playerController != null) playerController.enabled = false;
        if (playerAttack != null) playerAttack.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic; // Cambia a Kinematic para que los enemigos no empujen el cuerpo
        }

        // Evaluar qué Trigger de animación disparar en el Animator
        if (tagAgresor == "Explosion")
        {
            animator.SetTrigger("DieChest");
        }
        else if (tagAgresor == "ZombieEspecial") // Definido en el inspector del atacante o su tag
        {
            animator.SetTrigger("DieHead");
        }
        else
        {
            // Muerte regular por dirección de mirada del jugador (Blend Tree)
            float moveX = animator.GetFloat("MoveX");
            float moveY = animator.GetFloat("MoveY");

            // Izquierda (-1) o Arriba (Y > 0)
            if (moveX < -0.1f || (Mathf.Abs(moveY) > Mathf.Abs(moveX) && moveY > 0.1f))
            {
                animator.SetTrigger("DieLeft");
            }
            else // Derecha (1) o Abajo (Y < 0)
            {
                animator.SetTrigger("DieRight");
            }
        }

        Debug.Log($"[PLAYER LIFE] Muerto por: {tagAgresor}. Ejecutando animación.");
    }
}