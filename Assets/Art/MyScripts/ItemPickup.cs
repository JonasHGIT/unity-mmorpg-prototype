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