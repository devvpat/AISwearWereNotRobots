using UnityEngine;
using UnityEngine.EventSystems;

public class BaseWindow : MonoBehaviour, IDragHandler
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private bool isDraggable = true;
    [SerializeField] private bool isConfined = true;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        if (canvas == null) {
            canvas = FindFirstObjectByType<Canvas>(); // try to find the canvas if not set
            if (canvas == null) {
                Debug.LogError("Canvas not set nor could it be found");
                return;
            }
        }

        // calculate new position
        Vector2 newPosition = rectTransform.anchoredPosition + eventData.delta / canvas.scaleFactor;

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
}
