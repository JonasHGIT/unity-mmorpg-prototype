/*
 * ------------------------------------------------------------------------------
 * Script:       InventorySlot.cs
 * Author:       Jonas Hammer
 * Created:      [Erstellungsdatum]
 * Last Edited:  16. April 2025
 * Description:  Repräsentiert einen Slot im Inventarsystem. Unterstützt das
 *               Platzieren und Tauschen von Items via Drag & Drop.
 *
 * Funktionen:
 * - Prüft beim Drop, ob der Slot leer ist
 * - Unterstützt Item-Swap bei belegtem Slot
 * - Platziert das gedroppte Item korrekt im UI
 *
 * Dependencies:
 * - InventoryItem.cs
 *
 * Hinweise:
 * - Diese Basisklasse kann für spezialisierte Slots erweitert werden
 * ------------------------------------------------------------------------------
 */


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
