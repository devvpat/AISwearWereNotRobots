using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDropItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform parentAfterDrag;

    public enum WordType
    {
        Social,
        Academic,
    }

    public WordType wordType;

    [SerializeField] private bool generateRandomWord = true;

    private Image image;
    private TMP_Text wordText;

    private void Start()
    {
        image = GetComponent<Image>();
        wordText = transform.GetChild(0).GetComponent<TMP_Text>();

        if (generateRandomWord) 
        {
            string[] words = wordType == WordType.Social ? Globals.SocialWords : Globals.AcademicWords;
            int randomIndex = UnityEngine.Random.Range(0, words.Length);
            wordText.text = words[randomIndex];
        }     
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin Drag");
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging");
        // Convert screen position to world position
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Set z-axis to match the object's current z position (to avoid moving it on the z-axis)
        worldPosition.z = transform.position.z;

        // Update the object's position to the new world position
        transform.position = worldPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End Drag");
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
    }
}
