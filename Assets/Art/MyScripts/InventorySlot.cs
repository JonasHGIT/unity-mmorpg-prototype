using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public virtual void OnDrop(PointerEventData eventData)
    {
        InventoryItem droppedItem = eventData.pointerDrag.GetComponent<InventoryItem>();

        if (droppedItem != null)
        {
            // Prüfe, ob der Slot leer ist
            if (transform.childCount == 0)
            {
                // Setze das Item als Kind des neuen Slots
                droppedItem.transform.SetParent(transform);
                droppedItem.transform.localPosition = Vector3.zero;
            }
            else
            {
                // Wenn der Slot bereits ein Item enthält, tausche die Items
                InventoryItem existingItem = transform.GetChild(0).GetComponent<InventoryItem>();

                if (existingItem != null)
                {
                    // Tausche das existierende Item in den ursprünglichen Slot des gedroppten Items
                    existingItem.transform.SetParent(droppedItem.originalParent);
                    existingItem.transform.localPosition = Vector3.zero;

                    // Setze das gedropte Item in den neuen Slot
                    droppedItem.transform.SetParent(transform);
                    droppedItem.transform.localPosition = Vector3.zero;
                }
            }
        }
    }
}
