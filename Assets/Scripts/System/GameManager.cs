using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource musicSource;

    // PATRÓN SINGLETON: Permite que cualquier otro script acceda al GameManager 
    // usando "GameManager.Instance" sin necesidad de buscar la referencia.
    public static GameManager Instance { get; private set; }

    [Header("Datos Persistentes (Guardado Temporal)")]
    // Estas variables almacenan el estado del jugador entre cambios de escena.
    // [HideInInspector] hace que no aparezcan en el Inspector de Unity para evitar desorden.
    [HideInInspector] public float savedHealth = -1;
    [HideInInspector] public int savedContenedores;
    [HideInInspector] public int savedMunicion;
    [HideInInspector] public bool savedPistola, savedRifle, savedEscopeta;

    [Header("Referencias del Jugador")]
    public PlayerLife playerLife;

    [Header("Menús e Interfaces (Canvas)")]
    [Tooltip("El menú de Game Over / Pausa que se activará (UI_GameMenu).")]
    public GameObject uiGameMenu;

    // Lista de elementos de la UI (como el HUD de vida/balas) que deben ocultarse al pausar o morir.
    public List<GameObject> objetosUIaOcultar;

    // Control de estados internos del juego
    private bool juegoTerminado = false;
    private bool menuActivoPorPausa = false;

    [Header("Gestión de Victoria")]
    // Lista que contiene a todos los zombies del nivel para comprobar si el jugador los eliminó a todos.
    public List<ZombieLife> listaZombies;
    public GameObject uiVictoria;
    private bool victoriaActivada = false;

    [Header("Spawneo del Jugador")]
    public GameObject playerPrefab; // El prefab del jugador que se instanciará si no existe.
    private GameObject jugadorInstanciado; // Almacena la referencia en escena del jugador actual.

    void Start()
    {
        // Inicialización del estado visual al arrancar el nivel.
        if (uiGameMenu != null) uiGameMenu.SetActive(false);

        foreach (GameObject obj in objetosUIaOcultar)
        {
            if (obj != null) obj.SetActive(true);
        }
    }

    void Update()
    {
        // 1. COMPROBACIÓN DE MUERTE: Si el jugador muere y el juego no ha terminado, activa el menú.
        if (!juegoTerminado && playerLife != null && playerLife.IsDead)
        {
            ActivarMenuMuerte();
        }

        // 2. CONTROL DE PAUSA: Al presionar 'Escape', alterna entre pausar y continuar el juego.
        if (Input.GetKeyDown(KeyCode.Escape) && !juegoTerminado)
        {
            if (menuActivoPorPausa) ContinuarJuego();
            else PausarJuegoPorTecla();
        }

        // 3. REINICIO RÁPIDO: Permite reiniciar el nivel de inmediato presionando la tecla 'R'.
        if (Input.GetKeyDown(KeyCode.R)) ReiniciarJuego();

        // 4. CONDICIÓN DE VICTORIA: Revisa constantemente si se ha ganado el nivel.
        if (!victoriaActivada) CheckVictoria();
    }

    // Limpia la lista eliminando los zombies destruidos (null) o aquellos que ya murieron.
    private void ActualizarListaZombies()
    {
        listaZombies.RemoveAll(z => z == null || z.GetComponent<ZombieLife>().currentHealth <= 0);
    }

    void CheckVictoria()
    {
        if (victoriaActivada) return;

        ActualizarListaZombies();

        // Si la lista se queda completamente vacía, significa que ganaste.
        if (listaZombies.Count == 0) ActivarVictoria();
    }

    private void ActivarVictoria()
    {
        victoriaActivada = true;
        Time.timeScale = 0f; // Congela el tiempo del juego (física, movimientos, etc.)
        if (uiVictoria != null) uiVictoria.SetActive(true);
    }

    private void ActivarMenuMuerte()
    {
        juegoTerminado = true;
        Time.timeScale = 1f; // Mantiene el tiempo corriendo (por si la pantalla de muerte tiene animaciones)
        if (uiGameMenu != null) uiGameMenu.SetActive(true);

        // Oculta interfaces secundarias para limpiar la pantalla de Game Over
        foreach (GameObject obj in objetosUIaOcultar)
        {
            if (obj != null) obj.SetActive(false);
        }
    }

    private void PausarJuegoPorTecla()
    {
        menuActivoPorPausa = true;
        if (uiGameMenu != null) uiGameMenu.SetActive(true);

        foreach (GameObject obj in objetosUIaOcultar)
        {
            if (obj != null) obj.SetActive(false);
        }

        Time.timeScale = 0f; // Congela por completo el juego
    }

    public void ContinuarJuego()
    {
        if (juegoTerminado) return; // No se puede despausar si ya moriste

        menuActivoPorPausa = false;
        if (uiGameMenu != null) uiGameMenu.SetActive(false);

        foreach (GameObject obj in objetosUIaOcultar)
        {
            if (obj != null) obj.SetActive(true);
        }

        Time.timeScale = 1f; // Devuelve el tiempo a su velocidad normal
    }

    public void ReiniciarJuego()
    {
        Debug.Log("[GAME MANAGER] Reiniciando nivel, manteniendo datos...");
        Time.timeScale = 1f; // Asegura que el juego no empiece congelado

        // Vuelve a cargar la escena actual desde cero.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void IrAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Scenes/Menu");
    }

    // SUSCRIPCIÓN A EVENTOS DE ESCENA
    // Permite que el GameManager detecte automáticamente cuándo cambia o se recarga una escena.
    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    // Este método se ejecuta AUTOMÁTICAMENTE justo después de que cualquier escena termina de cargar.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // FIX: Nos aseguramos de que el tiempo vuelva a correr a velocidad normal (1) al cargar cualquier escena.
        Time.timeScale = 1f;

        // Busca el objeto con el Tag "Respawn" en la nueva escena para usarlo como punto de origen.
        GameObject puntoSpawn = GameObject.FindGameObjectWithTag("Respawn");
        if (puntoSpawn == null) return;

        // GESTIÓN DEL JUGADOR ENTRE ESCENAS
        if (jugadorInstanciado == null)
        {
            // Si es la primera vez (o el jugador fue destruido), crea uno nuevo en el punto de Spawn.
            jugadorInstanciado = Instantiate(playerPrefab, puntoSpawn.transform.position, Quaternion.identity);
        }
        else
        {
            // Si el jugador ya existía de la escena anterior, simplemente lo teletransporta al nuevo punto.
            jugadorInstanciado.transform.position = puntoSpawn.transform.position;
        }

        // TRASPASO DE DATOS GUARDADOS
        // Envía la vida y los contenedores que teníamos guardados en el GameManager hacia el script del jugador.
        PlayerLife vida = jugadorInstanciado.GetComponent<PlayerLife>();
        if (vida != null) vida.InicializarVida(savedHealth, savedContenedores);
    }
}