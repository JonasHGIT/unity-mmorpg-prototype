/*
 * EquipSlot.cs
 * 
 * Author: Jonas Hammer
 * Description: Handhabt das Ausrüsten und Abnehmen von Items in Equip-Slots.
 * Last Edited: 16. April 2025
 */

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipSlot : InventorySlot
{
    public GameItemType allowedItemType; // Erlaubter Item-Typ für diesen Slot (z.B. Rüstung, Waffe)
    private Image background; // Hintergrundbild für den Slot
    private PlayerController playerController; // Referenz zum PlayerController, um die Spieler-Statistiken zu aktualisieren

    // Initialisierung des EquipSlots
    private void Awake()
    {
        // Hintergrundbild des Equip-Slots holen, falls es ein Kindobjekt gibt
        if (transform.childCount > 0)
        {
            background = transform.GetChild(0).GetComponent<Image>();
        }

        // PlayerController referenzieren
        playerController = FindObjectOfType<PlayerController>();
    }

    // Aktualisierung pro Frame
    private void Update()
    {
        // Zeige den Hintergrund nur an, wenn ein Item im Slot ist
        if (transform.childCount == 1 && background != null)
        {
            background.gameObject.SetActive(true);
        }
    }

    // Wird aufgerufen, wenn ein Item in den Slot gezogen wird
    public override void OnDrop(PointerEventData eventData)
    {
        // Das gezogene Item holen
        InventoryItem droppedItem = eventData.pointerDrag.GetComponent<InventoryItem>();

        if (droppedItem != null)
        {
            // Prüfen, ob das Item ausgerüstet werden kann
            if (!CanEquip(droppedItem))
            {
                Debug.LogWarning("Das Item kann nicht in diesen Slot gelegt werden.");
                // Item zurück an den originalen Slot setzen
                droppedItem.transform.SetParent(droppedItem.originalParent);
                droppedItem.transform.localPosition = Vector3.zero;
                return;
            }

            // Prüfen, ob bereits ein Item im Slot ist
            if (transform.childCount > 1)
            {
                // Entferne das vorhandene Item
                InventoryItem existingItem = transform.GetChild(1).GetComponent<InventoryItem>();

                if (existingItem != null)
                {
                    UnequipItem(existingItem);

                    // Entferne das alte Item
                    existingItem.transform.SetParent(droppedItem.originalParent);
                    existingItem.transform.localPosition = Vector3.zero;
                }
            }

            // Füge das neue Item hinzu
            droppedItem.transform.SetParent(transform);
            droppedItem.transform.localPosition = Vector3.zero;
            droppedItem.transform.SetAsLastSibling();

            // Blende den Hintergrund aus
            if (background != null)
            {
                background.gameObject.SetActive(false);
            }

            EquipItem(droppedItem);
        }
    }

    // Prüft, ob das Item in diesem Slot ausgerüstet werden kann
    private bool CanEquip(InventoryItem inventoryItem)
    {
        if (inventoryItem == null) return false;

        GameItem gameItem = inventoryItem.item;

        if (gameItem == null)
        {
            Debug.LogWarning("InventoryItem enthält kein gültiges GameItem.");
            return false;
        }

        // Überprüft, ob der Typ des Items mit dem erlaubten Slot-Typ übereinstimmt
        if (gameItem.type != allowedItemType)
        {
            Debug.LogWarning($"GameItem vom Typ {gameItem.type} kann nicht in Slot für Typ {allowedItemType} ausgerüstet werden.");
            return false;
        }

        return true;
    }

    // Rüstet das Item aus und aktualisiert die Spieler-Stats
    public void EquipItem(InventoryItem item)
    {
        if (playerController == null) return;

        GameItem gameItem = item.item;

        // Werte des Spielers basierend auf dem ausgerüsteten Item aktualisieren
        playerController.maxHealth += gameItem.health;
        playerController.currentHealth += gameItem.health;
        playerController.maxMana += gameItem.mana;
        playerController.currentMana += gameItem.mana;
        playerController.minDamage += gameItem.minDamage;
        playerController.maxDamage += gameItem.maxDamage;
        playerController.attackSpeed += gameItem.attackSpeed;
        playerController.armorValue += gameItem.armorValue;
        playerController.critChance += gameItem.critChance;
        playerController.critDamageBonus += gameItem.critDamageBonus;
        playerController.moveSpeed += gameItem.moveSpeed;

        Debug.Log($"Item {item.item.name} vom Typ {item.item.type} wurde erfolgreich in Slot {allowedItemType} ausgerüstet.");

        UpdatePlayerStatsUI();
    }

    // Nimmt das Item ab und setzt die Spieler-Stats zurück
    public void UnequipItem(InventoryItem item)
    {
        if (playerController == null) return;

        GameItem gameItem = item.item;

        // Werte des Spielers zurücksetzen
        playerController.maxHealth -= gameItem.health;
        playerController.currentHealth -= gameItem.health;
        playerController.maxMana -= gameItem.mana;
        playerController.currentMana -= gameItem.mana;
        playerController.minDamage -= gameItem.minDamage;
        playerController.maxDamage -= gameItem.maxDamage;
        playerController.attackSpeed -= gameItem.attackSpeed;
        playerController.armorValue -= gameItem.armorValue;
        playerController.critChance -= gameItem.critChance;
        playerController.critDamageBonus -= gameItem.critDamageBonus;
        playerController.moveSpeed -= gameItem.moveSpeed;

        Debug.Log($"Item {item.item.name} wurde erfolgreich aus Slot {allowedItemType} entfernt.");

        UpdatePlayerStatsUI();
    }

    // Aktualisiert die Benutzeroberfläche für die Spieler-Stats nach der Ausrüstung eines Items
    private void UpdatePlayerStatsUI()
    {
        PlayerStatsManager statsManager = FindObjectOfType<PlayerStatsManager>();
        if (statsManager != null)
        {
            statsManager.UpdateStatsUI();
        }
    }
}
