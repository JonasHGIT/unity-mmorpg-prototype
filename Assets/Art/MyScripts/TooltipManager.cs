/*
 * ------------------------------------------------------------------------------
 * Script:       TooltipManager.cs
 * Author:       Jonas Hammer
 * Created:      [Erstellungsdatum]
 * Last Edited:  16. April 2025
 * Description:  Dieses Skript verwaltet das Anzeigen und Verwalten von Tooltipps, die beim Überfahren von Items im Inventar angezeigt werden.
 *               Es zeigt detaillierte Informationen zu einem Item, einschließlich seiner Basiswerte und eventuellen Verzauberungen, und vergleicht diese mit einem ausgerüsteten Item, falls vorhanden.
 * 
 * Hauptfunktionen:
 * - Zeigt detaillierte Tooltips an, wenn ein Item im Inventar oder der Ausrüstungsansicht überfahren wird.
 * - Vergleicht die Werte des Items mit einem möglicherweise ausgerüsteten Item und zeigt Differenzen an.
 * - Unterstützt das Anzeigen von Verzauberungen und deren Werten.
 * - Passt die Schriftgröße der Tooltipps dynamisch basierend auf der Anzahl relevanter Werte an, um eine optimale Darstellung zu gewährleisten.
 * 
 * UI-Elemente:
 * - `tooltip`: Das Haupttooltip, das angezeigt wird.
 * - `tooltipDeco`: Ein dekoratives Element, das das Tooltip visuell unterstützt.
 * - `tooltipText`: Der Textbereich für die Basiswerte des Items.
 * - `enchantmentsText`: Der Textbereich für Verzauberungen des Items.
 * - `sellValueText`: Der Textbereich für den Verkaufswert des Items.
 * - `tooltipImage`: Das Bild des Items im Tooltip.
 *
 * Abhängigkeiten:
 * - `GameItem`: Das Item-Objekt, das die Daten des Items enthält, das im Tooltip angezeigt wird.
 * - `InventoryManager`: Ein Verwalter des Inventars, der den Zugriff auf ausgerüstete Items und Slots ermöglicht.
 * - `EquipSlot`: Das Slot-Objekt in der Ausrüstungsansicht, in dem ausgerüstete Items platziert werden.
 *
 * Ereignis-Handling:
 * - Zeigt das Tooltip für das ausgewählte Item an, wenn dieses überfahren wird.
 * - Versteckt das Tooltip, wenn die Maus das Item verlässt.
 * - Berechnet und zeigt Unterschiede zwischen dem aktuellen Item und dem ausgerüsteten Item an.
 *
 * Wichtige Hinweise:
 * - Die Schriftgröße des Tooltips wird automatisch angepasst, basierend auf der Anzahl der relevanten Werte, um eine klare Darstellung zu gewährleisten.
 * - Das System vergleicht die Werte eines Items mit einem ausgerüsteten Item, um Unterschiede anzuzeigen, und berücksichtigt Verzauberungen sowie deren Änderungen.
 * ------------------------------------------------------------------------------
 */


using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    public GameObject tooltip;
    public GameObject tooltipDeco;
    public TextMeshProUGUI tooltipText;
    public TextMeshProUGUI enchantmentsText;
    public TMP_Text sellValueText;
    public Image tooltipImage;

    // Show the tooltip for the hovered item
    public void ShowTooltip(GameItem item)
    {
        if (tooltip != null && item != null)
        {
            tooltip.SetActive(true);
            tooltipDeco.SetActive(true);

            // Find an equipped item of the same type
            GameItem equippedItem = GetEquippedItemOfSameType(item.type);

            // Update the tooltip text with comparison if applicable
            if (equippedItem != null && equippedItem != item)
            {
                tooltipText.text = GetItemBaseStats(item, equippedItem);
            }
            else
            {
                tooltipText.text = GetItemBaseStats(item, null);
            }

            enchantmentsText.text = GetItemEnchantments(item, equippedItem);
            tooltipImage.sprite = item.image;

            // Count the number of relevant values to adjust the font size
            int relevantValuesCount = CountRelevantValues(item, equippedItem);
            AdjustFontSize(relevantValuesCount);
        }
    }

    // Hide the tooltip
    public void HideTooltip()
    {
        if (tooltip != null && tooltipDeco != null)
        {
            tooltip.SetActive(false);
            tooltipDeco.SetActive(false);
        }
    }

    // Find an equipped item of the same type
    private GameItem GetEquippedItemOfSameType(GameItemType itemType)
    {
        foreach (EquipSlot equipSlot in FindObjectOfType<InventoryManager>().equipSlots)
        {
            InventoryItem equippedItem = equipSlot.GetComponentInChildren<InventoryItem>();
            if (equippedItem != null && equippedItem.item.type == itemType)
            {
                return equippedItem.item;
            }
        }
        return null; // No item of the same type is equipped
    }

    // Generate the base stats text for the item, optionally with comparison
    private string GetItemBaseStats(GameItem item, GameItem equippedItem)
    {
        List<string> baseStats = new List<string>();

        if (equippedItem != null)
        {
            // Vergleiche mit ausgerüstetem Item
            if (item.inventoryItemLevel != 0 || equippedItem.inventoryItemLevel != 0)
                baseStats.Add(CompareStat("Item Level", item.inventoryItemLevel, equippedItem.inventoryItemLevel, item));
            if (item.health != 0 || equippedItem.health != 0)
                baseStats.Add(CompareStat("Health", item.health, equippedItem.health, item));
            if (item.mana != 0 || equippedItem.mana != 0)
                baseStats.Add(CompareStat("Mana", item.mana, equippedItem.mana, item));
            if (item.minDamage != 0 || equippedItem.minDamage != 0)
                baseStats.Add(CompareStat("Min Damage", item.minDamage, equippedItem.minDamage, item));
            if (item.maxDamage != 0 || equippedItem.maxDamage != 0)
                baseStats.Add(CompareStat("Max Damage", item.maxDamage, equippedItem.maxDamage, item));
            if (item.attackSpeed != 0 || equippedItem.attackSpeed != 0)
                baseStats.Add(CompareStat("Attack Speed", item.attackSpeed, equippedItem.attackSpeed, item));
            if (item.armorValue != 0 || equippedItem.armorValue != 0)
                baseStats.Add(CompareStat("Armor Value", item.armorValue, equippedItem.armorValue, item));
            if (item.critChance != 0 || equippedItem.critChance != 0)
                baseStats.Add(CompareStat("Crit Chance", item.critChance, equippedItem.critChance, item));
            if (item.critDamageBonus != 0 || equippedItem.critDamageBonus != 0)
                baseStats.Add(CompareStat("Crit Damage Bonus", item.critDamageBonus, equippedItem.critDamageBonus, item));
            if (item.moveSpeed != 0 || equippedItem.moveSpeed != 0)
                baseStats.Add(CompareStat("Move Speed", item.moveSpeed, equippedItem.moveSpeed, item));
        }
        else
        {
            // Vergleich mit Standardwert 0, wenn kein Gegenstand ausgerüstet ist
            if (item.inventoryItemLevel != 0)
                baseStats.Add(CompareStat("Item Level", item.inventoryItemLevel, 0, item));
            if (item.health != 0)
                baseStats.Add(CompareStat("Health", item.health, 0, item));
            if (item.mana != 0)
                baseStats.Add(CompareStat("Mana", item.mana, 0, item));
            if (item.minDamage != 0)
                baseStats.Add(CompareStat("Min Damage", item.minDamage, 0, item));
            if (item.maxDamage != 0)
                baseStats.Add(CompareStat("Max Damage", item.maxDamage, 0, item));
            if (item.attackSpeed != 0)
                baseStats.Add(CompareStat("Attack Speed", item.attackSpeed, 0, item));
            if (item.armorValue != 0)
                baseStats.Add(CompareStat("Armor Value", item.armorValue, 0, item));
            if (item.critChance != 0)
                baseStats.Add(CompareStat("Crit Chance", item.critChance, 0, item));
            if (item.critDamageBonus != 0)
                baseStats.Add(CompareStat("Crit Damage Bonus", item.critDamageBonus, 0, item));
            if (item.moveSpeed != 0)
                baseStats.Add(CompareStat("Move Speed", item.moveSpeed, 0, item));
        }

        return "\n" + string.Join("\n", baseStats);
    }

    private string CompareStat(string statName, float newValue, float? equippedValue, GameItem item)
    {
        float equippedValueToCompare = equippedValue ?? 0f;

        if (newValue == 0f && equippedValueToCompare == 0f)
            return ""; // Wenn beide Werte 0 sind, keinen Text anzeigen

        // Wenn das Item ausgerüstet ist, den Unterschied nicht anzeigen
        bool isEquipped = IsItemEquipped(item);
        string differenceText = "";
        if (!isEquipped) // Zeige den Unterschied nur an, wenn das Item nicht ausgerüstet ist
        {
            float difference = newValue - equippedValueToCompare;
            differenceText = difference > 0 ? $"<color=green>+{difference:F2}</color>" : difference < 0 ? $"<color=red>{difference:F2}</color>" : "";
        }

        // Rückgabe des Stat-Namens mit dem Wert und eventuell dem Unterschied
        return $"{statName}: {newValue:F2} {(differenceText != "" ? $"({differenceText})" : "")}";
    }

    private bool IsItemEquipped(GameItem item)
    {
        foreach (EquipSlot equipSlot in FindObjectOfType<InventoryManager>().equipSlots)
        {
            InventoryItem equippedItem = equipSlot.GetComponentInChildren<InventoryItem>();
            if (equippedItem != null && equippedItem.item == item)
            {
                return true; // Item ist im Equip-Slot
            }
        }
        return false; // Item ist nicht im Equip-Slot
    }


    private string GetItemEnchantments(GameItem item, GameItem equippedItem)
    {
        if (item.enchantments == null || item.enchantments.Count == 0)
            return "";

        List<string> enchantmentLines = new List<string>();

        foreach (var enchantment in item.enchantments)
        {
            List<string> enchantmentDetails = new List<string>();

            if (equippedItem != null)
            {
                var matchingEnchantment = equippedItem.enchantments?.Find(e => e.name == enchantment.name);

                if (enchantment.healthBonus != 0)
                    enchantmentDetails.Add(CompareStat("", enchantment.healthBonus, matchingEnchantment?.healthBonus, item));
                if (enchantment.manaBonus != 0)
                    enchantmentDetails.Add(CompareStat("", enchantment.manaBonus, matchingEnchantment?.manaBonus, item));
                if (enchantment.minDamageBonus != 0)
                    enchantmentDetails.Add(CompareStat("", enchantment.minDamageBonus, matchingEnchantment?.minDamageBonus, item));
                if (enchantment.maxDamageBonus != 0)
                    enchantmentDetails.Add(CompareStat("", enchantment.maxDamageBonus, matchingEnchantment?.maxDamageBonus, item));
                if (enchantment.attackSpeedBonus != 0)
                    enchantmentDetails.Add(CompareStat("", enchantment.attackSpeedBonus, matchingEnchantment?.attackSpeedBonus, item));
                if (enchantment.armorValueBonus != 0)
                    enchantmentDetails.Add(CompareStat("", enchantment.armorValueBonus, matchingEnchantment?.armorValueBonus, item));
                if (enchantment.critChanceBonus != 0)
                    enchantmentDetails.Add(CompareStat("", enchantment.critChanceBonus, matchingEnchantment?.critChanceBonus, item));
                if (enchantment.critDamageBonusBonus != 0)
                    enchantmentDetails.Add(CompareStat("", enchantment.critDamageBonusBonus, matchingEnchantment?.critDamageBonusBonus, item));
                if (enchantment.moveSpeedBonus != 0)
                    enchantmentDetails.Add(CompareStat("", enchantment.moveSpeedBonus, matchingEnchantment?.moveSpeedBonus, item));
            }
            else
            {
                if (enchantment.healthBonus != 0) enchantmentDetails.Add($"{enchantment.healthBonus:F2}");
                if (enchantment.manaBonus != 0) enchantmentDetails.Add($"{enchantment.manaBonus:F2}");
                if (enchantment.minDamageBonus != 0) enchantmentDetails.Add($"{enchantment.minDamageBonus:F2}");
                if (enchantment.maxDamageBonus != 0) enchantmentDetails.Add($"{enchantment.maxDamageBonus:F2}");
                if (enchantment.attackSpeedBonus != 0) enchantmentDetails.Add($"{enchantment.attackSpeedBonus:F2}");
                if (enchantment.armorValueBonus != 0) enchantmentDetails.Add($"{enchantment.armorValueBonus:F2}");
                if (enchantment.critChanceBonus != 0) enchantmentDetails.Add($"{enchantment.critChanceBonus:F2}");
                if (enchantment.critDamageBonusBonus != 0) enchantmentDetails.Add($"{enchantment.critDamageBonusBonus:F2}");
                if (enchantment.moveSpeedBonus != 0) enchantmentDetails.Add($"{enchantment.moveSpeedBonus:F2}");
            }

            if (enchantmentDetails.Count > 0)
                enchantmentLines.Add($"{enchantment.name}: " + string.Join("\n", enchantmentDetails));
        }

        return enchantmentLines.Count > 0 ? "\n\n\n" + string.Join("\n", enchantmentLines) : "";
    }

    private int CountRelevantValues(GameItem item, GameItem equippedItem)
    {
        int count = 0;

        // Count base stats, including comparing the item with the equipped version
        if (item.inventoryItemLevel != 0 || (equippedItem != null && equippedItem.inventoryItemLevel != 0)) count++;
        if (item.health != 0 || (equippedItem != null && equippedItem.health != 0)) count++;
        if (item.mana != 0 || (equippedItem != null && equippedItem.mana != 0)) count++;
        if (item.minDamage != 0 || (equippedItem != null && equippedItem.minDamage != 0)) count++;
        if (item.maxDamage != 0 || (equippedItem != null && equippedItem.maxDamage != 0)) count++;
        if (item.attackSpeed != 0 || (equippedItem != null && equippedItem.attackSpeed != 0)) count++;
        if (item.armorValue != 0 || (equippedItem != null && equippedItem.armorValue != 0)) count++;
        if (item.critChance != 0 || (equippedItem != null && equippedItem.critChance != 0)) count++;
        if (item.critDamageBonus != 0 || (equippedItem != null && equippedItem.critDamageBonus != 0)) count++;
        if (item.moveSpeed != 0 || (equippedItem != null && equippedItem.moveSpeed != 0)) count++;

        // Count enchantment stats, including comparing the item with the equipped version
        if (item.enchantments != null)
        {
            foreach (var enchantment in item.enchantments)
            {
                if (enchantment.healthBonus != 0 || (equippedItem?.enchantments?.Find(e => e.name == enchantment.name)?.healthBonus ?? 0) != 0) count++;
                if (enchantment.manaBonus != 0 || (equippedItem?.enchantments?.Find(e => e.name == enchantment.name)?.manaBonus ?? 0) != 0) count++;
                if (enchantment.minDamageBonus != 0 || (equippedItem?.enchantments?.Find(e => e.name == enchantment.name)?.minDamageBonus ?? 0) != 0) count++;
                if (enchantment.maxDamageBonus != 0 || (equippedItem?.enchantments?.Find(e => e.name == enchantment.name)?.maxDamageBonus ?? 0) != 0) count++;
                if (enchantment.attackSpeedBonus != 0 || (equippedItem?.enchantments?.Find(e => e.name == enchantment.name)?.attackSpeedBonus ?? 0) != 0) count++;
                if (enchantment.armorValueBonus != 0 || (equippedItem?.enchantments?.Find(e => e.name == enchantment.name)?.armorValueBonus ?? 0) != 0) count++;
                if (enchantment.critChanceBonus != 0 || (equippedItem?.enchantments?.Find(e => e.name == enchantment.name)?.critChanceBonus ?? 0) != 0) count++;
                if (enchantment.critDamageBonusBonus != 0 || (equippedItem?.enchantments?.Find(e => e.name == enchantment.name)?.critDamageBonusBonus ?? 0) != 0) count++;
                if (enchantment.moveSpeedBonus != 0 || (equippedItem?.enchantments?.Find(e => e.name == enchantment.name)?.moveSpeedBonus ?? 0) != 0) count++;
            }
        }

        return count;
    }

    // Adjust font size based on the count of relevant values
    private void AdjustFontSize(int valueCount)
    {
        int fontSize = 25;  // Default to max size

        if (valueCount >= 9) fontSize = 21;
        else if (valueCount >= 8) fontSize = 23;
        else if (valueCount >= 7) fontSize = 25;

        tooltipText.fontSize = fontSize;
        enchantmentsText.fontSize = fontSize;
        sellValueText.fontSize = fontSize;
    }

    // Set the sell value in the tooltip UI
    public void SetSellValue(float sellValue)
    {
        if (sellValueText != null)
        {
            sellValueText.text = $"\n\n Sell Value: {sellValue:F2}";
        }
    }
}
