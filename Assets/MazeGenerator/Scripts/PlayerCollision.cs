using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollison : MonoBehaviour
{
    public Vector3 initialPosition = new Vector3(-13f, 14.04f, -25f);

    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el jugador colisiona con una pared
        if (other.CompareTag("Wall"))
        {
            // Mueve al jugador de vuelta a la posición inicial
            transform.position = initialPosition;
            Debug.Log("El jugador ha chocado con la pared y ha vuelto a la posición inicial.");
        }
    }
}
