using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinCounter2 : MonoBehaviour
{
    public TMP_Text coinText;
    private int totalCoins;
    public GameObject victoryPanel;

    void Start()
    {
        MazeSpawner2 mazeSpawner = FindObjectOfType<MazeSpawner2>(); // Encuentra el objeto MazeSpawner en la escena

        if (mazeSpawner != null)
        {
            UpdateCoinText();
        }
    }

    void Update()
    {
        MazeSpawner2 mazeSpawner = FindObjectOfType<MazeSpawner2>(); // Encuentra el objeto MazeSpawner en la escena
        if (mazeSpawner != null) 
        {
            totalCoins = mazeSpawner.GetTotalCoinsGenerated(); // Obtén la cantidad total de monedas generadas desde el MazeSpawner

            // Verificar si se han recolectado todas las monedas
            if (CoinCollision.collectedCoins == totalCoins)
            {
                Debug.Log("¡Juego completado!");
                if (victoryPanel != null) 
                {
                    Time.timeScale = 0;
                    victoryPanel.SetActive(true);                 
                }
            }
        }

        UpdateCoinText();
    }

    void UpdateCoinText()
    {
        coinText.text = string.Format("Monedas: {0}/{1}", CoinCollision.collectedCoins, totalCoins);
    }
}
