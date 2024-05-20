using System.Collections;
using UnityEngine;
using UnityEngine.AI; // Importante para trabajar con NavMeshAgent
using UnityEngine.SceneManagement;

public class PlayerSpawn : MonoBehaviour
{
    public GameObject player;

    private void Start()
    {
        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("No se encontró el jugador en el Start.");
            }
        }

        DontDestroyOnLoad(gameObject); // Asegúrate de aplicar esto al objeto correcto
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        MovePlayerToSpawnPoint();
    }

    private void MovePlayerToSpawnPoint()
    {
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");
        if (spawnPoint != null && player != null)
        {
            // Desactiva el NavMeshAgent
            NavMeshAgent agent = player.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.enabled = false;
            }

            // Elimina el Rigidbody
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            bool wasKinematic = false;
            if (playerRb != null)
            {
                wasKinematic = playerRb.isKinematic;
                Destroy(playerRb);
            }

            // Mueve al jugador
            player.transform.position = spawnPoint.transform.position;
            player.transform.rotation = spawnPoint.transform.rotation;

            // Espera un frame y luego agrega y configura un nuevo Rigidbody
            StartCoroutine(ReAddRigidbody(player, wasKinematic));

            // Reactiva el NavMeshAgent
            if (agent != null)
            {
                StartCoroutine(ReEnableNavMeshAgent(agent));
            }
        }
        else
        {
            Debug.LogWarning("No se encontró un objeto con el tag 'SpawnPoint' o el jugador es null.");
        }
    }

    IEnumerator ReAddRigidbody(GameObject player, bool wasKinematic)
    {
        yield return null; // Espera un frame
        if (player != null)
        {
            Rigidbody rb = player.AddComponent<Rigidbody>();
            rb.isKinematic = wasKinematic;
            // Configura aquí cualquier otro estado o propiedad del Rigidbody que necesites
        }
    }

    IEnumerator ReEnableNavMeshAgent(NavMeshAgent agent)
    {
        yield return null; // Espera un frame
        if (agent != null)
        {
            agent.enabled = true;
            // Opcionalmente reinicia su ruta o estado aquí si es necesario
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
