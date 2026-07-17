using UnityEngine;
using TMPro;
using System.Collections;

public class DisableTextAfterTime : MonoBehaviour
{
    // Arrastra aquí el componente de TextMeshPro desde el Inspector
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private float delay = 15f;

    void Start()
    {
        // Iniciamos la corrutina
        StartCoroutine(DisableAfterDelay());
    }

    IEnumerator DisableAfterDelay()
    {
        // Espera los segundos definidos
        yield return new WaitForSeconds(delay);

        // Desactiva el componente (o el objeto entero si prefieres)
        if (textComponent != null)
        {
            textComponent.gameObject.SetActive(false);
        }
    }
}