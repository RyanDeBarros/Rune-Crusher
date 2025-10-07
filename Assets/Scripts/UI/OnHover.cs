using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OnHover : MonoBehaviour, IPointerEnterHandler
{
    public UnityEvent onHover;

    public void OnPointerEnter(PointerEventData _)
    {
        onHover?.Invoke();
    }
}
