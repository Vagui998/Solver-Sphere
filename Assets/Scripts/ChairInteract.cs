using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ChairInteract : MonoBehaviour
{
    public GameObject Panel; // Asigna tu panel de selección de niveles aquí desde el Inspector.

    private void OnTriggerEnter(Collider other)
    {
        // Asegúrate de que el personaje tiene un tag que puedas chequear, como "Player"
        if (other.CompareTag("Player"))
        {
            Panel.SetActive(true); // Mostrar el panel de selección de niveles
            // Opcionalmente, pausar el juego o desactivar el control del jugador aquí
        }
    }
}

