using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RobotController : MonoBehaviour
{
    Vector3 rot = new Vector3(0f, 180f, 0f); // Initial rotation
    float rotateDuration = 2f; // Time taken to rotate
    Animator anim;
    Quaternion targetRotation;
    bool isRotating = false;
    bool isWalking = false;
    float walkDistance = 5f; // Distance to walk in one press
    float walkSpeed = 2f; // Speed of walking
    List<System.Action> actionsQueue = new List<System.Action>(); // Queue to store actions
    Vector3 initialPosition = new Vector3(-13f, 14.04f, -25f); // Initial position

    void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
        gameObject.transform.eulerAngles = rot;
        targetRotation = transform.rotation;
    }

    // Method to be called by the Rotate +90 button
    public void RotatePlus90()
    {
        actionsQueue.Add(() => Rotate(90));
    }

    // Method to be called by the Rotate -90 button
    public void RotateMinus90()
    {
        actionsQueue.Add(() => Rotate(-90));
    }

    // Method to be called by the Move Forward button
    public void AddMoveForward()
    {
        actionsQueue.Add(MoveForward);
    }

    // Method to be called by the Start button
    public void StartSequence()
    {
        StartCoroutine(ExecuteActionsQueue());
    }

    void Rotate(int angle)
    {
        targetRotation *= Quaternion.Euler(Vector3.up * angle);
        StartCoroutine(RotateTowards(targetRotation));
    }

    IEnumerator RotateTowards(Quaternion targetRot)
    {
        isRotating = true;
        while (Quaternion.Angle(transform.rotation, targetRot) > 0.01f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rotateDuration);
            yield return null;
        }
        transform.rotation = targetRot;
        isRotating = false;
    }

    void MoveForward()
    {
        if (!isWalking)
        {
            StartCoroutine(Walk());
        }
    }

    IEnumerator ExecuteActionsQueue()
    {
        foreach (var action in actionsQueue)
        {
            action.Invoke();
            // Wait for the action to complete before moving to the next one
            while (isRotating || isWalking)
            {
                yield return null;
            }
        }
        actionsQueue.Clear(); // Clear actions after execution
    }

    IEnumerator Walk()
    {
        isWalking = true;
        anim.SetBool("Walk_Anim", true); // Start the walk animation
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + transform.forward * walkDistance;
        float startTime = Time.time;
        while (Time.time < startTime + (walkDistance / walkSpeed))
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, (Time.time - startTime) * walkSpeed / walkDistance);
            yield return null;
        }
        transform.position = endPosition;
        isWalking = false;
        anim.SetBool("Walk_Anim", false); // Stop the walk animation after reaching the end position
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player collides with a wall
        if (other.CompareTag("Wall"))
        {
            // Reiniciar el contador de monedas recogidas a cero
            CoinCollision.collectedCoins = 0;

            // Volver a cargar la escena actual
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            Debug.Log("Player collided with the wall and returned to the initial position.");
        }
    }
}
