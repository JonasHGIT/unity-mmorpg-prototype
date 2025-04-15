using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HotkeySlot : MonoBehaviour, IDropHandler
{
    public Image image;
    public Color selectedColor, notSelectedColor;

    private void Awake()
    {
        Deselect();
    }

    public void Select()
    {
        image.color = selectedColor;
    }

    public void Deselect()
    {
        image.color = notSelectedColor;
    }

    // Drag and drop
    public void OnDrop(PointerEventData eventData)
    {
        HotkeySkill droppedSkill = eventData.pointerDrag.GetComponent<HotkeySkill>();

        if (transform.childCount == 0)
        {
            // Der Slot ist leer, daher einfach das Item hinzufügen
            droppedSkill.parentAfterDrag = transform;
        }
        else
        {
            // Es gibt bereits ein Item in diesem Slot
            Transform skillInThisSlot = transform.GetChild(0);
            HotkeySlot originalSlot = droppedSkill.parentAfterDrag.GetComponent<HotkeySlot>();

            // Das Item, das sich bereits im Slot befindet, in den ursprünglichen Slot des gezogenen Items verschieben
            skillInThisSlot.SetParent(originalSlot.transform);
            skillInThisSlot.position = originalSlot.transform.position;

            // Das gezogene Item in den aktuellen Slot verschieben
            droppedSkill.parentAfterDrag = transform;
        }
    }
}
