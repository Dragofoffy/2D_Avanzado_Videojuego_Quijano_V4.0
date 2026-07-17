using UnityEngine;

public class GunSystem : MonoBehaviour
{
    [Header("Referencias de Armas")]
    [SerializeField] private GunWeapon pistola;
    [SerializeField] private GunWeapon rifle;
    [SerializeField] private GunWeapon escopeta;

    [Header("Configuración Munición")]
    public int municionActual = 30;
    public int municionMaxima = 99;

    [HideInInspector] public bool tienePistola, tieneRifle, tieneEscopeta;

    private GunWeapon armaActiva;
    private Camera cam;

    void Awake() => cam = Camera.main;
    void Start()
    {
        if (GameManager.Instance.savedHealth != -1) // Se puede usar cualquier variable como flag
        {
            municionActual = GameManager.Instance.savedMunicion;
            tienePistola = GameManager.Instance.savedPistola;
            tieneRifle = GameManager.Instance.savedRifle;
            tieneEscopeta = GameManager.Instance.savedEscopeta;
        }
        OcultarTodosLosSprites();
    }

    void Update()
    {
        // 1. Selección de armas mediante teclado
        if (Input.GetKeyDown(KeyCode.Alpha1) && tienePistola) EquiparArma(pistola);
        if (Input.GetKeyDown(KeyCode.Alpha2) && tieneRifle) EquiparArma(rifle);
        if (Input.GetKeyDown(KeyCode.Alpha3) && tieneEscopeta) EquiparArma(escopeta);

        if (!armaActiva) return;

        // 2. Control del arma
        RotarArmaHaciaMouse();

        if (Input.GetMouseButtonDown(0)) EnfundarArma();
        if (Input.GetMouseButton(1)) armaActiva.IntentarDisparar();
    }

    private void EquiparArma(GunWeapon nueva)
    {
        OcultarTodosLosSprites();
        armaActiva = nueva;
        if (armaActiva) armaActiva.SetVisibilidadSprite(true);
    }

    public void EnfundarArma()
    {
        OcultarTodosLosSprites();
        armaActiva = null;
    }

    private void OcultarTodosLosSprites()
    {
        if (pistola) pistola.SetVisibilidadSprite(false);
        if (rifle) rifle.SetVisibilidadSprite(false);
        if (escopeta) escopeta.SetVisibilidadSprite(false);
    }

    private void RotarArmaHaciaMouse()
    {
        Vector3 mPos = cam.ScreenToWorldPoint(Input.mousePosition);
        mPos.z = 0;
        Vector2 dir = (mPos - armaActiva.transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        armaActiva.transform.rotation = Quaternion.Euler(0, 0, angle);
        armaActiva.transform.localScale = new Vector3(1, Mathf.Abs(angle) > 90 ? -1 : 1, 1);
    }

    // Métodos llamados por los recolectables
    public bool ConsumirBala(int costo)
    {
        if (municionActual >= costo) { municionActual -= costo; return true; }
        return false;
    }

    public void RecargarMunicionGlobal(int cantidad)
    {
        municionActual = Mathf.Clamp(municionActual + cantidad, 0, municionMaxima);
    }

    public void RecolectarPistola() => tienePistola = true;
    public void RecolectarRifle() => tieneRifle = true;
    public void RecolectarEscopeta() => tieneEscopeta = true;
}