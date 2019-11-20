using UnityEngine;
using UnityEngine.EventSystems;

public class Pickup : MonoBehaviour, IDragHandler
{
    public Gate.Type type;
    public string symbol;
    public void OnDrag(PointerEventData ped)
    {

    }
}
