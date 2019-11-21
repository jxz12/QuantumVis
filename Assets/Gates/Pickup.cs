using UnityEngine;
using UnityEngine.EventSystems;

public class Pickup : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Gate.Type type;

    Vector2 originalPos;
    public void OnDrag(PointerEventData ped)
    {
        transform.position = ped.position;
    }
    public void OnBeginDrag(PointerEventData ped)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        GetComponent<CanvasGroup>().alpha = .7f;
        originalPos = transform.position;
    }
    public void OnEndDrag(PointerEventData ped)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        GetComponent<CanvasGroup>().alpha = 1;
        transform.position = originalPos;
    }
}
