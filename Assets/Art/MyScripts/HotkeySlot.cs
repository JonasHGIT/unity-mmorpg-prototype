/*
 * HotkeySlot.cs
 * 
 * Author: Jonas Hammer
 * Description: Repräsentiert einen Hotkey-Slot im UI, der das Ziehen und Ablegen von Fähigkeiten unterstützt. Verwalten von ausgewählten und nicht ausgewählten Farben sowie Drag-and-Drop-Logik.
 * Last Edited: 16. April 2025
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;  // Notwendig für Drag-and-Drop-Funktionalität

// Diese Klasse ermöglicht es, einen Slot für Hotkeys zu verwalten, wobei auch das Ziehen und Ablegen von Fähigkeiten unterstützt wird.
public class HotkeySlot : MonoBehaviour, IDropHandler
{
    // Die UI-Komponente für das Bild im Hotkey-Slot
    public Image image;
    
    // Farben für ausgewählte und nicht ausgewählte Zustände
    public Color selectedColor, notSelectedColor;

    // Wird beim Start des Scripts aufgerufen
    private void Awake()
    {
        Deselect();  // Initialer Zustand des Slots ist "nicht ausgewählt"
    }

    // Ändert die Farbe des Slots, um den "ausgewählten" Zustand darzustellen
    public void Select()
    {
        image.color = selectedColor;  // Setzt die Farbe des Slots auf die "selectedColor"
    }

    // Ändert die Farbe des Slots zurück auf den "nicht ausgewählten" Zustand
    public void Deselect()
    {
        image.color = notSelectedColor;  // Setzt die Farbe des Slots auf die "notSelectedColor"
    }

    // Die OnDrop-Methode wird aufgerufen, wenn ein Element (Skill) auf den Slot abgelegt wird
    public void OnDrop(PointerEventData eventData)
    {
        // Holt die HotkeySkill-Komponente des gezogenen Objekts
        HotkeySkill droppedSkill = eventData.pointerDrag.GetComponent<HotkeySkill>();

        // Wenn der Slot leer ist (kein Kindobjekt)
        if (transform.childCount == 0)
        {
            // Der Slot ist leer, also fügen wir die Fähigkeit einfach hinzu
            droppedSkill.parentAfterDrag = transform;  // Setzt den Elternteil des Skills auf diesen Slot
        }
        else
        {
            // Es gibt bereits eine Fähigkeit im Slot, daher muss das Original zurückgesetzt werden
            Transform skillInThisSlot = transform.GetChild(0);  // Das bestehende Element im Slot
            HotkeySlot originalSlot = droppedSkill.parentAfterDrag.GetComponent<HotkeySlot>();  // Der ursprüngliche Slot des gezogenen Skills

            // Verschiebe das vorhandene Item zurück in den ursprünglichen Slot des gezogenen Items
            skillInThisSlot.SetParent(originalSlot.transform);
            skillInThisSlot.position = originalSlot.transform.position;

            // Setze den gezogenen Skill in den aktuellen Slot
            droppedSkill.parentAfterDrag = transform;
        }
    }
}
