using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Configuración de Combate")]
    public float damage = 100f;
    public float attackCooldown = 0.4f;
    public LayerMask enemyLayers; // Asigna aquí la capa de tus enemigos

    [Header("Referencias de la Jerarquía")]
    [Tooltip("Arrastra aquí el objeto hijo 'AttackPoint' de tu jerarquía")]
    public Transform attackPointTransform;

    private CircleCollider2D attackCollider;
    private Animator animator;
    private PlayerController playerController;
    private float nextAttackTime = 0f;

    // Distancia fija a la que se desplazará el objeto AttackPoint desde el centro del jugador
    private float distanciaDeGolpe = 0.8f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();

        // Si asignaste el transform en el inspector, buscamos su CircleCollider2D
        if (attackPointTransform != null)
        {
            attackCollider = attackPointTransform.GetComponent<CircleCollider2D>();

            // Forzamos a que sea un Trigger para evitar colisiones físicas raras al atacar
            if (attackCollider != null)
            {
                attackCollider.isTrigger = true;
            }
        }
    }

    void Update()
    {
        // Si el jugador está en medio de un Knockback (aturdido), no puede atacar
        if (playerController != null && playerController.isKnockedBack) return;

        // Control del Cooldown y ejecución del clic
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetMouseButtonDown(0)) // Clic Izquierdo del mouse
            {
                Atacar();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    private void Atacar()
    {
        FindObjectOfType<SoundManager>()?.PlayHit();

        if (attackPointTransform == null || attackCollider == null)
        {
            Debug.LogError("Falta asignar el AttackPoint o este no tiene un CircleCollider2D.");
            return;
        }

        // 1. Obtener la dirección actual que tiene el Animator
        float moveX = animator.GetFloat("MoveX");
        float moveY = animator.GetFloat("MoveY");

        // 2. Mover físicamente el objeto hijo 'AttackPoint' y disparar la animación de golpe correcta
        if (Mathf.Abs(moveX) > Mathf.Abs(moveY))
        {
            if (moveX > 0)
            {
                animator.SetTrigger("PunchRight");
                attackPointTransform.localPosition = new Vector2(distanciaDeGolpe, 0); // Derecha
            }
            else
            {
                animator.SetTrigger("PunchLeft");
                attackPointTransform.localPosition = new Vector2(-distanciaDeGolpe, 0); // Izquierda
            }
        }
        else
        {
            if (moveY > 0)
            {
                animator.SetTrigger("PunchUp");
                attackPointTransform.localPosition = new Vector2(0, distanciaDeGolpe); // Arriba
            }
            else
            {
                animator.SetTrigger("PunchDown");
                attackPointTransform.localPosition = new Vector2(0, -distanciaDeGolpe); // Abajo
            }
        }

        // 3. Detectar enemigos usando la posición global actual de tu objeto hijo 'AttackPoint' 
        // y el radio exacto que le configuraste a su CircleCollider2D
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPointTransform.position, attackCollider.radius, enemyLayers);

        // 4. Aplicar daño universal a los enemigos detectados en el área
        foreach (Collider2D enemy in hitEnemies)
        {
            ZombieLife zombie = enemy.GetComponent<ZombieLife>();
            if (zombie != null)
            {
                zombie.TakeDamage(damage);
                continue; // Si dañó al zombie, pasa al siguiente enemigo en el bucle
            }

            // Aquí podrás añadir los componentes de vida de futuros monstruos de forma limpia:
            // BossLife boss = enemy.GetComponent<BossLife>();
            // if (boss != null) boss.TakeDamage(damage);
        }
    }
}