/*
 * CraftingManager.cs
 *
 * Author: Jonas Hammer
 * Description: Verarbeitet die Logik für das Crafting-System. Kombiniert Items aus Slots, erstellt neue Items
 *              basierend auf ihren Werten und stellt sie grafisch im UI dar.
 * Last Edited: 16. April 2025
 *
 * Hinweise:
 * - Verwendet CraftingSlot, InventoryManager, LootManager und GameItem
 * - Das neue Item wird berechnet als Durchschnitt der kombinierten Items
 * - Nur Items mit gleicher Seltenheit können kombiniert werden
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingManager : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public CraftingSlot[] craftingSlots;                 // Die vier Slots für das Crafting
    public GameObject inventoryItemPrefab;               // Prefab für das UI-Element eines Items
    public TextMeshProUGUI craftButtonText;              // Der Buttontext, z. B. "Craft!" oder Fehlermeldung
    public Transform craftingResultSlot;                 // UI-Slot für das hergestellte Item
    private LootManager lootManager;

    private void Start()
    {
        lootManager = FindObjectOfType<LootManager>();
        UpdateCraftButton();
    }

    /// <summary>
    /// Wird aufgerufen, wenn sich ein Crafting-Slot verändert (Item rein/raus).
    /// </summary>
    public void OnCraftingSlotChanged()
    {
        UpdateCraftButton();
    }

    /// <summary>
    /// Aktiviert oder deaktiviert den Crafting-Button basierend auf Gültigkeit der Kombination.
    /// </summary>
    private void UpdateCraftButton()
    {
        bool canCraft = CanCraft();
        craftButtonText.text = canCraft ? "Craft!" : "Invalid Combination";
    }

    /// <summary>
    /// Überprüft, ob vier gültige Items vorhanden sind, alle mit derselben Seltenheit.
    /// </summary>
    private bool CanCraft()
    {
        if (craftingSlots.Length < 4) return false;

        Rarity? rarity = null;

        foreach (var slot in craftingSlots)
        {
            var item = slot.GetItem();
            if (item == null) return false;

            if (rarity == null)
                rarity = item.rarity;
            else if (item.rarity != rarity)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Führt den Crafting-Prozess aus, erzeugt ein neues Item und fügt es dem Inventar hinzu.
    /// </summary>
    public void Craft()
    {
        if (!CanCraft()) return;

        GameItem craftedItem = CalculateCraftedItem();

        foreach (var slot in craftingSlots)
        {
            slot.ClearSlot(); // Leere die Crafting-Slots
        }

        DisplayCraftedItem(craftedItem); // Zeige Item im UI
        inventoryManager.AddItem(craftedItem, craftedItem.itemLevel); // Inventar hinzufügen
        UpdateCraftButton();
    }

    /// <summary>
    /// Berechnet ein neues Item, basierend auf dem Durchschnitt der Werte der kombinierten Items.
    /// </summary>
    private GameItem CalculateCraftedItem()
    {
        GameItem newItem = CreateBaseItemFromCrafting(); // Erstellt ein Basisitem

        int totalItemLevel = 0;
        float totalHealth = 0, totalMana = 0, totalMinDamage = 0, totalMaxDamage = 0;
        float totalArmor = 0, totalCritChance = 0, totalCritDamage = 0, totalMoveSpeed = 0;

        foreach (var slot in craftingSlots)
        {
            GameItem item = slot.GetItem();
            totalItemLevel += item.itemLevel;
            totalHealth += item.health;
            totalMana += item.mana;
            totalMinDamage += item.minDamage;
            totalMaxDamage += item.maxDamage;
            totalArmor += item.armorValue;
            totalCritChance += item.critChance;
            totalCritDamage += item.critDamageBonus;
            totalMoveSpeed += item.moveSpeed;
        }

        int avgLevel = Mathf.RoundToInt(totalItemLevel / craftingSlots.Length);

        newItem.itemLevel = avgLevel;
        newItem.health = totalHealth / craftingSlots.Length;
        newItem.mana = totalMana / craftingSlots.Length;
        newItem.minDamage = totalMinDamage / craftingSlots.Length;
        newItem.maxDamage = totalMaxDamage / craftingSlots.Length;
        newItem.armorValue = totalArmor / craftingSlots.Length;
        newItem.critChance = totalCritChance / craftingSlots.Length;
        newItem.critDamageBonus = totalCritDamage / craftingSlots.Length;
        newItem.moveSpeed = totalMoveSpeed / craftingSlots.Length;

        return newItem;
    }

    /// <summary>
    /// Erstellt ein neues Basis-Item mit erhöhter Seltenheit basierend auf dem ersten Item.
    /// </summary>
    private GameItem CreateBaseItemFromCrafting()
    {
        GameItem newItem = ScriptableObject.CreateInstance<GameItem>();
        newItem.rarity = GetNextRarity(craftingSlots[0].GetItem().rarity);
        newItem.name = $"{newItem.rarity} Crafted Item";
        newItem.stackable = false;
        return newItem;
    }

    /// <summary>
    /// Gibt die nächsthöhere Rarity zurück.
    /// </summary>
    private Rarity GetNextRarity(Rarity currentRarity)
    {
        return currentRarity switch
        {
            Rarity.Common => Rarity.Uncommon,
            Rarity.Uncommon => Rarity.Rare,
            Rarity.Rare => Rarity.Epic,
            Rarity.Epic => Rarity.Legendary,
            _ => currentRarity,
        };
    }

    /// <summary>
    /// Zeigt das hergestellte Item im UI-Slot inklusive Hintergrundbild und Icon.
    /// </summary>
    private void DisplayCraftedItem(GameItem craftedItem)
    {
        foreach (Transform child in craftingResultSlot)
        {
            Destroy(child.gameObject); // Vorherige Darstellung löschen
        }

        GameObject newInventoryItem = Instantiate(inventoryItemPrefab, craftingResultSlot);
        InventoryItem inventoryItemComponent = newInventoryItem.GetComponent<InventoryItem>();
        inventoryItemComponent.InitialiseItem(craftedItem);

        GameObject backgroundPrefab = lootManager.GetBackgroundPrefabByRarity(craftedItem.rarity);
        if (backgroundPrefab != null)
        {
            GameObject backgroundObject = Instantiate(backgroundPrefab, newInventoryItem.transform);
            backgroundObject.transform.SetAsFirstSibling();
        }

        if (craftedItem.image != null)
        {
            inventoryItemComponent.image.sprite = craftedItem.image;
        }
    }
}
