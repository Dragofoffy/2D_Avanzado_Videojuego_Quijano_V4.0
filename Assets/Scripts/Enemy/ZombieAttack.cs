using System.Collections;
using UnityEngine;

public class ZombieAttack : MonoBehaviour
{
    [Header("Configuración: Fuerzas Físicas")]
    public float launchForce = 10f;
    public float liftForce = 4f;
    public float cooldownTime = 5f;
    public float stunDuration = 2f;

    [Header("Tiempos de Reacción (Velocidad)")]
    [Tooltip("Fase 1: Cuántos segundos demora en iniciar el amago de ataque y encender los efectos visuales.")]
    public float tiempoParaEfecto = 0.4f;
    [Tooltip("Fase 2: Cuántos segundos ADICIONALES debe permanecer el jugador en rango para recibir el daño.")]
    public float tiempoParaImpactoReal = 0.5f;

    [Header("Configuración de Daño (Probabilidad 50/50)")]
    public float danoPosible1 = 0.5f;
    public float danoPosible2 = 1.0f;

    [Header("Comportamiento Post-Mortem")]
    [Tooltip("Si está activado, el zombie seguirá golpeando el cadáver del jugador en bucle. Si está desactivado, se detendrá inmediatamente.")]
    public bool seguirAtacandoTrasMuerte = false;

    [Header("Efectos Visuales")]
    public GameObject attackEffect;
    public GameObject cooldownIndicator;
    public float attackAnimationDuration = 0.4f;

    private int originalSortingOrder = 18;
    private SpriteRenderer attackEffectRenderer;

    public bool IsAttacking { get; private set; } = false;
    public float NextAttackTime { get; private set; } = 0f;

    private Animator animator;
    private ZombieController movementController;
    private bool jugadorEnRangoAtaque = false;
    private PlayerController playerScriptReferencia;

    void Awake()
    {
        animator = GetComponent<Animator>();
        movementController = GetComponent<ZombieController>();

        if (attackEffect != null)
        {
            attackEffect.SetActive(false);
            attackEffectRenderer = attackEffect.GetComponent<SpriteRenderer>();
            originalSortingOrder = attackEffectRenderer.sortingOrder;
        }
        if (cooldownIndicator != null) cooldownIndicator.SetActive(false);
    }

    public void RegistrarJugadorEnRangoAtaque(PlayerController player)
    {
        playerScriptReferencia = player;
        jugadorEnRangoAtaque = true;

        if (!IsAttacking && Time.time >= NextAttackTime)
        {
            StartCoroutine(AtaqueZombieFases());
        }
    }

    public void RegistrarJugadorSalioAtaque()
    {
        jugadorEnRangoAtaque = false;
        if (!IsAttacking) playerScriptReferencia = null;
    }

    IEnumerator AtaqueZombieFases()
    {
        IsAttacking = true;
        if (movementController != null) movementController.SetAsKinematic();

        Vector2 dir = ((Vector2)playerScriptReferencia.transform.position - (Vector2)transform.position).normalized;
        bool isAttackingUp = false;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            if (dir.x > 0) animator.SetTrigger("AttackRight");
            else animator.SetTrigger("AttackLeft");
        }
        else
        {
            if (dir.y > 0) { animator.SetTrigger("AttackUp"); isAttackingUp = true; }
            else animator.SetTrigger("AttackDown");
        }

        // --- FASE 1: TIEMPO HASTA APARECER EL EFECTO ---
        yield return new WaitForSeconds(tiempoParaEfecto);

        if (attackEffect != null)
        {
            if (isAttackingUp) attackEffectRenderer.sortingOrder = 16;
            else attackEffectRenderer.sortingOrder = originalSortingOrder;
            attackEffect.SetActive(true);
        }

        // --- FASE 2: TIEMPO ADICIONAL PARA LA COMPROBACIÓN DEL GOLPE ---
        yield return new WaitForSeconds(tiempoParaImpactoReal);

        // Validamos si el jugador sigue dentro del rango físico
        if (jugadorEnRangoAtaque && playerScriptReferencia != null)
        {
            PlayerLife playerLife = playerScriptReferencia.GetComponent<PlayerLife>();

            // Si el jugador sigue vivo O está activada la opción de ensañarse con el cadáver
            if ((playerLife != null && !playerLife.IsDead) || seguirAtacandoTrasMuerte)
            {
                Vector2 directionToPlayer = ((Vector2)playerScriptReferencia.transform.position - (Vector2)transform.position).normalized;
                playerScriptReferencia.StartKnockback(stunDuration);

                Rigidbody2D playerRb = playerScriptReferencia.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.linearVelocity = Vector2.zero;
                    playerRb.AddForce(directionToPlayer * launchForce + Vector2.up * liftForce, ForceMode2D.Impulse);
                }

                if (playerLife != null)
                {
                    float danoFinal = (Random.value < 0.5f) ? danoPosible1 : danoPosible2;
                    playerLife.TakeDamage(danoFinal, gameObject.tag);
                }
            }
        }

        // Tiempos de recuperación tras el golpe
        NextAttackTime = Time.time + attackAnimationDuration + cooldownTime;

        yield return new WaitForSeconds(attackAnimationDuration);

        if (attackEffect != null) { attackEffect.SetActive(false); attackEffectRenderer.sortingOrder = originalSortingOrder; }
        if (cooldownIndicator != null) cooldownIndicator.SetActive(true);

        yield return new WaitForSeconds(cooldownTime);

        if (cooldownIndicator != null) cooldownIndicator.SetActive(false);

        if (movementController != null) movementController.SetAsDynamic();
        IsAttacking = false;

        // --- EVALUACIÓN DE REPETICIÓN EN CADENA ---
        if (jugadorEnRangoAtaque && playerScriptReferencia != null)
        {
            PlayerLife playerLife = playerScriptReferencia.GetComponent<PlayerLife>();

            // Si el jugador murió y NO tenemos activado el comportamiento post-mortem, frenamos el bucle aquí
            if (playerLife != null && playerLife.IsDead && !seguirAtacandoTrasMuerte)
            {
                yield break;
            }

            // Si pasa el filtro, vuelve a atacar inmediatamente
            StartCoroutine(AtaqueZombieFases());
        }
    }

    public void DesplazarIndicadorPorDaño()
    {
        if (cooldownIndicator != null) StartCoroutine(AnimarIndicadorSubida());
    }

    System.Collections.IEnumerator AnimarIndicadorSubida()
    {
        float tiempo = 0f;
        float duracion = 0.3f;
        Vector3 posInicial = new Vector3(cooldownIndicator.transform.localPosition.x, 1.8f, cooldownIndicator.transform.localPosition.z);
        Vector3 posFinal = new Vector3(cooldownIndicator.transform.localPosition.x, 2.1f, cooldownIndicator.transform.localPosition.z);

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            cooldownIndicator.transform.localPosition = Vector3.Lerp(posInicial, posFinal, tiempo / duracion);
            yield return null;
        }
        cooldownIndicator.transform.localPosition = posFinal;
    }
}