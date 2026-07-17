using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    private float currentSpeed;

    [Header("Configuración de Stun (Efecto Visual)")]
    [Tooltip("Probabilidad de que el jugador gire al ser golpeado (0 = 0%, 1 = 100%)")]
    [Range(0f, 1f)] public float stunSpinProbability = 0.5f;
    public GameObject stunEffectObject;

    public float speedMultiplier = 1f; // 1 = normal, 0.6f = ralentizado (3f / 5f)

    private Rigidbody2D rb;
    private Animator animator;

    [HideInInspector] public bool isKnockedBack = false;
    private Coroutine currentKnockbackCoroutine;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Configuración física inicial para juego Top-Down 2D
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (stunEffectObject != null) stunEffectObject.SetActive(false);
    }

    void Update()
    {
        // Si el jugador está bajo el efecto de knockback/aturdido, congelamos el control
        if (isKnockedBack) return;

        // --- ENTRADA DE MOVIMIENTO (WASD / Flechas) ---
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 movement = new Vector2(moveX, moveY).normalized;

        // --- SISTEMA DE SPRINT (Shift Izquierdo) ---
        bool isMoving = movement.magnitude > 0.1f;
        bool wantsToRun = Input.GetKey(KeyCode.LeftShift);

        // Solo corre si se está moviendo físicamente y presiona la tecla
        bool isRunning = isMoving && wantsToRun;
        currentSpeed = (isRunning ? runSpeed : walkSpeed) * speedMultiplier;

        rb.linearVelocity = movement * currentSpeed;

        // --- ENVIAR DATOS AL ANIMATOR ---
        // 'IsRunning' controla la transición entre tus dos Blend Trees (Walk/Idle y RunState)
        animator.SetBool("IsRunning", isRunning);

        if (isMoving)
        {
            animator.SetFloat("MoveX", moveX);
            animator.SetFloat("MoveY", moveY);
        }

    }


    // --- DETECCIÓN Y RECOGIDA DE OBJETOS ACTUALIZADA ---
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Pickable"))
        {
            Vector2 direccionAlObjeto = (collision.transform.position - transform.position).normalized;
            ActivarAnimacionRecogida(direccionAlObjeto);

            GunSystem gunSystem = GetComponent<GunSystem>();
            if (gunSystem != null)
            {
                // REVISIÓN 1: ¿Es un arma?
                ArmaRecolectable armaDelSuelo = collision.GetComponent<ArmaRecolectable>();
                if (armaDelSuelo != null)
                {
                    armaDelSuelo.ProcesarRecogidaDesdeJugador(gunSystem);
                    return; // Terminamos para evitar buscar el otro script
                }

                // REVISIÓN 2: ¿Es munición?
                MunicionRecolectable cajaMunicion = collision.GetComponent<MunicionRecolectable>();
                if (cajaMunicion != null)
                {
                    cajaMunicion.ProcesarRecogidaDesdeJugador(gunSystem);
                }
            }
        }
    }

    private void ActivarAnimacionRecogida(Vector2 dir)
    {
        FindObjectOfType<SoundManager>()?.PlayPickup();

        // Evaluamos si el objeto está más hacia los lados (X) o arriba/abajo (Y)
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            if (dir.x > 0) animator.SetTrigger("PickUpRight");
            else animator.SetTrigger("PickUpLeft");
        }
        else
        {
            if (dir.y > 0) animator.SetTrigger("PickUpUp");
            else animator.SetTrigger("PickUpDown");
        }
    }

    // --- SISTEMA DE RECIBIR GOLPES (Llamado por el Zombie) ---
    public void StartKnockback(float duration)
    {
        if (currentKnockbackCoroutine != null) StopCoroutine(currentKnockbackCoroutine);
        currentKnockbackCoroutine = StartCoroutine(KnockbackRoutine(duration));
    }

    System.Collections.IEnumerator KnockbackRoutine(float duration)
    {
        isKnockedBack = true;
        rb.linearVelocity = Vector2.zero; // Detiene la inercia del jugador inmediatamente

        if (stunEffectObject != null) stunEffectObject.SetActive(true);

        // Lógica de probabilidad para el giro cosmético de 90 grados
        bool hasSpun = false;
        if (Random.value <= stunSpinProbability)
        {
            hasSpun = true;
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }

        // Fricción alta durante el empuje para frenar con estilo
        float originalDrag = rb.linearDamping;
        rb.linearDamping = 4f;

        yield return new WaitForSeconds(duration);

        // Restauración de estado original
        if (hasSpun) transform.rotation = Quaternion.identity;
        if (stunEffectObject != null) stunEffectObject.SetActive(false);

        rb.linearDamping = originalDrag;
        isKnockedBack = false;
        currentKnockbackCoroutine = null;
    }
}