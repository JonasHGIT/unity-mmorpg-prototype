/*
 * CraftingSlot.cs
 *
 * Author: Jonas Hammer
 * Description: Verhält sich als Ziel für Items im Crafting-System. Ermöglicht das Droppen,
 *              Setzen, Tauschen und Entfernen von Items innerhalb eines Crafting-Slots.
 *              Triggert CraftingManager bei Änderungen.
 * Last Edited: 16. April 2025
 *
 * Interfaces:
 * - IDropHandler: Empfängt Drag-and-Drop-Events vom Unity EventSystem.
 */

using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingSlot : MonoBehaviour, IDropHandler
{
    private InventoryItem currentItem;

    /// <summary>
    /// Wird aufgerufen, wenn ein Item auf diesen Slot gedroppt wird.
    /// </summary>
    public void OnDrop(PointerEventData eventData)
    {
        InventoryItem droppedItem = eventData.pointerDrag.GetComponent<InventoryItem>();

        if (droppedItem != null)
        {
            if (currentItem == null)
            {
                // Slot ist leer → Item setzen
                SetItem(droppedItem);
            }
            else
            {
                // Slot belegt → Items tauschen
                SwapItems(droppedItem);
            }

            // Informiere CraftingManager über Änderung
            FindObjectOfType<CraftingManager>().OnCraftingSlotChanged();
        }
    }

    /// <summary>
    /// Gibt das GameItem des aktuell gesetzten InventoryItems zurück.
    /// </summary>
    public GameItem GetItem()
    {
        return currentItem?.item;
    }

    /// <summary>
    /// Setzt ein neues InventoryItem in den Slot.
    /// </summary>
    public void SetItem(InventoryItem item)
    {
        currentItem = item;
        currentItem.transform.SetParent(transform);
        currentItem.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// Entfernt das Item aus dem Slot (z.B. bei Clear-Button oder Craft-Erfolg).
    /// </summary>
    public void ClearSlot()
    {
        if (currentItem != null)
        {
            Destroy(currentItem.gameObject);
            currentItem = null;
        }
    }

    /// <summary>
    /// Tauscht das aktuelle Item mit einem neuen gedroppten Item aus.
    /// </summary>
    private void SwapItems(InventoryItem newItem)
    {
        // Altes Item zurück an seinen ursprünglichen Platz
        Transform originalParent = newItem.originalParent;
        currentItem.transform.SetParent(originalParent);
        currentItem.transform.localPosition = Vector3.zero;

        // Neues Item in diesen Slot setzen
        SetItem(newItem);
    }
}
