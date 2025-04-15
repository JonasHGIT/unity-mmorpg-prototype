using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI; // Für die UI-Komponenten wie Button und Image

public class InventoryManager : MonoBehaviour
{
    public TooltipManager tooltipManager;
    public int maxStackedItems = 99;
    public InventorySlot[] inventorySlots; // alle Slots (für alle Seiten)
    public EquipSlot[] equipSlots;
    public GameObject inventoryItemPrefab;
    public Canvas canvasShop;

    // Neue Variablen für Seiten-Parent-Objekte und Seitenmanagement
    public GameObject[] pageParents; // Parent-GameObjects für jede Seite
    public int slotsPerPage = 28; // Anzahl der Slots pro Seite
    public GameObject[] pageButtons; // Referenz zu den Seitenbuttons (10 Buttons)
    public Sprite activeButtonSprite; // Sprite für aktiven Button
    public Sprite inactiveButtonSprite; // Sprite für inaktive Buttons
    private int currentPage = 0; // Aktuelle Seite, die angezeigt wird

    // UI für Währung
    public TextMeshProUGUI bronzeAmountText;
    public TextMeshProUGUI silverAmountText; // Neu hinzugefügt
    public TextMeshProUGUI goldAmountText;   // Neu hinzugefügt

    private InventorySlot selectedSlot;
    private LootManager lootManager;

    // Interne Währungswerte
    private int bronzeAmount = 0;
    private int silverAmount = 0;
    private int goldAmount = 0;

    void Start()
    {
        lootManager = FindObjectOfType<LootManager>();
        ShowPage(0); // Starte mit der ersten Seite
    }

    void Update()
    {
        DetectRightClick();
    }

    // Methode, um eine bestimmte Seite anzuzeigen
    public void ShowPage(int pageNumber)
    {
        // Alle Seiten außer der aktuellen deaktivieren
        for (int i = 0; i < pageParents.Length; i++)
        {
            pageParents[i].SetActive(i == pageNumber); // Nur die aktuelle Seite aktivieren
        }

        // Setze die aktuelle Seite als letztes Sibling, damit sie immer oben liegt
        pageParents[pageNumber].transform.SetAsLastSibling();

        currentPage = pageNumber;
        int startSlot = pageNumber * slotsPerPage;
        int endSlot = Mathf.Min(startSlot + slotsPerPage, inventorySlots.Length);

        // Deaktiviere alle Slots
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].gameObject.SetActive(false);
        }

        // Aktiviere nur die Slots der aktuellen Seite
        for (int i = startSlot; i < endSlot; i++)
        {
            inventorySlots[i].gameObject.SetActive(true);
        }

        // Aktualisiere das Bild des aktiven Buttons
        UpdatePageButtonVisuals(pageNumber);
    }

    // Methode, um das Bild des aktiven/inaktiven Buttons zu ändern
    void UpdatePageButtonVisuals(int activePageIndex)
    {
        for (int i = 0; i < pageButtons.Length; i++)
        {
            Image buttonImage = pageButtons[i].GetComponent<Image>(); // Hole das Image des Buttons
            if (i == activePageIndex)
            {
                buttonImage.sprite = activeButtonSprite; // Aktives Bild setzen
            }
            else
            {
                buttonImage.sprite = inactiveButtonSprite; // Inaktives Bild setzen
            }
        }
    }

    public void OnPageButtonClick(int pageIndex)
    {
        ShowPage(pageIndex);
    }

    public void SwitchPageOnHover(int pageIndex)
    {
        ShowPage(pageIndex); // Seite wechseln
    }

    public bool AddItem(GameItem item, int enemyLevel)
    {
        bool isPotion = item.type == GameItemType.Potion;
        bool itemExistsInInventory = false;

        for (int i = 0; i < equipSlots.Length; i++)
        {
            EquipSlot slot = equipSlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null &&
                itemInSlot.item.Equals(item) &&
                itemInSlot.count < maxStackedItems &&
                item.stackable)
            {
                itemInSlot.count++;
                itemInSlot.RefreshCount();
                return true;
            }

            if (itemInSlot != null && itemInSlot.item.Equals(item))
            {
                itemExistsInInventory = true;
            }
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null &&
                itemInSlot.item.Equals(item) &&
                itemInSlot.count < maxStackedItems &&
                item.stackable)
            {
                itemInSlot.count++;
                itemInSlot.RefreshCount();
                return true;
            }

            if (itemInSlot != null && itemInSlot.item.Equals(item))
            {
                itemExistsInInventory = true;
            }
        }

        if (!isPotion && !itemExistsInInventory)
        {
            item = item.Clone();
            item.itemLevel = enemyLevel;
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                SpawnNewItem(item, slot);
                return true;
            }
        }

        return false;
    }

    void SpawnNewItem(GameItem item, InventorySlot slot)
    {
        GameObject newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItemGo.GetComponent<InventoryItem>();
        inventoryItem.InitialiseItem(item);

        if (lootManager != null)
        {
            GameObject backgroundPrefab = lootManager.GetBackgroundPrefabByRarity(item.rarity);
            if (backgroundPrefab != null)
            {
                inventoryItem.backgroundPrefab = backgroundPrefab;
                inventoryItem.InitialiseItem(item);
            }
        }
    }

    public void DestroySelectedItem()
    {
        if (selectedSlot != null)
        {
            InventoryItem itemInSlot = selectedSlot.GetComponentInChildren<InventoryItem>();

            if (itemInSlot != null)
            {
                if (itemInSlot.item.stackable && itemInSlot.count > 1)
                {
                    itemInSlot.count--;
                    itemInSlot.RefreshCount();
                }
                else
                {
                    Destroy(itemInSlot.gameObject);
                }
            }
        }
    }

    public void SellSelectedItem()
    {
        if (canvasShop != null && canvasShop.gameObject.activeInHierarchy)
        {
            if (selectedSlot != null)
            {
                InventoryItem itemInSlot = selectedSlot.GetComponentInChildren<InventoryItem>();

                if (itemInSlot != null)
                {
                    int sellValue = itemInSlot.item.sellValue;

                    if (itemInSlot.item.stackable && itemInSlot.count > 1)
                    {
                        itemInSlot.count--;
                        itemInSlot.RefreshCount();
                    }
                    else
                    {
                        Destroy(itemInSlot.gameObject);
                    }

                    IncreaseCurrency(sellValue);
                }
            }
        }
    }

    void DetectRightClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, raycastResults);

            foreach (RaycastResult result in raycastResults)
            {
                InventorySlot slot = result.gameObject.GetComponent<InventorySlot>();
                if (slot != null)
                {
                    selectedSlot = slot;

                    if (canvasShop != null && canvasShop.gameObject.activeInHierarchy)
                    {
                        SellSelectedItem();
                    }
                    else
                    {
                        DestroySelectedItem();
                    }

                    break;
                }
            }
        }
    }

    void IncreaseCurrency(int bronzeAmountToAdd)
    {
        bronzeAmount += bronzeAmountToAdd;

        if (bronzeAmount >= 100)
        {
            silverAmount += bronzeAmount / 100;
            bronzeAmount = bronzeAmount % 100;
        }

        if (silverAmount >= 100)
        {
            goldAmount += silverAmount / 100;
            silverAmount = silverAmount % 100;
        }

        UpdateCurrencyUI();
    }

    void UpdateCurrencyUI()
    {
        if (bronzeAmountText != null)
        {
            bronzeAmountText.text = bronzeAmount.ToString();
        }

        if (silverAmountText != null)
        {
            silverAmountText.text = silverAmount.ToString();
        }

        if (goldAmountText != null)
        {
            goldAmountText.text = goldAmount.ToString();
        }
    }

    public void ShowTooltip(GameItem item)
    {
        if (tooltipManager != null)
        {
            tooltipManager.ShowTooltip(item);
            tooltipManager.SetSellValue(item.sellValue);
        }
    }

    public void HideTooltip()
    {
        if (tooltipManager != null)
        {
            tooltipManager.HideTooltip();
        }
    }
}
