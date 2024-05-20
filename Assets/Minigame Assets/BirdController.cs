using UnityEngine;
using UnityEngine.AI;
using System;
using Random = UnityEngine.Random; // Alias para Random de UnityEngine

public class BirdController : MonoBehaviour
{
    public static event Action<GameObject> OnBirdTouchedPlayer;

    private NavMeshAgent agent;
    private bool isMoving = false;
    private BirdSpawner birdSpawner; // Reference to the BirdSpawner

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Start moving the bird when the script is enabled
        StartMoving();
    }

    // Method to set the reference to the BirdSpawner
    public void SetSpawner(BirdSpawner spawner)
    {
        birdSpawner = spawner;
    }

    void StartMoving()
    {
        isMoving = true;
        MoveToRandomPosition();
    }

    void MoveToRandomPosition()
    {
        // Generate a random point on the NavMesh
        Vector3 randomPoint = RandomNavMeshPosition(10f);

        // Set the destination to the random point
        agent.SetDestination(randomPoint);
    }

    Vector3 RandomNavMeshPosition(float radius)
    {
        // Get a random point on the NavMesh within the specified radius
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas);
        return hit.position;
    }

    void Update()
    {
        if (isMoving && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            // Bird has reached the destination, choose a new random position
            MoveToRandomPosition();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

        if (playerController != null && !playerController.TieneValor())
        {
            Debug.Log("Colisión con el jugador, pero el jugador ya tiene un valor.");
            OnBirdTouchedPlayer?.Invoke(this.gameObject);
        }
        else
        {
            Debug.Log("El jugador ya tiene un valor y no puede interactuar con otra ave.");
        }
    }
}
