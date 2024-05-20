using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollision : MonoBehaviour
{
    // Variable estática para llevar la cuenta de las monedas recolectadas
    public static int collectedCoins = 0;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Se ha recogido la moneda, por lo tanto la desactivamos
            gameObject.SetActive(false);

            collectedCoins++;
        }
    }
}
