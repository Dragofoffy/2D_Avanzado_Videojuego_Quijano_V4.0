using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunWeapon : MonoBehaviour
{
    public enum IdentificadorArma { Pistol, Rifle, Shotgun }
    public IdentificadorArma tipoDeArma;

    [Header("Configuraciones")]
    public GameObject prefabBala;
    public float duracionLineaVisual = 0.05f;
    public float alcanceMaximo = 50f;

    [Header("Referencias")]
    public ConfiguracionPistola configuracionPistola;
    public ConfiguracionRifle configuracionRifle;
    public ConfiguracionEscopeta configuracionEscopeta;

    private float tiempoSiguienteDisparo = 0f;
    private SpriteRenderer sr;
    private GunSystem sistema;
    private bool estaDisparando = false;

    // Estructuras serializadas (Mantener igual)
    [System.Serializable] public struct ConfiguracionPistola { public float daño, cooldown, velocidadBala; public int balasPorDisparo; public Transform puntoDisparo; public LineRenderer lineaDisparo; }
    [System.Serializable] public struct ConfiguracionRifle { public float daño, cooldown, velocidadBala, tiempoEntreBalasRafaga; public int balasPorRafaga, balasPorDisparo; public Transform puntoDisparo; public List<LineRenderer> lineasDisparo; }
    [System.Serializable] public struct ConfiguracionEscopeta { public float daño, cooldown, velocidadBala, anguloDispersion; public int balasPorDisparo; public Transform puntoDisparo; public List<LineRenderer> lineasDisparo; }

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sistema = GetComponentInParent<GunSystem>();
    }

    void Update()
    {
        // Bloqueo estricto de visibilidad para evitar parpadeos
        if (sr && sr.enabled && !estaDisparando)
        {
            GestionarLaserVisual(true);
        }
        else if (!estaDisparando)
        {
            ApagarTodosLosLineRenderers();
        }
    }

    // Añade estos métodos dentro de la clase GunWeapon:

    private void DispararPistola()
    {
        Vector3 fin = configuracionPistola.puntoDisparo.position + (configuracionPistola.puntoDisparo.right * alcanceMaximo);
        CrearBala(configuracionPistola.puntoDisparo, configuracionPistola.velocidadBala, configuracionPistola.daño);
        StartCoroutine(MostrarLinea(configuracionPistola.lineaDisparo, configuracionPistola.puntoDisparo.position, fin));
    }

    private IEnumerator DispararRifleRafaga()
    {
        for (int i = 0; i < configuracionRifle.balasPorRafaga; i++)
        {
            CrearBala(configuracionRifle.puntoDisparo, configuracionRifle.velocidadBala, configuracionRifle.daño);
            // Asegúrate de que configuracionRifle.lineasDisparo tenga elementos asignados en el Inspector
            if (configuracionRifle.lineasDisparo.Count > 0)
            {
                StartCoroutine(MostrarLinea(configuracionRifle.lineasDisparo[i % configuracionRifle.lineasDisparo.Count],
                               configuracionRifle.puntoDisparo.position,
                               configuracionRifle.puntoDisparo.position + configuracionRifle.puntoDisparo.right * 10));
            }
            yield return new WaitForSeconds(configuracionRifle.tiempoEntreBalasRafaga);
        }
    }

    private void DispararEscopetaAbanico()
    {
        for (int i = 0; i < configuracionEscopeta.lineasDisparo.Count; i++)
        {
            float angulo = -(configuracionEscopeta.anguloDispersion / 2) + (i * (configuracionEscopeta.anguloDispersion / Mathf.Max(1, configuracionEscopeta.lineasDisparo.Count - 1)));
            Vector3 dir = Quaternion.Euler(0, 0, angulo) * configuracionEscopeta.puntoDisparo.right;
            CrearBala(configuracionEscopeta.puntoDisparo, configuracionEscopeta.velocidadBala, configuracionEscopeta.daño, dir);
            StartCoroutine(MostrarLinea(configuracionEscopeta.lineasDisparo[i], configuracionEscopeta.puntoDisparo.position, configuracionEscopeta.puntoDisparo.position + dir * 10));
        }
    }

    private void CrearBala(Transform punto, float velocidad, float daño, Vector3? direccion = null)
    {
        GameObject b = Instantiate(prefabBala, punto.position, punto.rotation);
        // Asegúrate de que el prefab de bala tenga el script "BalaProyectil"
        var balaScript = b.GetComponent<BalaProyectil>();
        if (balaScript) balaScript.Inicializar(velocidad, daño);
    }

    private void GestionarLaserVisual(bool activo)
    {
        LineRenderer laser = GetPrimerLineRenderer();
        if (laser && laser.enabled != activo)
        {
            laser.enabled = activo;
            if (activo) laser.sortingOrder = 10;
        }
    }

    public void ApagarTodosLosLineRenderers()
    {
        if (configuracionPistola.lineaDisparo) configuracionPistola.lineaDisparo.enabled = false;
        configuracionRifle.lineasDisparo?.ForEach(lr => { if (lr) lr.enabled = false; });
        configuracionEscopeta.lineasDisparo?.ForEach(lr => { if (lr) lr.enabled = false; });
    }

    private LineRenderer GetPrimerLineRenderer()
    {
        return tipoDeArma switch
        {
            IdentificadorArma.Pistol => configuracionPistola.lineaDisparo,
            IdentificadorArma.Rifle => configuracionRifle.lineasDisparo?.Count > 0 ? configuracionRifle.lineasDisparo[0] : null,
            IdentificadorArma.Shotgun => configuracionEscopeta.lineasDisparo?.Count > 0 ? configuracionEscopeta.lineasDisparo[0] : null,
            _ => null
        };
    }

    public void IntentarDisparar()
    {
        // Obtenemos datos de configuración mediante un método auxiliar para limpiar IntentarDisparar
        if (!ObtenerConfiguracionDisparo(out int costo, out float cooldown)) return;

        if (Time.time < tiempoSiguienteDisparo || !sistema.ConsumirBala(costo)) return;

        tiempoSiguienteDisparo = Time.time + cooldown;
        EjecutarLogicaDisparo();
    }

    private bool ObtenerConfiguracionDisparo(out int costo, out float cooldown)
    {
        costo = 0; cooldown = 0;
        switch (tipoDeArma)
        {
            case IdentificadorArma.Pistol: costo = configuracionPistola.balasPorDisparo; cooldown = configuracionPistola.cooldown; break;
            case IdentificadorArma.Rifle: costo = configuracionRifle.balasPorDisparo; cooldown = configuracionRifle.cooldown; break;
            case IdentificadorArma.Shotgun: costo = configuracionEscopeta.balasPorDisparo; cooldown = configuracionEscopeta.cooldown; break;
            default: return false;
        }
        return true;
    }

    private void EjecutarLogicaDisparo()
    {
        switch (tipoDeArma)
        {
            case IdentificadorArma.Pistol: DispararPistola(); break;
            case IdentificadorArma.Rifle: StartCoroutine(DispararRifleRafaga()); break;
            case IdentificadorArma.Shotgun: DispararEscopetaAbanico(); break;
        }
    }

    private IEnumerator MostrarLinea(LineRenderer lr, Vector3 inicio, Vector3 fin)
    {
        estaDisparando = true;
        if (lr)
        {
            lr.enabled = true;
            // Forzamos el uso de posiciones locales si el LineRenderer es hijo del arma
            lr.SetPosition(0, inicio);
            lr.SetPosition(1, fin);
        }
        yield return new WaitForSeconds(duracionLineaVisual);
        if (lr) lr.enabled = false;
        estaDisparando = false;
    }

    // Mantén CrearBala, DispararRifleRafaga y DispararEscopetaAbanico igual, ya funcionan bien.
    public void SetVisibilidadSprite(bool active) { if (sr) sr.enabled = active; }
}