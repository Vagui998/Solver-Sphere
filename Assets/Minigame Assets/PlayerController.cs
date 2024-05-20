using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    private NavMeshAgent agent;

    private string aveVariable; // Para almacenar la variable del ave
    private bool tieneValor = false; // Indica si el jugador ya tiene un valor almacenado



    void Start()
    {
        GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint"); // Asegúrate de que el punto de spawn tenga este tag
    if (spawnPoint != null) {
        transform.position = spawnPoint.transform.position;
        transform.rotation = spawnPoint.transform.rotation;
    } else {
        Debug.Log("No se encontró el punto de spawn en la escena.");
    }
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Sample a position on the NavMesh closest to the hit point
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit navMeshHit, 10.0f, NavMesh.AllAreas))
                {
                    // Set the destination to the closest valid point on the NavMesh
                    agent.SetDestination(navMeshHit.position);
                }
            }
        }

        UpdateRotation();
        UpdateAnimation();
    }

    public void SetAveVariable(string variable)
    {
        if (!tieneValor) // Solo almacena la variable si el jugador no tiene una ya
        {
            aveVariable = variable;
            tieneValor = true; // Indica que ahora el jugador tiene un valor
            Debug.Log("Variable del ave almacenada en PlayerController: " + aveVariable);
        }
        else
        {
            Debug.Log("El jugador ya tiene una variable almacenada y no puede almacenar otra.");
        }
    }

    public bool TieneValor()
    {
        return tieneValor;
    }

    // Método para "usar" la variable y permitir al jugador recoger otra
    public void UsarVariable()
    {
        if (tieneValor)
        {
            // Aquí puedes añadir cualquier lógica específica que necesite ejecutarse
            // cuando el jugador "usa" su variable, como aplicar efectos o actualizar el estado del juego.

            Debug.Log("El jugador ha usado la variable: " + aveVariable);
            
            // Resetear el estado y la variable del jugador
            aveVariable = string.Empty;
            tieneValor = false;
        }
        else
        {
            Debug.Log("El jugador intentó usar una variable, pero no tenía ninguna.");
        }
    }
    public string ObtenerAveVariable()
    {
    return aveVariable;
    }


    void UpdateRotation()
    {
        // Check if the character is moving
        if (agent.velocity.magnitude > 0.1f)
        {
            // Calculate the direction to move
            Vector3 moveDirection = agent.velocity.normalized;

            // Calculate the rotation to face the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            // Apply rotation to align the character with the movement direction
            // Here we add or subtract 90 degrees based on the character's initial orientation
            transform.rotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y - 90f, 0f);
        }
    }

    void UpdateAnimation()
    {
        // Check if the character is moving
        bool isMoving = agent.velocity.magnitude > 0.1f;

        // Set the "Walk" parameter of the animator accordingly
        if (animator != null)
        {
            animator.SetBool("Walk", isMoving);
        }
    }
}
