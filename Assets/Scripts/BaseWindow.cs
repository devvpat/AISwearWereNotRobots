using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BaseWindow : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [SerializeField] protected bool isDraggable = true;
    [SerializeField] protected bool isConfined = true;

    [Header("Portrait Only Settings")]
    [SerializeField] protected bool isPortrait = false;
    [SerializeField] protected DragAndDropItem.WordType pointType = DragAndDropItem.WordType.Social;
    [SerializeField] protected Sprite[] levelSprites = new Sprite[3];
    [SerializeField] protected Image portraitImage;

    protected RectTransform rectTransform;

    protected virtual void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;

        // set the window to the top of the hierarchy
        rectTransform.SetAsLastSibling();
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;

        // calculate new position
        Vector2 newPosition = rectTransform.anchoredPosition + eventData.delta / Globals.CanvasScaleFactor;

        if (isConfined)
        {
            // Reference to canvas RectTransform
            RectTransform canvasRect = Globals.CanvasRectTransform;
            if (canvasRect == null) return;

            // Convert new anchoredPosition to world position
            Vector3 worldPos = rectTransform.parent.TransformPoint(newPosition);

            // Convert to canvas local space
            Vector3 canvasLocalPos = canvasRect.InverseTransformPoint(worldPos);

            // Get half sizes for clamping
            Vector2 halfCanvas = canvasRect.rect.size / 2f;
            Vector2 halfWindow = (rectTransform.rect.size * rectTransform.lossyScale) / 2f;

            // Clamp inside canvas bounds
            canvasLocalPos.x = Mathf.Clamp(canvasLocalPos.x, -halfCanvas.x + halfWindow.x, halfCanvas.x - halfWindow.x);
            canvasLocalPos.y = Mathf.Clamp(canvasLocalPos.y, -halfCanvas.y + halfWindow.y, halfCanvas.y - halfWindow.y);

            // Convert back to world position and then to local position relative to parent
            Vector3 clampedWorldPos = canvasRect.TransformPoint(canvasLocalPos);
            newPosition = rectTransform.parent.InverseTransformPoint(clampedWorldPos);

            // // calculate bounds
            // float maxXAbs = (Globals.CanvasWidth - rectTransform.rect.width) / 2;
            // float maxYAbs = (Globals.CanvasHeight - rectTransform.rect.height) / 2;

            // // clamp position to bounds
            // newPosition.x = Mathf.Clamp(newPosition.x, -maxXAbs, maxXAbs);
            // newPosition.y = Mathf.Clamp(newPosition.y, -maxYAbs, maxYAbs);
        }

        // set new position
        rectTransform.anchoredPosition = newPosition;
    }

    public void DestroySelfGameObject()
    {
        Destroy(gameObject);
    }

    public void DeactivateSelfGameObject()
    {
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        if (isPortrait)
        {
            int level = pointType == DragAndDropItem.WordType.Social ? GameManager.Instance.GetSocialPoints() : GameManager.Instance.GetAcademicPoints();
            portraitImage.sprite = levelSprites[level];
        }
    }
}
