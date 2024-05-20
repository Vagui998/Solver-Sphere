using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimiento : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                targetPosition = hit.point;

                Vector3 moveDirection = (targetPosition - transform.position).normalized;

                if (moveDirection != Vector3.zero)
                {
                    Quaternion newRotation = Quaternion.LookRotation(moveDirection);
                    Vector3 euler = newRotation.eulerAngles;
                    euler.x = 0f;
                    euler.z = 0f;
                    euler.y -= 90f;
                    transform.rotation = Quaternion.Euler(euler);
                }

                if (!isMoving)
                {
                    isMoving = true;
                    PlayWalkAnimation();
                }
            }
        }

        if (isMoving)
        {
            MoveTowardsTarget();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check for collision with the tree collider
        if (collision.gameObject.CompareTag("TagArbol")) // Replace "TreeTag" with your tree tag or identifier
        {
            // Stop or adjust character movement here
            // For example, if using Rigidbody for movement:
            // GetComponent<Rigidbody>().velocity = Vector3.zero;
            // Or stop the character's movement in your character movement script.
            Debug.Log("Collision with tree!");
        }
    }

    void MoveTowardsTarget()
    {
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isMoving = false;
            PlayIdleAnimation();
        }
    }

    void PlayWalkAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("Walk", true);
        }
    }

    void PlayIdleAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("Walk", false);
        }
    }
}
