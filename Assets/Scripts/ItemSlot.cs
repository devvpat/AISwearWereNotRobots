using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private bool isWordBankSlot = true;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("DROPPED");
        if (transform.childCount == 0)
        {
            GameObject dropped = eventData.pointerDrag;
            if (dropped == null) return;
            DragAndDropItem item = dropped.GetComponent<DragAndDropItem>();
            if (item == null) return;
            bool prevParentIsWordBank = item.parentAfterDrag.GetComponent<ItemSlot>().isWordBankSlot;
            Debug.Log("DROP INFO:\nPrevious Parent word bank: " + prevParentIsWordBank + "\nCurrent Parent word bank: " + isWordBankSlot);
            // use key if wordbank slot and previous parent was not a wordbank slot
            if (isWordBankSlot && !prevParentIsWordBank && GameManager.Instance.TryUseKey())
            {
                item.parentAfterDrag = transform;
                // add word if target is wordbank slot and previous parent was not a wordbank slot
                if (isWordBankSlot && !prevParentIsWordBank)
                {
                    GameManager.Instance.wordBankComp.AddWord(item.wordType);
                }
            }
            // dont use key if target is not wordbank slot OR target is wordbank and previous parent was also wordbank
            else if (!isWordBankSlot || (isWordBankSlot && prevParentIsWordBank))
            {
                item.parentAfterDrag = transform;
                // remove word if target is not wordbank slot and previous parent was wordbank slot
                if (!isWordBankSlot && prevParentIsWordBank)
                {
                    GameManager.Instance.wordBankComp.RemoveWord(item.wordType);
                }
                // print message if no keys left
            }
            else
            {
                Debug.Log("Word Bank: No keys left to use.");
            }
        }
    }
}
