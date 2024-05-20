using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RobotController2 : MonoBehaviour
{
    Vector3 rot = new Vector3(0f, 180f, 0f); // Initial rotation
    float rotateDuration = 2f; // Time taken to rotate
    Animator anim;
    Quaternion targetRotation;
    bool isRotating = false;
    bool isWalking = false;
    float walkDistance = 5f; // Distance to walk in one press
    float walkSpeed = 2f; // Speed of walking
    Vector3 initialPosition = new Vector3(-13f, 14.04f, -25f); // Initial position

    bool[,] walkedTiles = new bool[4, 4];
    Vector2Int currentPosition = new Vector2Int(0, 0);


    List<System.Action> actionsQueue = new List<System.Action>(); // Queue to store actions

    public enum ActionType
    {
        MoveForward,
        RotateLeft,
        RotateRight,
        IfWall,
        IfWalked,
        Else,
        EndIf,
        // Add any other action types you need
    }

    public class ActionBlock
    {
        public ActionType actionType;
        public List<System.Action> actions = new List<System.Action>();
        public ActionBlock parentBlock; // Reference to the parent block for nested structure
        public ActionBlock elseBlock;
    }

    // This list will hold your main action sequence including conditionals
    List<ActionBlock> mainSequence = new List<ActionBlock>();
    ActionBlock currentBlock;

    Stack<ActionBlock> blockStack = new Stack<ActionBlock>();




    void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
        gameObject.transform.eulerAngles = rot;
        targetRotation = transform.rotation;

        ActionBlock rootBlock = new ActionBlock();
        blockStack.Push(rootBlock); // Start with a root block
        mainSequence.Add(rootBlock);
        currentBlock = rootBlock; // Ensure currentBlock is initialized to rootBlock
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
            MarkCurrentTileAsWalked();
        }
    }
    IEnumerator ExecuteActionsQueue()
    {
        while (true) // Continuously execute actions
        {
            foreach (var block in mainSequence)
            {
                // Start executing from the root blocks
                yield return StartCoroutine(ExecuteBlock(block));
            }
            yield return new WaitForSeconds(1); // Small delay to prevent rapid re-execution
        }
    }

    IEnumerator ExecuteBlock(ActionBlock block)
    {
        if (block.actionType == ActionType.IfWall || block.actionType == ActionType.IfWalked)
        {
            bool conditionMet = block.actionType == ActionType.IfWall ? IsCollisionAhead() : IsTileInFrontWalked();
            var actionsToExecute = conditionMet ? block.actions : block.elseBlock?.actions;
            if (actionsToExecute != null)
            {
                foreach (System.Action action in actionsToExecute)
                {
                    if (action.Method.Name == "StartCoroutine") // Detect nested block executions
                    {
                        yield return StartCoroutine((IEnumerator)action.DynamicInvoke());
                    }
                    else
                    {
                        action.Invoke();
                        while (isRotating || isWalking)
                            yield return null;
                    }
                }
            }
        }
        else
        {
            foreach (System.Action action in block.actions)
            {
                action.Invoke();
                while (isRotating || isWalking)
                    yield return null;
            }
        }

        if (block.elseBlock != null && !(block.actionType == ActionType.IfWall || block.actionType == ActionType.IfWalked))
        {
            yield return StartCoroutine(ExecuteBlock(block.elseBlock));
        }
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

    public void AddInstruction(ActionType actionType)
    {
        switch (actionType)
        {
            case ActionType.MoveForward:
            case ActionType.RotateRight:
            case ActionType.RotateLeft:
                currentBlock.actions.Add(actionType == ActionType.MoveForward ? MoveForward : () => Rotate(actionType == ActionType.RotateRight ? 90 : -90));
                break;
            case ActionType.IfWall:
            case ActionType.IfWalked:
                ActionBlock newBlock = new ActionBlock { actionType = actionType, parentBlock = currentBlock };
                currentBlock.actions.Add(() => StartCoroutine(ExecuteBlock(newBlock)));
                blockStack.Push(newBlock);
                currentBlock = newBlock;
                break;
            case ActionType.Else:
                currentBlock.elseBlock = new ActionBlock { parentBlock = currentBlock.parentBlock };
                blockStack.Push(currentBlock.elseBlock);
                currentBlock = currentBlock.elseBlock;
                break;
            case ActionType.EndIf:
                blockStack.Pop();
                currentBlock = blockStack.Peek();
                break;
        }
    }

    public void PlayButtonPressed()
    {
        StartSequence();
    }

    bool IsCollisionAhead()
    {
        float checkDistance = walkDistance;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, checkDistance))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                return true; // There's a wall ahead
            }
        }
        return false; // No wall detected
    }

    void MarkCurrentTileAsWalked()
    {
        // Set the current tile as walked
        walkedTiles[currentPosition.x, currentPosition.y] = true;

        // Determine the direction the robot is facing based on its Y-axis rotation
        // Normalize the angle to be within 0-360 range
        float yRotation = (transform.eulerAngles.y + 360f) % 360f;

        // Define directions based on Y-axis rotation
        // Correcting for the initial rotation of 180 degrees
        if ((yRotation >= 225f && yRotation < 315f) || (yRotation >= -135f && yRotation < -45f))
        {
            currentPosition.x += 1; // Facing right
        }
        else if ((yRotation >= 135f && yRotation < 225f) || (yRotation >= -225f && yRotation < -135f))
        {
            currentPosition.y += 1; // Facing down/backward
        }
        else if ((yRotation >= 45f && yRotation < 135f) || (yRotation >= -315f && yRotation < -225f))
        {
            currentPosition.x -= 1; // Facing left
        }
        else // Default case covers both 315 to 360 (-45 to 0) and 0 to 45 (-360 to -315) degrees
        {
            currentPosition.y -= 1; // Facing up/forward
        }

        // Clamp the values to ensure they remain within bounds
        currentPosition.x = Mathf.Clamp(currentPosition.x, 0, 3);
        currentPosition.y = Mathf.Clamp(currentPosition.y, 0, 3);
        Debug.Log("Position: " + currentPosition.x + " , " + currentPosition.y);
    }

    bool IsTileInFrontWalked()
    {
        // Determine the direction the robot is facing based on its Y-axis rotation
        // Normalize the angle to be within 0-360 range
        float yRotation = (transform.eulerAngles.y + 360f) % 360f;

        // Initialize potential new position based on current position
        Vector2Int newPosition = currentPosition;

        // Define directions based on Y-axis rotation
        // Correcting for the initial rotation of 180 degrees
        if ((yRotation >= 225f && yRotation < 315f) || (yRotation >= -135f && yRotation < -45f))
        {
            newPosition.x += 1; // Facing right
        }
        else if ((yRotation >= 135f && yRotation < 225f) || (yRotation >= -225f && yRotation < -135f))
        {
            newPosition.y += 1; // Facing down/backward
        }
        else if ((yRotation >= 45f && yRotation < 135f) || (yRotation >= -315f && yRotation < -225f))
        {
            newPosition.x -= 1; // Facing left
        }
        else // Default case covers both 315 to 360 (-45 to 0) and 0 to 45 (-360 to -315) degrees
        {
            newPosition.y -= 1; // Facing up/forward
        }

        // Clamp the values to ensure they remain within bounds
        newPosition.x = Mathf.Clamp(newPosition.x, 0, 3);
        newPosition.y = Mathf.Clamp(newPosition.y, 0, 3);

        // Return whether the tile in the determined position has been walked
        return walkedTiles[newPosition.x, newPosition.y];
    }

    public void ClearActionsQueue()
    {
        // Clear the action queue
        actionsQueue.Clear();

        // Clear the main sequence of action blocks if needed
        mainSequence.Clear();
        ActionBlock rootBlock = new ActionBlock();
        mainSequence.Add(rootBlock);
        currentBlock = rootBlock;
        blockStack.Clear();
        blockStack.Push(rootBlock); // Start with a new root block
    }



}