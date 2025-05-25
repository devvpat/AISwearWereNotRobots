using UnityEngine;
using TMPro;

public class WordBank : BaseWindow
{
    [SerializeField] private GameObject slotHolder;
    [SerializeField] private GameObject defaultWordPrefab;
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
                CreateNewWord(wordType, slot);

                // Debug.Log($"Word Bank: Added {wordType} word prefab to slot {slot.name}");
                AddWord(wordType);
                break;
            }
        }
        return true;
    }

    public GameObject CreateNewWord(DragAndDropItem.WordType wordType, Transform slot, string wordText = null)
    {
        // instantiate the word prefab, attach it to the slot, and set its wordType
        GameObject newWord = Instantiate(defaultWordPrefab, slot);
        newWord.GetComponent<DragAndDropItem>().wordType = wordType;
        // randomly choose the text if wordText is null
        if (wordText != null)
        {
            newWord.transform.GetChild(0).GetComponent<TMP_Text>().text = wordText;
        }
        else
        {
            string[] words = wordType == DragAndDropItem.WordType.Social ? Globals.SocialWords : Globals.AcademicWords;
            int randomIndex = UnityEngine.Random.Range(0, words.Length);
            newWord.transform.GetChild(0).GetComponent<TMP_Text>().text = words[randomIndex];
        }
        return newWord;
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
