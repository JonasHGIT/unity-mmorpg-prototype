using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("UI")]
    public Image image;
    public TMP_Text countText;
    public GameObject backgroundPrefab;
    public TMP_Text itemLevelText; // Verweis auf das TMP-Text-Element

    [HideInInspector] public GameItem item;
    [HideInInspector] public int count = 1;

    [HideInInspector] public Transform originalParent;
    private CanvasGroup canvasGroup;

    [SerializeField] private GameObject currentBackground;

    private InventoryManager inventoryManager; // Referenz zu InventoryManager
    private EnemyController enemyController; // Referenz zu EnemyController

    // Neues Feld zum Speichern des initialen ItemLevels
    public int initialItemLevel;

    private Coroutine hoverCoroutine; // Speichert die laufende Coroutine

    [HideInInspector] public int itemLevel;

    private float lastClickTime;
    private const float doubleClickThreshold = 0.25f; // Zeitfenster für Doppelklick (in Sekunden)

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        inventoryManager = FindObjectOfType<InventoryManager>(); // Holt die Instanz des InventoryManagers
    }

    public void InitialiseItem(GameItem newItem)
    {
        item = newItem;
        item.ApplyBaseValues();
        item.CalculateSellValue();
        image.sprite = newItem.image;
        RefreshCount();
        SetBackgroundPrefab();

        if (initialItemLevel == 0)
        {
            initialItemLevel = item.itemLevel;
            item.inventoryItemLevel = initialItemLevel;
        }

        RefreshItemLevelCount(); // Aktualisiere die Anzeige des Item Levels
    }

    public void RefreshItemLevelCount()
    {
        if (itemLevelText != null)
        {
            itemLevelText.text = initialItemLevel.ToString();
        }
    }

    private void SetBackgroundPrefab()
    {
        if (backgroundPrefab != null)
        {
            RectTransform parentRectTransform = (RectTransform)transform;
            Vector2 parentSize = parentRectTransform.sizeDelta;

            if (currentBackground != null)
            {
                Destroy(currentBackground);
            }

            currentBackground = Instantiate(backgroundPrefab, transform);
            currentBackground.transform.SetAsFirstSibling();

            RectTransform backgroundRect = currentBackground.GetComponent<RectTransform>();
            backgroundRect.anchoredPosition = Vector2.zero;
            backgroundRect.sizeDelta = parentSize;
            backgroundRect.localScale = Vector3.one;
        }
    }

    public void RefreshCount()
    {
        countText.text = count > 1 ? count.ToString() : ""; // Setze den Text je nach Anzahl der Items
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent; // Speichere den originalen Parent
        transform.SetParent(transform.root); // Setze den Parent auf das Root-Objekt
        transform.SetAsLastSibling(); // Setze das Item an die letzte Position
        canvasGroup.blocksRaycasts = false; // Deaktiviere Raycast, damit die Maus-Events durch das Item durchgehen
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition; // Setze die Position des Items auf die Mausposition

        // Überprüfe, ob das Item über einem Seitenbutton schwebt
        for (int i = 0; i < inventoryManager.pageButtons.Length; i++)
        {
            GameObject pageButton = inventoryManager.pageButtons[i];
            if (RectTransformUtility.RectangleContainsScreenPoint(pageButton.GetComponent<RectTransform>(), Input.mousePosition, null))
            {
                if (hoverCoroutine == null) // Wenn keine Coroutine läuft, starte eine
                {
                    hoverCoroutine = StartCoroutine(HoverOverButton(i));
                }
                return;
            }
        }

        // Wenn das Item nicht mehr über einem Button ist, stoppe die Coroutine
        if (hoverCoroutine != null)
        {
            StopCoroutine(hoverCoroutine);
            hoverCoroutine = null;
        }
    }

    private IEnumerator HoverOverButton(int pageIndex)
    {
        yield return new WaitForSeconds(0.5f); // Warte 0.5 Sekunde

        inventoryManager.SwitchPageOnHover(pageIndex); // Wechsel nach 0.5 Sekunde die Seite
        hoverCoroutine = null; // Coroutine beenden
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true; // Raycast wieder aktivieren

        // Überprüfen, ob das Item außerhalb des EquipSlots losgelassen wurde
        if (eventData.pointerCurrentRaycast.gameObject == null || 
            eventData.pointerCurrentRaycast.gameObject.GetComponent<EquipSlot>() == null)
        {
            // Versuchen, das EquipSlot zu finden, in dem das Item war
            EquipSlot equipSlot = originalParent.GetComponent<EquipSlot>();
            if (equipSlot != null)
            {
                // Das Item wird aus dem Slot entfernt, also rufe UnequipItem auf
                equipSlot.UnequipItem(this);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (inventoryManager != null)
        {
            inventoryManager.ShowTooltip(item); // Tooltip für das Item anzeigen
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (inventoryManager != null)
        {
            inventoryManager.HideTooltip(); // Tooltip ausblenden
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Doppelklick-Logik
        if (Time.time - lastClickTime <= doubleClickThreshold)
        {
            // Prüfen, ob das Item im Inventar ist, um eine doppelte Bewegung zu verhindern
            if (transform.parent != null && transform.parent.GetComponent<EquipSlot>() == null)
            {
                EquipItemInProperSlot();
            }
        }
        lastClickTime = Time.time;
    }

    private void EquipItemInProperSlot()
    {
        if (inventoryManager == null || item == null) return;

        foreach (EquipSlot slot in inventoryManager.equipSlots)
        {
            if (slot.allowedItemType == item.type)
            {
                InventoryItem itemToSwap = null;

                // Überprüfe alle Kinder des Slots
                foreach (Transform child in slot.transform)
                {
                    InventoryItem childItem = child.GetComponent<InventoryItem>();

                    if (childItem != null)
                    {
                        // Wenn ein InventoryItem gefunden wurde, markiere es zum Swappen
                        itemToSwap = childItem;
                        break;
                    }
                    else
                    {
                        // Wenn es kein InventoryItem ist, deaktiviere das GameObject
                        child.gameObject.SetActive(false);
                    }
                }

                if (itemToSwap != null)
                {
                    // Swappe das gefundene Item mit dem aktuellen
                    itemToSwap.originalParent = originalParent; // Setze den Parent des geswappten Items
                    itemToSwap.transform.SetParent(originalParent); // Verschiebe das bestehende Item zurück zum Inventory
                    itemToSwap.transform.localPosition = Vector3.zero;
                    itemToSwap.transform.SetAsLastSibling(); // Stelle sicher, dass es oben angezeigt wird
                }

                // Bewege das aktuelle Item in den EquipSlot
                originalParent = transform.parent; // Speichere den aktuellen Parent, bevor das Item verschoben wird
                transform.SetParent(slot.transform);
                transform.localPosition = Vector3.zero;
                transform.SetAsLastSibling(); // Stelle sicher, dass das Item oben angezeigt wird

                // Markiere das Item als ausgerüstet
                slot.EquipItem(this);
                break;
            }
        }
    }
}
