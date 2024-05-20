using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractivePlayerHouse : MonoBehaviour
{
    private UIManager uiManager;

    // Referencia al nombre de la escena de personalización
    [SerializeField] private string sceneName;
    [SerializeField] private Vector3 playerPosition;
    [SerializeField] private Quaternion playerRotation;



    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el objeto que colisionó es el jugador
        if (other.CompareTag("Player"))
        {
            // Verificar si se ha configurado un UIManager
            if (uiManager == null)
            {
                // Intentar encontrar el UIManager en la escena
                uiManager = FindObjectOfType<UIManager>();

                // Si no se encuentra, mostrar un mensaje de advertencia
                if (uiManager == null)
                {
                    Debug.LogWarning("No se encontró el UIManager en la escena.");
                    return;
                }
            }

            // Cambiar a la escena de personalización
            if (!string.IsNullOrEmpty(sceneName))
            {
                other.transform.position = playerPosition;
                other.transform.rotation = playerRotation;
                // Activar el panel de personalización (si es necesario)
                // Cambiar a la escena de personalización
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogWarning("No se encontro el nombre de la escena.");
            }
        }
    }
}
