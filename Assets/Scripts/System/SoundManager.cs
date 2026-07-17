using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Referencias")]
    public PlayerController player;
    public AudioSource movementSource;
    public AudioSource effectsSource;

    [Header("Clips")]
    public AudioClip pickupSound;
    public AudioClip stunSound;
    public AudioClip walkSound;
    public AudioClip runSound;
    public AudioClip hitSound;

    [Header("Configuración de Seguimiento")]
    public bool seguirAlJugador = true;
    public Vector3 offsetPosition = Vector3.zero;

    private bool wasStunned;
    private Rigidbody2D rb;

    void Start()
    {
        if (player != null) rb = player.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null || rb == null) return;

        // 1. Lógica de Movimiento (Sigue en el Update porque es continua)
        float speed = rb.linearVelocity.magnitude;
        bool isMoving = speed > 0.1f;
        bool isRunning = isMoving && (speed > player.walkSpeed);

        HandleMovementAudio(isMoving, isRunning);

        // 2. Lógica de Stun
        if (player.isKnockedBack && !wasStunned)
        {
            PlayStun();
        }
        wasStunned = player.isKnockedBack;
    }

    void LateUpdate()
    {
        if (seguirAlJugador && player != null)
        {
            transform.position = player.transform.position + offsetPosition;
        }
    }

    private void HandleMovementAudio(bool moving, bool running)
    {
        if (!moving)
        {
            if (movementSource.isPlaying) movementSource.Stop();
            return;
        }

        AudioClip target = running ? runSound : walkSound;
        if (movementSource.clip != target || !movementSource.isPlaying)
        {
            movementSource.clip = target;
            movementSource.loop = true;
            movementSource.Play();
        }
    }

    // Métodos públicos que CUALQUIER script puede llamar en cualquier momento
    public void PlayPickup() => effectsSource.PlayOneShot(pickupSound);
    public void PlayHit() => effectsSource.PlayOneShot(hitSound);
    public void PlayStun() => effectsSource.PlayOneShot(stunSound);
}