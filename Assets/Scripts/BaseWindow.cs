using UnityEngine;
using UnityEngine.EventSystems;

public class BaseWindow : MonoBehaviour, IDragHandler
{
    [SerializeField] protected bool isDraggable = true;
    [SerializeField] protected bool isConfined = true;

    protected RectTransform rectTransform;

    protected virtual void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;

        // calculate new position
        Vector2 newPosition = rectTransform.anchoredPosition + eventData.delta / Globals.CanvasScaleFactor;

        if (isConfined) {
            // calculate bounds
            float maxXAbs = (Globals.CanvasWidth - rectTransform.rect.width) / 2;
            float maxYAbs = (Globals.CanvasHeight - rectTransform.rect.height) / 2;

            // clamp position to bounds
            newPosition.x = Mathf.Clamp(newPosition.x, -maxXAbs, maxXAbs);
            newPosition.y = Mathf.Clamp(newPosition.y, -maxYAbs, maxYAbs);
        }

        // set new position
        rectTransform.anchoredPosition = newPosition;
    }

    public void DestroySelfGameObject()
    {
        Destroy(gameObject);
    }
}
