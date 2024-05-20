using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using static RobotController2;

public class DropArea : MonoBehaviour, IDropHandler
{
    private Dictionary<string, ActionType> instructionMap = new Dictionary<string, ActionType>
    {
        { "IzqBtn", ActionType.RotateLeft },
        { "DerBtn", ActionType.RotateRight },
        { "AvanzarBtn", ActionType.MoveForward },
        { "IF1StartBtn", ActionType.IfWall },
        { "IF2StartBtn", ActionType.IfWalked },
        { "ElseBtn", ActionType.Else },
        { "EndIfBtn", ActionType.EndIf },
        // Add more mappings as necessary
    };

    public RobotController2 robotController;

    void Awake()
    {
        robotController = FindObjectOfType<RobotController2>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop called");

        GameObject draggedObject = eventData.pointerDrag;
        if (draggedObject == null)
        {
            Debug.LogError("Dragged object is null.");
            return;
        }
        DraggableInstruction draggableScript = draggedObject.GetComponent<DraggableInstruction>();
        if (draggableScript == null)
        {
            Debug.LogError("DraggableInstruction component is missing in the dragged object.");
            return;
        }

        if (draggableScript.itemPrefab != null)
        {
            Transform contentTransform = GetComponentInParent<ScrollRect>()?.content;

            if (contentTransform != null)
            {
                draggableScript.clone.transform.SetParent(contentTransform, false);
                draggableScript.clone.transform.localScale = Vector3.one;
                draggableScript.clone.transform.localPosition = Vector3.zero;
                draggableScript.clone.GetComponent<CanvasGroup>().blocksRaycasts = true;
                string cloneName = draggableScript.clone.name.Replace("(Clone)", "").Trim();

                if (instructionMap.TryGetValue(cloneName, out ActionType instructionType))
                {
                    robotController.AddInstruction(instructionType);
                }
                else
                {
                    Debug.LogError("No instruction mapping found for: " + cloneName);
                }

                Debug.Log("Cloned object added to the Content successfully.");
            }
            else
            {
                Debug.LogError("ScrollRect component not found in the parents.");
            }
        }
    }

    // Function to delete all clones inside the DropArea
    public void DeleteClones()
    {
        // Find the Content object within the ScrollRect's viewport
        Transform contentTransform = GetComponentInParent<ScrollRect>()?.content;
        if (contentTransform == null)
        {
            Debug.LogError("Content object not found in parents of the DropArea.");
            return;
        }

        // Detach and destroy only the clones (children of the Content)
        foreach (Transform child in contentTransform)
        {
            DraggableInstruction draggableInstruction = child.GetComponent<DraggableInstruction>();
            if (draggableInstruction != null)
            {
                Destroy(child.gameObject);
            }
        }
    }







}
