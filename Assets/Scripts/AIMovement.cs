using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class AIMovement : MonoBehaviour
{
    public Animator animator;
    private NavMeshAgent agent;

    void Start()
    {
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