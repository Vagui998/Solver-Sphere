using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random; 

public class Establo : MonoBehaviour
{
    public TextMeshPro textoDelCartel; 
    public CSVReader csvReader; 

    public TMPro.TextMeshProUGUI contadorDeCoincidenciasTexto;
    public GameObject panelFinalizacion;

    private List<string> valoresMostrados = new List<string>(); 
    private List<VariableEjemplo> listaVariables; 

    private int contadorDeCoincidencias = 0;
    private const int UmbralDeCoincidencias = 10;

    public Transform spawnPoint;
    void Start()
    {
        contadorDeCoincidenciasTexto.text = "Coincidencias: 0";

        if (csvReader == null)
        {
            Debug.LogError("CSVReader no asignado en el Inspector.");
            return;
        }

        string path = Application.dataPath + "/Minigame Assets/variables.csv";
        listaVariables = csvReader.LeerCSV(path);

        MostrarNuevoValor();
    }

    public void SacudirCamara()
    {
        StartCoroutine(CamaraSacudida(0.5f, 0.4f));
    }

    IEnumerator CamaraSacudida(float duracion, float magnitud)
    {
        Vector3 posicionOriginal = Camera.main.transform.localPosition;
        float tiempoTranscurrido = 0.0f;

        while (tiempoTranscurrido < duracion)
        {
            float x = Random.Range(-1f, 1f) * magnitud;
            float y = Random.Range(-1f, 1f) * magnitud;
            Camera.main.transform.localPosition = new Vector3(x, y, posicionOriginal.z);

            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        Camera.main.transform.localPosition = posicionOriginal;

        
         if (spawnPoint!= null)
          {
            PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
          {
            player.transform.position = spawnPoint.position; 
            Debug.Log("Jugador movido al punto de respawn");
          }
          }
    }

    private void MostrarNuevoValor()
{
    if (listaVariables != null && listaVariables.Count > 0)
    {
        List<VariableEjemplo> opcionesValidas = new List<VariableEjemplo>(listaVariables);
        opcionesValidas.RemoveAll(variable => valoresMostrados.Contains(variable.ejemplo));

        if (opcionesValidas.Count > 0)
        {
            int indiceAleatorio = Random.Range(0, opcionesValidas.Count);
            VariableEjemplo variableAleatoria = opcionesValidas[indiceAleatorio];

            valoresMostrados.Add(variableAleatoria.ejemplo);
            textoDelCartel.text = variableAleatoria.ejemplo;
        }
        else
        {
            valoresMostrados.Clear();
            MostrarNuevoValor(); 
        }
    }
    else
    {
        Debug.LogError("La lista de variables está vacía o no se pudo cargar.");
    }
}

private void FinalizarJuego()
    {
        panelFinalizacion.SetActive(true); 
        Time.timeScale = 0; 
        Debug.Log("Juego finalizado: se alcanzaron 10 coincidencias.");

    }

   private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController != null && playerController.TieneValor())
        {
            string valorJugador = playerController.ObtenerAveVariable().Trim();
            string valorEstablo = textoDelCartel.text.Trim();

            Debug.Log($"El jugador lleva: '{valorJugador}' y el establo muestra: '{valorEstablo}'");

            bool coincidenciaEncontrada = false;
            foreach (VariableEjemplo variable in listaVariables)
            {
                Debug.Log($"Buscando coincidencia: establo ('{variable.tipo}') vs jugador ('{variable.ejemplo}')");

                if (string.Equals(variable.ejemplo.Trim(), valorEstablo, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(variable.tipo.Trim(), valorJugador, StringComparison.OrdinalIgnoreCase))
                {
                    coincidenciaEncontrada = true;
                    Debug.Log("Coincidencia encontrada. El jugador puede entregar el valor.");
                    contadorDeCoincidencias++;
                    int coincidenciasRestantes = UmbralDeCoincidencias - contadorDeCoincidencias;
                    contadorDeCoincidenciasTexto.text = "Coincidencias: " + contadorDeCoincidencias + "/" + UmbralDeCoincidencias;
                     if (contadorDeCoincidencias >= UmbralDeCoincidencias)
                    {
                      FinalizarJuego();
                      Debug.Log("Umbral de coincidencias alcanzado.");
                      return;
                    }
                    
                    playerController.UsarVariable();
                    MostrarNuevoValor();
                    break;
                }
               
            }

            if (!coincidenciaEncontrada)
            {
                Debug.Log("No se encontró una coincidencia. El jugador no puede entregar el valor.");
                playerController.UsarVariable();
                SacudirCamara();
            }
         MostrarNuevoValor(); 

        }
    }
}

}




