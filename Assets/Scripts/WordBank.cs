using UnityEngine;

public class WordBank : BaseWindow
{
    [SerializeField] private GameObject slotHolder;
    [SerializeField] private GameObject defaultAcademicWordPrefab;
    [SerializeField] private GameObject defaultSocialWordPrefab;

    private int numTotalSlots;
    private int numUsedSlots;
    private int numSocialSlots;
    private int numAcademicSlots;

    protected override void Start()
    {
        base.Start();
        numTotalSlots = slotHolder.transform.childCount;
        numUsedSlots = 0;
        numSocialSlots = 0;
        numAcademicSlots = 0;
        Debug.Log("Word Bank: Initialized with " + numTotalSlots + " slots.");
    }

    public void AddWord(DragAndDropItem.WordType wordType)
    {
        Debug.Log("Word Bank: Added word of type " + wordType);
        numUsedSlots++;
        if (wordType == DragAndDropItem.WordType.Social)
        {
            numSocialSlots++;
        }
        else if (wordType == DragAndDropItem.WordType.Academic)
        {
            numAcademicSlots++;
        }
    }

    public bool TryAddWord(DragAndDropItem.WordType wordType)
    {
        if (numUsedSlots == numTotalSlots)
        {
            Debug.Log("Word Bank: No more slots available.");
            return false;
        }
        foreach (Transform slot in slotHolder.transform)
        {
            if (slot.childCount == 0)
            {
                GameObject wordPrefab = wordType == DragAndDropItem.WordType.Social ? defaultSocialWordPrefab : defaultAcademicWordPrefab;
                GameObject newWord = Instantiate(wordPrefab, slot);
                newWord.GetComponent<DragAndDropItem>().wordType = wordType;
                // Debug.Log($"Word Bank: Added {wordType} word prefab to slot {slot.name}");
                AddWord(wordType);
                break;
            }
        }
        return true;
    }

    public void RemoveWord(DragAndDropItem.WordType wordType)
    {
        Debug.Log("Word Bank: Removed word of type " + wordType);
        numUsedSlots--;
        if (wordType == DragAndDropItem.WordType.Social)
        {
            numSocialSlots--;
        }
        else if (wordType == DragAndDropItem.WordType.Academic)
        {
            numAcademicSlots--;
        }
    }

    public bool TryRemoveWord(DragAndDropItem.WordType wordType)
    {
        if (numUsedSlots == 0)
        {
            Debug.Log("Word Bank: No words to remove.");
            return false;
        }
        foreach (Transform slot in slotHolder.transform)
        {
            if (slot.childCount > 0)
            {
                DragAndDropItem item = slot.GetChild(0).GetComponent<DragAndDropItem>();
                if (item.wordType == wordType)
                {
                    Destroy(item.gameObject);
                    // Debug.Log($"Word Bank: Removed {wordType} word prefab from slot {slot.name}");
                    RemoveWord(wordType);
                    break;
                }
            }
        }
        return true;
    }

    public bool IsEmpty()
    {
        return numUsedSlots == 0;
    }
}
