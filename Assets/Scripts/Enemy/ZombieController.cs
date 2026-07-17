using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ZombieController : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float speed = 2f;
    public LayerMask zombieLayer;

    [Header("Configuración: Separación (Evitar amigos)")]
    public float separationRadius = 1.0f;
    public float separationForce = 0.2f;

    [Header("Configuración de IA (Tiempos de Rastro)")]
    [Tooltip("Tiempo en segundos que el zombie seguirá persiguiendo al jugador tras salir de su rango de detección.")]
    public float tiempoParaPerderRastro = 5f;

    [Header("Indicador de Confusión")]
    public GameObject confuseIndicator;
    [Range(0.1f, 1f)] public float confusionThreshold = 0.8f;

    private Transform targetPlayer;
    private float perderRastroTimer = 0f;
    private bool jugadorEnRangoDeteccion = false;

    private Vector2 lastDirection;
    private float confuseTimer = 0f;
    private bool isFirstFrame = true;

    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 smoothedDirection;
    private ZombieAttack zombieAttack;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        zombieAttack = GetComponent<ZombieAttack>();
        SetAsDynamic();

        if (confuseIndicator != null) confuseIndicator.SetActive(false);
    }

    public void SetAsDynamic()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    public void SetAsKinematic()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
    }

    void FixedUpdate()
    {
        if ((zombieAttack != null && zombieAttack.IsAttacking) || !CanMoveAfterAttack())
        {
            if (confuseIndicator != null && confuseIndicator.activeSelf) confuseIndicator.SetActive(false);
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (!jugadorEnRangoDeteccion && perderRastroTimer > 0f)
        {
            perderRastroTimer -= Time.fixedDeltaTime;
            if (perderRastroTimer <= 0f)
            {
                targetPlayer = null;
            }
        }

        if (targetPlayer != null)
        {
            Vector2 directionToPlayer = ((Vector2)targetPlayer.position - (Vector2)transform.position).normalized;
            Vector2 separation = GetSeparationForce();
            Vector2 finalDirection = (directionToPlayer + (separation * separationForce)).normalized;

            smoothedDirection = Vector2.MoveTowards(smoothedDirection, finalDirection, 10f * Time.fixedDeltaTime);
            rb.linearVelocity = smoothedDirection * speed;

            if (rb.linearVelocity.magnitude > 0.1f)
            {
                animator.SetFloat("MoveZombieX", smoothedDirection.x);
                animator.SetFloat("MoveZombieY", smoothedDirection.y);
            }

            DetectConfusion(finalDirection);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private bool CanMoveAfterAttack()
    {
        if (zombieAttack == null) return true;
        return Time.time >= zombieAttack.NextAttackTime;
    }

    public void RegistrarJugadorDetectado(Transform playerTransform)
    {
        targetPlayer = playerTransform;
        jugadorEnRangoDeteccion = true;
        perderRastroTimer = tiempoParaPerderRastro;
    }

    public void RegistrarJugadorSalio()
    {
        jugadorEnRangoDeteccion = false;
        perderRastroTimer = tiempoParaPerderRastro;
    }

    void DetectConfusion(Vector2 currentDirection)
    {
        if (confuseIndicator == null) return;

        if (isFirstFrame)
        {
            lastDirection = currentDirection;
            isFirstFrame = false;
            return;
        }

        float directionChange = Vector2.Dot(lastDirection, currentDirection);

        if (directionChange < confusionThreshold)
        {
            confuseIndicator.SetActive(true);
            confuseTimer = 0.5f;
        }

        lastDirection = currentDirection;

        if (confuseIndicator.activeSelf)
        {
            confuseTimer -= Time.fixedDeltaTime;
            if (confuseTimer <= 0f) confuseIndicator.SetActive(false);
        }
    }

    Vector2 GetSeparationForce()
    {
        Vector2 force = Vector2.zero;
        Collider2D[] others = Physics2D.OverlapCircleAll(transform.position, separationRadius, zombieLayer);
        int count = 0;
        foreach (Collider2D other in others)
        {
            if (other.gameObject != this.gameObject)
            {
                Vector2 diff = (Vector2)(transform.position - other.transform.position);
                force += diff; count++;
            }
        }
        if (count > 0) force /= count;
        return force;
    }

    public void DesplazarIndicadorPorDaño()
    {
        if (confuseIndicator != null) StartCoroutine(AnimarIndicadorSubida());
    }

    System.Collections.IEnumerator AnimarIndicadorSubida()
    {
        float tiempo = 0f;
        float duracion = 0.3f;
        Vector3 posInicial = new Vector3(confuseIndicator.transform.localPosition.x, 1.8f, confuseIndicator.transform.localPosition.z);
        Vector3 posFinal = new Vector3(confuseIndicator.transform.localPosition.x, 2.225f, confuseIndicator.transform.localPosition.z);

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            confuseIndicator.transform.localPosition = Vector3.Lerp(posInicial, posFinal, tiempo / duracion);
            yield return null;
        }
        confuseIndicator.transform.localPosition = posFinal;
    }
}