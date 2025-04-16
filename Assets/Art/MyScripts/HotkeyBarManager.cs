/*
 * ------------------------------------------------------------------------------
 * Script:       HotkeyBarManager.cs
 * Author:       Jonas Hammer
 * Created:      [Erstellungsdatum]
 * Last Edited:  16. April 2025
 * Description:  Dieses Skript verwaltet die Hotkey-Leiste im UI, mit der Skills
 *               Spielern per Tastendruck oder Mausklick zugewiesen und ausgeführt
 *               werden können. Es ermöglicht dynamisches Hinzufügen, Auswählen,
 *               Ausführen und Tauschen von Fähigkeiten in den Slots.
 *
 * Hauptfunktionen:
 * - Auswahl von Hotkey-Slots per Nummerntasten (1-5) oder Maustasten (LMB, RMB)
 * - Ausführung des Skills im aktuell ausgewählten Slot
 * - Dynamisches Hinzufügen neuer Skills zu freien Slots
 * - Skill-Tausch (Swap) zwischen dem letzten Slot und dem Rechtsklick-Slot via Tab
 * - Visuelle Rückmeldung bei Slot-Auswahl (via Select/Deselect)
 *
 * Unterstützt:
 * - Manuelles Setzen von Mausklick-Slots (z. B. Slot 5 = LMB, Slot 6 = RMB)
 * - Skill-Initialisierung über Prefabs
 * - Flexible Erweiterung der Leiste durch HotkeySlot-Array
 *
 * Dependencies:
 * - HotkeySlot.cs
 * - HotkeySkill.cs
 * - Skill.cs
 *
 * Hinweise:
 * - HotkeySlots und SkillPrefab müssen im Unity Inspector zugewiesen werden
 * - Skill-Ausführung wird durch `ExecuteSkill()` in HotkeySkill.cs gesteuert
 * - Visuelles Feedback wird über `Select()` / `Deselect()` der Slots gegeben
 * ------------------------------------------------------------------------------
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotkeyBarManager : MonoBehaviour
{
    public HotkeySlot[] hotkeySlots;
    public GameObject hotkeySkillPrefab;

    int selectedSlot = -1;
    int leftClickSlot = -1;   // Slot für Linksklick
    int rightClickSlot = -1;  // Slot für Rechtsklick

    private void Start()
    {
        SetMouseClickSlots(5, 6); // Beispiel: Slot 5 für Linksklick, Slot 6 für Rechtsklick
        //ChangeSelectedSlot(0);
    }

    private void Update()
    {
        // Überprüfe, ob eine Nummerntaste gedrückt wurde
        if (Input.inputString != null)
        {
            bool isNumber = int.TryParse(Input.inputString, out int number);
            if (isNumber && number > 0 && number <= 5)
            {
                ChangeSelectedSlot(number - 1);
            }
        }

        // Überprüfe, ob die linke Maustaste gedrückt wurde
        if (Input.GetMouseButtonDown(0))
        {
            ChangeSelectedSlot(leftClickSlot);
        }

        // Überprüfe, ob die rechte Maustaste gedrückt wurde
        if (Input.GetMouseButtonDown(1))
        {
            ChangeSelectedSlot(rightClickSlot);
        }

        // Überprüfe, ob die Tab-Taste gedrückt wurde
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwapSkillsBetweenSlots();
        }
    }

    void ChangeSelectedSlot(int newValue)
    {
        if (newValue < 0 || newValue >= hotkeySlots.Length) return; // Überprüfe, ob der Slot gültig ist

        if (selectedSlot >= 0)
        {
            hotkeySlots[selectedSlot].Deselect();
        }

        hotkeySlots[newValue].Select();
        selectedSlot = newValue;

        // Führe den Skill aus, der sich im neuen ausgewählten Slot befindet
        ExecuteSkillInSelectedSlot();
    }

    void ExecuteSkillInSelectedSlot()
    {
        HotkeySkill skillInSlot = hotkeySlots[selectedSlot].GetComponentInChildren<HotkeySkill>();
        if (skillInSlot != null)
        {
            skillInSlot.ExecuteSkill();
        }
    }

    public bool AddSkill(Skill skill)
    {
        for (int i = 0; i < hotkeySlots.Length; i++)
        {
            HotkeySlot slot = hotkeySlots[i];
            HotkeySkill skillInSlot = slot.GetComponentInChildren<HotkeySkill>();
        }

        for (int i = 0; i < hotkeySlots.Length; i++)
        {
            HotkeySlot slot = hotkeySlots[i];
            HotkeySkill skillInSlot = slot.GetComponentInChildren<HotkeySkill>();
            if (skillInSlot == null)
            {
                SpawnNewSkill(skill, slot);
                return true;
            }
        }

        return false;
    }

    void SpawnNewSkill(Skill skill, HotkeySlot slot)
    {
        GameObject newSkillGo = Instantiate(hotkeySkillPrefab, slot.transform);
        HotkeySkill hotkeySkill = newSkillGo.GetComponent<HotkeySkill>();
        hotkeySkill.InitialiseSkill(skill);
    }

    public void SetMouseClickSlots(int leftClickIndex, int rightClickIndex)
    {
        if (leftClickIndex >= 0 && leftClickIndex < hotkeySlots.Length)
        {
            leftClickSlot = leftClickIndex;
        }

        if (rightClickIndex >= 0 && rightClickIndex < hotkeySlots.Length)
        {
            rightClickSlot = rightClickIndex;
        }
    }

    // Methode zum Tauschen der Skills zwischen dem letzten und vorletzten Slot
    void SwapSkillsBetweenSlots()
    {
        if (rightClickSlot < 0 || rightClickSlot >= hotkeySlots.Length) return; // Überprüfen, ob der Rechtsklick-Slot gültig ist

        int lastSlotIndex = hotkeySlots.Length - 1;

        if (lastSlotIndex == rightClickSlot) return; // Wenn letzter Slot gleich dem Rechtsklick-Slot ist, nichts tun

        // Holen die Skills aus den Slots
        HotkeySkill lastSlotSkill = hotkeySlots[lastSlotIndex].GetComponentInChildren<HotkeySkill>();
        HotkeySkill rightClickSlotSkill = hotkeySlots[rightClickSlot].GetComponentInChildren<HotkeySkill>();

        // Swap der Skills
        if (lastSlotSkill != null && rightClickSlotSkill != null)
        {
            Skill tempSkill = lastSlotSkill.skill;
            lastSlotSkill.InitialiseSkill(rightClickSlotSkill.skill);
            rightClickSlotSkill.InitialiseSkill(tempSkill);
        }
        else if (lastSlotSkill != null && rightClickSlotSkill == null)
        {
            // Wenn der Rechtsklick-Slot leer ist, den Skill verschieben
            SpawnNewSkill(lastSlotSkill.skill, hotkeySlots[rightClickSlot]);
            Destroy(lastSlotSkill.gameObject);
        }
        else if (lastSlotSkill == null && rightClickSlotSkill != null)
        {
            // Wenn der letzte Slot leer ist, den Skill verschieben
            SpawnNewSkill(rightClickSlotSkill.skill, hotkeySlots[lastSlotIndex]);
            Destroy(rightClickSlotSkill.gameObject);
        }
    }
}
