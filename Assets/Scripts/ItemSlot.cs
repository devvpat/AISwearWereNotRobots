using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("DROPPED");
        if (transform.childCount == 0)
        {
            GameObject dropped = eventData.pointerDrag;
            if (dropped == null) return;
            DragAndDropItem item = dropped.GetComponent<DragAndDropItem>();
            if (item != null ) item.parentAfterDrag = transform;
        }
    }
}
