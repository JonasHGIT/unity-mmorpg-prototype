using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipSlot : InventorySlot
{
    public GameItemType allowedItemType;
    private Image background;
    private PlayerController playerController;

    private void Awake()
    {
        if (transform.childCount > 0)
        {
            background = transform.GetChild(0).GetComponent<Image>();
        }

        playerController = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        if (transform.childCount == 1 && background != null)
        {
            background.gameObject.SetActive(true);
        }
    }

    public override void OnDrop(PointerEventData eventData)
    {
        InventoryItem droppedItem = eventData.pointerDrag.GetComponent<InventoryItem>();

        if (droppedItem != null)
        {
            if (!CanEquip(droppedItem))
            {
                Debug.LogWarning("Das Item kann nicht in diesen Slot gelegt werden.");
                droppedItem.transform.SetParent(droppedItem.originalParent);
                droppedItem.transform.localPosition = Vector3.zero;
                return;
            }

            // Prüfen, ob bereits ein Item im Slot ist
            if (transform.childCount > 1)
            {
                InventoryItem existingItem = transform.GetChild(1).GetComponent<InventoryItem>();

                if (existingItem != null)
                {
                    UnequipItem(existingItem);

                    // Entfernen des alten Items
                    existingItem.transform.SetParent(droppedItem.originalParent);
                    existingItem.transform.localPosition = Vector3.zero;
                }
            }

            // Hinzufügen des neuen Items
            droppedItem.transform.SetParent(transform);
            droppedItem.transform.localPosition = Vector3.zero;
            droppedItem.transform.SetAsLastSibling();

            if (background != null)
            {
                background.gameObject.SetActive(false);
            }

            EquipItem(droppedItem);
        }
    }

    private bool CanEquip(InventoryItem inventoryItem)
    {
        if (inventoryItem == null) return false;

        GameItem gameItem = inventoryItem.item;

        if (gameItem == null)
        {
            Debug.LogWarning("InventoryItem enthält kein gültiges GameItem.");
            return false;
        }

        if (gameItem.type != allowedItemType)
        {
            Debug.LogWarning($"GameItem vom Typ {gameItem.type} kann nicht in Slot für Typ {allowedItemType} ausgerüstet werden.");
            return false;
        }

        return true;
    }

    public void EquipItem(InventoryItem item)
    {
        if (playerController == null) return;

        GameItem gameItem = item.item;

        // Werte aktualisieren
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

    public void UnequipItem(InventoryItem item)
    {
        if (playerController == null) return;

        GameItem gameItem = item.item;

        // Werte zurücksetzen
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

    private void UpdatePlayerStatsUI()
    {
        PlayerStatsManager statsManager = FindObjectOfType<PlayerStatsManager>();
        if (statsManager != null)
        {
            statsManager.UpdateStatsUI();
        }
    }
}