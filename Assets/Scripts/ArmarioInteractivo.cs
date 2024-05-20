using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArmarioInteractivo : MonoBehaviour
{
    private UIManager uiManager;

    // Referencia al nombre de la escena de personalizaci�n
    [SerializeField] private string sceneName;
    [SerializeField] private Vector3 guardarropaPosition;
    [SerializeField] private Quaternion guardarropaRotation;



    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el objeto que colision� es el jugador
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
                    Debug.LogWarning("No se encontr� el UIManager en la escena.");
                    return;
                }
            }
              
            // Cambiar a la escena de personalizaci�n
            if (!string.IsNullOrEmpty(sceneName))
            {
                other.transform.position = guardarropaPosition;
                other.transform.rotation = guardarropaRotation;
                // Activar el panel de personalizaci�n (si es necesario)
                uiManager.ActivateCustomizationPanel();

                // Cambiar a la escena de personalizaci�n
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogWarning("El nombre de la escena de personalizaci�n no est� configurado en el armario.");
            }
        }
    }
}