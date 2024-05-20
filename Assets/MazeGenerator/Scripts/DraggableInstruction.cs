using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableInstruction : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject itemPrefab; // Asegúrate de asignar el prefab correcto aquí
    public GameObject clone;
    public Vector3 startPosition;

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = transform.position; // Guarda la posición inicial del objeto para poder regresar si es necesario.

        clone = Instantiate(itemPrefab, transform.position, Quaternion.identity, transform.parent);
        clone.GetComponent<CanvasGroup>().blocksRaycasts = false; // Desactivamos el bloqueo de raycast para permitir el evento drop en el clone.
        clone.transform.SetAsLastSibling(); // Asegúrate de que el clone se renderiza al frente.

        // Ocultamos el objeto original para que no interfiera visualmente.
        GetComponent<CanvasGroup>().alpha = 0.6f;
        GetComponent<CanvasGroup>().blocksRaycasts = false;

        Debug.Log("Clon creado para arrastrar");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (clone != null)
        {
            clone.transform.position = Input.mousePosition; // Mueve el clon, no el objeto original.
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (clone != null && clone.transform.parent == transform.parent)
        {
            // Si el clone no ha sido colocado en DropArea, lo destruimos.
            Destroy(clone);
        }

        // Restauramos la visibilidad y la interactividad del objeto original.
        GetComponent<CanvasGroup>().alpha = 1f;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
