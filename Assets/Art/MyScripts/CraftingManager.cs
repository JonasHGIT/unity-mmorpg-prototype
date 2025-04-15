using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingManager : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public CraftingSlot[] craftingSlots; // Die vier Slots für das Crafting
    public GameObject inventoryItemPrefab; // Das Prefab für die InventoryItems
    public TextMeshProUGUI craftButtonText;
    public Transform craftingResultSlot; // Slot für das resultierende Item
    private LootManager lootManager;

    private void Start()
    {
        lootManager = FindObjectOfType<LootManager>();
        UpdateCraftButton();
    }

    // Wird aufgerufen, wenn ein Item in einen Crafting-Slot gelegt oder entfernt wird
    public void OnCraftingSlotChanged()
    {
        UpdateCraftButton();
    }

    // Überprüft, ob der Crafting-Prozess gestartet werden kann
    private void UpdateCraftButton()
    {
        bool canCraft = CanCraft();
        craftButtonText.text = canCraft ? "Craft!" : "Invalid Combination";
    }

    // Überprüft, ob die vier Items kombiniert werden können
    private bool CanCraft()
    {
        if (craftingSlots.Length < 4) return false;

        Rarity? rarity = null;

        foreach (var slot in craftingSlots)
        {
            var item = slot.GetItem();
            if (item == null) return false;

            if (rarity == null)
            {
                rarity = item.rarity;
            }
            else if (item.rarity != rarity)
            {
                return false;
            }
        }

        return true;
    }

    // Der Crafting-Prozess, der aufgerufen wird, wenn der Spieler auf den Craft-Button klickt
    public void Craft()
    {
        if (!CanCraft()) return;

        // Berechne die Basiswerte der kombinierten Items
        GameItem craftedItem = CalculateCraftedItem();

        // Entferne die Items aus den Crafting-Slots und füge das neue Item dem Inventar hinzu
        foreach (var slot in craftingSlots)
        {
            slot.ClearSlot();
        }

        // Setze das erstellte Item in den Crafting-Result-Slot
        DisplayCraftedItem(craftedItem);

        // Füge das Item dem Inventar hinzu
        inventoryManager.AddItem(craftedItem, craftedItem.itemLevel);

        UpdateCraftButton();
    }

    // Berechnet das resultierende Item basierend auf den kombinierten Items
    private GameItem CalculateCraftedItem()
    {
        GameItem newItem = CreateBaseItemFromCrafting(); // Erstellt ein Basisitem

        // Berechne den Durchschnitt des Item-Levels der verwendeten Items
        int totalItemLevel = 0;
        float totalHealth = 0, totalMana = 0, totalMinDamage = 0, totalMaxDamage = 0;
        float totalArmor = 0, totalCritChance = 0, totalCritDamage = 0, totalMoveSpeed = 0;

        foreach (var slot in craftingSlots)
        {
            GameItem item = slot.GetItem();
            totalItemLevel += item.itemLevel;

            // Basiswerte aufsummieren
            totalHealth += item.health;
            totalMana += item.mana;
            totalMinDamage += item.minDamage;
            totalMaxDamage += item.maxDamage;
            totalArmor += item.armorValue;
            totalCritChance += item.critChance;
            totalCritDamage += item.critDamageBonus;
            totalMoveSpeed += item.moveSpeed;
        }

        int averageItemLevel = Mathf.RoundToInt(totalItemLevel / craftingSlots.Length);

        // Werte des neuen Items setzen
        newItem.itemLevel = averageItemLevel;
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

    // Erstelle ein Basis-Item für das Crafting-Ergebnis
    private GameItem CreateBaseItemFromCrafting()
    {
        GameItem newItem = ScriptableObject.CreateInstance<GameItem>();
        newItem.rarity = GetNextRarity(craftingSlots[0].GetItem().rarity);
        newItem.name = $"{newItem.rarity} Crafted Item";
        newItem.stackable = false; // Angenommen, es ist kein stapelbares Item
        return newItem;
    }

    // Bestimme die nächste Seltenheit basierend auf der aktuellen Seltenheit
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

    // Zeige das hergestellte Item im Crafting-Result-Slot an
    private void DisplayCraftedItem(GameItem craftedItem)
    {
        // Entferne altes Ergebnis
        foreach (Transform child in craftingResultSlot)
        {
            Destroy(child.gameObject);
        }

        // Erstelle ein neues Inventory-Item im Crafting-Result-Slot
        GameObject newInventoryItem = Instantiate(inventoryItemPrefab, craftingResultSlot);
        InventoryItem inventoryItemComponent = newInventoryItem.GetComponent<InventoryItem>();
        inventoryItemComponent.InitialiseItem(craftedItem);

        // Setze den Hintergrund basierend auf der Seltenheit des Items
        GameObject backgroundPrefab = lootManager.GetBackgroundPrefabByRarity(craftedItem.rarity);
        if (backgroundPrefab != null)
        {
            GameObject backgroundObject = Instantiate(backgroundPrefab, newInventoryItem.transform);
            backgroundObject.transform.SetAsFirstSibling();
        }

        // Setze das Icon des Items
        if (craftedItem.image != null)
        {
            inventoryItemComponent.image.sprite = craftedItem.image;
        }
    }
}
