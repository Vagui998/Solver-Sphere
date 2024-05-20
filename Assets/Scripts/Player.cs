using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Referencia estática al jugador
    public static Player instance;

    // Escala y posición inicial del jugador
    public Vector3 escalaInicial = Vector3.one;
    public Vector3 posicionInicial = Vector3.zero;

    // Método que se llama al iniciar la escena
    void Start()
    {
        // Verificar si ya hay una instancia del jugador
        if (instance == null)
        {
            // Si no hay ninguna instancia, establecer esta como la instancia única
            instance = this;
            DontDestroyOnLoad(gameObject); // Conservar el objeto del jugador al cambiar de escena
        }
        else
        {
            // Si ya hay una instancia, destruir esta
            Destroy(gameObject);
        }
    }

    // Método para cambiar la escala y la posición del jugador
    public void UpdateEscaleAndPosition(Vector3 nuevaEscala, Vector3 nuevaPosicion)
    {
        transform.localScale = nuevaEscala;
        transform.position = nuevaPosicion;
    }
}