using UnityEngine;
using UnityEngine.UI; // Necesario para trabajar con elementos de UI
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject pauseMenu; // Referencia al panel de pausa
    public TMPro.TextMeshProUGUI textDisplay;
    public CSVReader csvReader; // Referencia al lector de CSV
    private GameObject lastTouchedBird;
    private List<string> valoresMostrados = new List<string>();

    public PlayerController playerController;

    private List<VariableEjemplo> datos; // Almacena las variables cargadas del CSV

    public BirdSpawner birdSpawner;

    void Start()
    {
        // Intenta encontrar el controlador del jugador al inicio
        // Asegúrate de que el jugador tenga el componente PlayerController y esté etiquetado correctamente
        playerController = FindObjectOfType<PlayerController>();

        if(playerController == null)
        {
            Debug.LogError("No se encontró el PlayerController en la escena.");
        }

        // ... cualquier otra lógica de inicialización ...
    }

    private void OnEnable()
    {
        BirdController.OnBirdTouchedPlayer += ShowRandomFact;
    }

    private void OnDisable()
    {
        BirdController.OnBirdTouchedPlayer -= ShowRandomFact;
    }

    void ShowRandomFact(GameObject bird)
    {
        // Asume que el archivo CSV está en Assets/Resources y se llama "data.csv"
        string path = Application.dataPath + "/Minigame Assets/variables.csv";
        var datos = csvReader.LeerCSV(path);
        if (datos != null && datos.Count > 0)
    {
        List<VariableEjemplo> opcionesValidas = new List<VariableEjemplo>(datos);
        opcionesValidas.RemoveAll(variable => valoresMostrados.Contains(variable.tipo)); // Asume que 'tipo' es lo que quieres mostrar

        if (opcionesValidas.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, opcionesValidas.Count);
            VariableEjemplo variableAleatoria = opcionesValidas[randomIndex];

            valoresMostrados.Add(variableAleatoria.tipo); // Añade el valor mostrado a la lista
            textDisplay.text = variableAleatoria.tipo; // Actualiza el texto
            lastTouchedBird = bird;
            pauseMenu.SetActive(true);
        }
        else
        {
           valoresMostrados.Clear();
            // Muestra de nuevo un valor aleatorio
            ShowRandomFact(bird);
        }
    }

        PauseGame();
    }

    public void NoTakeButtonPressed()
{
    // Ocultar el panel de pausa
    pauseMenu.SetActive(false);

    // Si tienes alguna lógica adicional para manejar el estado del jugador cuando rechaza el valor, colócala aquí
    // Por ejemplo, si el jugador ya tiene un valor asignado y quieres descartarlo:
     if (lastTouchedBird != null)
    {
        Destroy(lastTouchedBird);
        birdSpawner.BirdDestroyed(); // Avisa al spawner para que pueda manejar el conteo y el respawn de aves
       
    }

    pauseMenu.SetActive(false);
    Time.timeScale = 1;
}

     public void OnTakeButtonPressed()
    {
      
         if (lastTouchedBird != null)
    {
        Destroy(lastTouchedBird);
        birdSpawner.BirdDestroyed(); // Avisa al spawner para que pueda manejar el conteo y el respawn de aves
        if(playerController != null)
                playerController.SetAveVariable(textDisplay.text);
    }
        // Finalmente, ocultas el panel de pausa y resumes el juego
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    void PauseGame()
    {
        Time.timeScale = 0;
    }

    // Asegúrate de tener también un método para reanudar el juego
}
