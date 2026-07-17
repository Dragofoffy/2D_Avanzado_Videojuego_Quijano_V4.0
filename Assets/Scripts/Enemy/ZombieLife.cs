using UnityEngine;
using UnityEngine.UI;

public class ZombieLife : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [Tooltip("Puntos de vida del zombie configurable desde el Inspector (ej. 1500)")]
    public float maxHealth = 1500f;
    public float currentHealth;

    [Header("Interfaz de Usuario (UI)")]
    [Tooltip("Arrastra aquí el Canvas flotante que está sobre la cabeza del zombie")]
    public GameObject healthCanvas;
    [Tooltip("Arrastra aquí el componente Slider de la barra de vida")]
    public Slider healthSlider;

    [Header("Efecto de Impacto (Recibir Daño)")]
    [Tooltip("Arrastra aquí el objeto hijo de la jerarquía que contiene la animación visual de golpe.")]
    public GameObject efectoDañoObjeto;
    [Tooltip("Duración en segundos antes de volver a ocultar el objeto de animación de golpe.")]
    public float duracionEfectoDaño = 0.3f;

    [Header("Efectos de Muerte (Hijo de la Jerarquía)")]
    [Tooltip("Arrastra aquí el objeto hijo que tiene la animación de la sangre")]
    public GameObject bloodEffectObject;

    [Header("Configuración de Cadáver Pesado")]
    [Tooltip("La masa que tendrá el cuerpo al morir para que se sienta pesado al empujarlo.")]
    public float masaCadaver = 20f;
    [Tooltip("Fricción para que el cuerpo no se deslice como si estuviera en hielo.")]
    public float friccionCadaver = 10f;

    private Animator animator;
    private ZombieController movementScript;
    private ZombieAttack attackScript;
    private Rigidbody2D rb;
    private Collider2D col;

    void Awake()
    {
        animator = GetComponent<Animator>();
        movementScript = GetComponent<ZombieController>();
        attackScript = GetComponent<ZombieAttack>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        if (bloodEffectObject != null) bloodEffectObject.SetActive(false);
    }

    void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (healthCanvas != null) healthCanvas.SetActive(false);
        if (efectoDañoObjeto != null) efectoDañoObjeto.SetActive(false);
    }

    public void TakeDamage(float damageAmount, bool isPlayerPunch = true)
    {
        if (currentHealth <= 0) return;

        if (healthCanvas != null && !healthCanvas.activeSelf)
        {
            healthCanvas.SetActive(true);

            if (movementScript != null) movementScript.DesplazarIndicadorPorDaño();
            if (attackScript != null) attackScript.DesplazarIndicadorPorDaño();
        }

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (healthSlider != null) healthSlider.value = currentHealth;

        if (efectoDañoObjeto != null)
        {
            StartCoroutine(ReproducirEfectoDaño());
        }

        if (currentHealth <= 0)
        {
            Die(isPlayerPunch);
        }
    }

    private System.Collections.IEnumerator ReproducirEfectoDaño()
    {
        efectoDañoObjeto.SetActive(false);
        efectoDañoObjeto.SetActive(true);

        Animator animatorEfecto = efectoDañoObjeto.GetComponent<Animator>();
        if (animatorEfecto != null)
        {
            animatorEfecto.Play(0, -1, 0f);
        }

        yield return new WaitForSeconds(duracionEfectoDaño);

        efectoDañoObjeto.SetActive(false);
    }

    private void Die(bool isPlayerPunch)
    {
        // Ocultar la barra de vida inmediatamente al fallecer
        if (healthCanvas != null) healthCanvas.SetActive(false);

        // 1. Desactivar por completo la inteligencia artificial y sus ataques
        if (movementScript != null) movementScript.enabled = false;
        if (attackScript != null) attackScript.enabled = false;

        // Desactivar los sensores/triggers de los hijos (rangos de ataque y detección) para que no reviva
        Collider2D[] todosLosColisionadores = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D c in todosLosColisionadores)
        {
            if (c.isTrigger)
            {
                c.enabled = false;
            }
        }

        // 2. CONFIGURACIÓN DEL CADÁVER PESADO (FÍSICAS PERSISTENTES)
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic; // Mantener dinámico para que pueda ser empujado
            rb.linearVelocity = Vector2.zero;
            rb.mass = masaCadaver;
            rb.linearDamping = friccionCadaver;
            rb.angularDamping = friccionCadaver;
            rb.freezeRotation = true;
        }

        // --- ACTUALIZACIÓN DE MEDIDAS EXACTAS DEL BOX COLLIDER 2D ---
        if (col != null && col is BoxCollider2D box)
        {
            // Pasa de los valores de pie a los valores de cuerpo acostado en el suelo
            box.size = new Vector2(1.5f, 0.5f);
            box.offset = new Vector2(0.09f, 0.11f);
        }

        // 3. Disparar los triggers de animación de muerte según corresponda
        DeterminarAnimacionMuerte(isPlayerPunch);

        // 4. Calcular la duración del clip de animación de muerte
        float tiempoMuerte = 1.5f;
        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            tiempoMuerte = stateInfo.length;
        }

        // 5. Activar y animar el efecto de sangre independiente
        if (bloodEffectObject != null)
        {
            bloodEffectObject.SetActive(true);
            bloodEffectObject.transform.SetParent(null);

            StartCoroutine(BajarSangreCorrutina(tiempoMuerte));
            Destroy(bloodEffectObject, tiempoMuerte + 0.5f);
        }

        // Desactivamos este script de vida para sellar el estado de muerte definitiva
        this.enabled = false;
    }

    private void DeterminarAnimacionMuerte(bool isPlayerPunch)
    {
        if (animator == null) return;

        float moveX = animator.GetFloat("MoveZombieX");

        if (isPlayerPunch)
        {
            if (moveX >= 0) animator.SetTrigger("ZombieBig_Death_Right");
            else animator.SetTrigger("ZombieBig_Death_Left");
        }
        else
        {
            if (moveX >= 0) animator.SetTrigger("ZombieBig_Death_Right_Brutal");
            else animator.SetTrigger("ZombieBig_Death_Left_Brutal");
        }
    }

    System.Collections.IEnumerator BajarSangreCorrutina(float duracionTotal)
    {
        float tiempoTranscurrido = 0f;

        Vector3 posicionInicial = new Vector3(bloodEffectObject.transform.position.x, transform.position.y + 0.5f, bloodEffectObject.transform.position.z);
        Vector3 posicionFinal = new Vector3(bloodEffectObject.transform.position.x, transform.position.y, bloodEffectObject.transform.position.z);

        bloodEffectObject.transform.position = posicionInicial;

        while (tiempoTranscurrido < duracionTotal)
        {
            tiempoTranscurrido += Time.deltaTime;
            bloodEffectObject.transform.position = Vector3.Lerp(posicionInicial, posicionFinal, tiempoTranscurrido / duracionTotal);
            yield return null;
        }

        bloodEffectObject.transform.position = posicionFinal;
    }

    // Añade este método en tu script ZombieLife para que la bala pueda comunicarse
    public void RecibirDaño(float daño)
    {
        // Llamamos a tu lógica existente, pasando false para indicar 
        // que el daño no viene de un "puñetazo" sino de un arma/bala.
        TakeDamage(daño, false);
    }
}