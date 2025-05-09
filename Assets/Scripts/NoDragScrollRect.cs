using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NoDragScrollRect : ScrollRect
{
    public override void OnDrag(PointerEventData eventData)
    {
        // Do nothing to avoid dragging
    }
}
