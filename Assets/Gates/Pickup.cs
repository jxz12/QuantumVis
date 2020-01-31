using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pickup : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Gate.Type type;
    public event Action<PointerEventData> OnDragged;
    public event Action<PointerEventData> OnDropped;

    Vector2 originalPos;
    public void OnDrag(PointerEventData ped)
    {
        OnDragged?.Invoke(ped);
    }
    public void OnBeginDrag(PointerEventData ped)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        originalPos = transform.position;
    }
    public void OnEndDrag(PointerEventData ped)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        transform.position = originalPos;
        OnDropped?.Invoke(ped);
    }
    public float Alpha {
        get { return GetComponent<CanvasGroup>().alpha; }
        set { GetComponent<CanvasGroup>().alpha = value; }
    }
}
