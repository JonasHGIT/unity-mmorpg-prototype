using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingSlot : MonoBehaviour, IDropHandler
{
    private InventoryItem currentItem;

    public void OnDrop(PointerEventData eventData)
    {
        InventoryItem droppedItem = eventData.pointerDrag.GetComponent<InventoryItem>();

        if (droppedItem != null)
        {
            // Überprüfe, ob der Slot leer ist
            if (currentItem == null)
            {
                // Setze das Item in den Slot
                SetItem(droppedItem);
            }
            else
            {
                // Tausche das aktuelle Item mit dem gedroppten Item
                SwapItems(droppedItem);
            }

            FindObjectOfType<CraftingManager>().OnCraftingSlotChanged();
        }
    }

    public GameItem GetItem()
    {
        return currentItem?.item;
    }

    public void SetItem(InventoryItem item)
    {
        currentItem = item;
        currentItem.transform.SetParent(transform);
        currentItem.transform.localPosition = Vector3.zero;
    }

    public void ClearSlot()
    {
        if (currentItem != null)
        {
            Destroy(currentItem.gameObject);
            currentItem = null;
        }
    }

    private void SwapItems(InventoryItem newItem)
    {
        Transform originalParent = newItem.originalParent;

        currentItem.transform.SetParent(originalParent);
        currentItem.transform.localPosition = Vector3.zero;

        SetItem(newItem);
    }
}
