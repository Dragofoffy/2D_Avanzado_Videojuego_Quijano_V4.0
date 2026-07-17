using UnityEngine;

public class InitialAlphaSetter : MonoBehaviour
{
    private void Start()
    {
        // Buscamos todos los objetos que tengan el tag "Limited"
        GameObject[] limitedItems = GameObject.FindGameObjectsWithTag("Limited");

        foreach (GameObject item in limitedItems)
        {
            // Obtenemos el componente SpriteRenderer
            SpriteRenderer sprite = item.GetComponent<SpriteRenderer>();

            if (sprite != null)
            {
                // Obtenemos el color actual
                Color color = sprite.color;

                // Cambiamos el valor de Alpha (a) a 0
                color.a = 0f;

                // Aplicamos el nuevo color
                sprite.color = color;
            }
        }
    }
}