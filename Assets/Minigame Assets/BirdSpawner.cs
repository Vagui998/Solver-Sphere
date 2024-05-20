using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
    public GameObject birdPrefab; // Prefab to instantiate
    public Transform spawnPoint; // Point where the prefab will be spawned
    public int maxBirds = 3; // Maximum number of birds allowed
    public float spawnDelay = 5f; // Delay between each bird spawn

    private int currentBirds = 0; // Counter for current number of spawned birds

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnInitialBirds());
    }

    // Coroutine to spawn initial birds with delay
    IEnumerator SpawnInitialBirds()
    {
        for (int i = 0; i < maxBirds; i++)
        {
            SpawnBird();
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    // Method to spawn a single bird
    void SpawnBird()
    {
        Vector3 spawnPosition = new Vector3(spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z - 3f);
        GameObject newBird = Instantiate(birdPrefab, spawnPosition, Quaternion.identity);
        newBird.GetComponent<BirdController>().SetSpawner(this); // Set the BirdSpawner reference in BirdController
        currentBirds++;
    }

    // Method called by BirdController when a bird is destroyed
    public void BirdDestroyed()
    {
        currentBirds--;
        StartCoroutine(RespawnBirdAfterDelay());
    }

    // Coroutine to respawn a bird after a delay
    IEnumerator RespawnBirdAfterDelay()
    {
        yield return new WaitForSeconds(spawnDelay);
        if (currentBirds < maxBirds)
        {
            SpawnBird();
        }
    }
}
