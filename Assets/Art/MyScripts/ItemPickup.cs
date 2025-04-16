/*
 * ------------------------------------------------------------------------------
 * Script:       ItemPickup.cs
 * Author:       Jonas Hammer
 * Created:      [Erstellungsdatum]
 * Last Edited:  16. April 2025
 * Description:  Dieses Skript ermöglicht das Aufsammeln von Items durch den
 *               Spieler. Es interagiert mit dem Inventar- und Loot-System und
 *               berücksichtigt dabei den Level des besiegten Gegners.
 *
 * Hauptfunktionen:
 * - Automatische Referenzierung von InventoryManager, LootManager & EnemyController
 * - Aufheben und Zerstören des Items nach erfolgreicher Aufnahme
 * - Übergibt den Gegner-Level zur Item-Verarbeitung (z. B. für Drop-Logik)
 * - Unterstützt zerstörbare Container durch Entfernen der Parent-Objekte
 *
 * Unterstützt:
 * - Automatisches Finden von Game-Management-Komponenten
 * - Zerstörung des Pickups und ggf. des gesamten Loot-Objekts
 * - Flexibles System über Vererbung von `Interactable.cs`
 *
 * Dependencies:
 * - GameItem.cs
 * - InventoryManager.cs
 * - LootManager.cs
 * - EnemyController.cs
 * - Interactable.cs (Basis-Klasse)
 *
 * Hinweise:
 * - Das Item-GameObject sollte einen Collider mit `IsTrigger` aktiv haben
 * - Parent-Objekte (z. B. Loot-Säcke) werden automatisch entfernt
 * - `Interact()` wird von der übergeordneten Klasse aufgerufen
 * ------------------------------------------------------------------------------
 */


using UnityEngine;

public class ItemPickup : Interactable
{
    public GameItem item;
    public InventoryManager inventoryManager;
    public LootManager lootManager;
    public EnemyController enemyController; // Added reference to EnemyController

    public override void Interact()
    {
        base.Interact();
        PickUp();
    }

    void Start()
    {
        if (inventoryManager == null)
        {
            inventoryManager = FindObjectOfType<InventoryManager>();
        }
        if (lootManager == null)
        {
            lootManager = FindObjectOfType<LootManager>();
        }
        if (enemyController == null)
        {
            enemyController = FindObjectOfType<EnemyController>(); // Get the EnemyController
        }

        objectName = item.name;
        base.Start();
    }

    void PickUp()
    {
        Debug.Log("Picking up " + item.name);

        // Pass the enemyLevel to the AddItem method
        bool result = inventoryManager.AddItem(item, enemyController.enemyLevel);

        if (result)
        {
            Debug.Log("Item added to inventory");
            Destroy(gameObject);

            if (gameObject.transform.parent != null)
            {
                Destroy(gameObject.transform.parent.gameObject);
            }
        }
        else
        {
            Debug.Log("Inventory is full or item could not be added");
        }
    }
}