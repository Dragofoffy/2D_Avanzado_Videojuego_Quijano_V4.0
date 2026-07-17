using UnityEngine;
using System.Collections;

public class OrbitController : MonoBehaviour
{
    [Header("Configuración de Giro")]
    public Transform[] orbitObjects; // Arrastra aquí tus dos objetos
    public float rotationSpeed = 100f;
    public float orbitRadius = 2f;

    [Header("Configuración de Tiempo")]
    public float activeDuration = 3f;
    public float inactiveDuration = 2f;

    private bool isOrbiting = true;
    private float angle;

    void Start()
    {
        StartCoroutine(ToggleOrbitRoutine());
    }

    void Update()
    {
        if (isOrbiting)
        {
            angle += rotationSpeed * Time.deltaTime * Mathf.Deg2Rad;

            for (int i = 0; i < orbitObjects.Length; i++)
            {
                // Desfase de 180 grados para que estén opuestos (forma de cruz/línea)
                float offset = i * Mathf.PI;
                float x = Mathf.Cos(angle + offset) * orbitRadius;
                float y = Mathf.Sin(angle + offset) * orbitRadius;

                orbitObjects[i].position = (Vector2)transform.position + new Vector2(x, y);
            }
        }
    }

    IEnumerator ToggleOrbitRoutine()
    {
        while (true)
        {
            // Activar
            isOrbiting = true;
            foreach (var obj in orbitObjects) obj.gameObject.SetActive(true);
            yield return new WaitForSeconds(activeDuration);

            // Desactivar
            isOrbiting = false;
            foreach (var obj in orbitObjects) obj.gameObject.SetActive(false);
            yield return new WaitForSeconds(inactiveDuration);
        }
    }
}