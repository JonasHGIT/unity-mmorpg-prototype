using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoSpawnItem : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public GameItem[] itemsToPickup;

    public void PickupItem(int id)
    {
        bool result = inventoryManager.AddItem(itemsToPickup[id], 5);
        if (result == true)
        {
            Debug.Log("Item added");
        } 
        else
        {
            Debug.Log("ITEM NOT ADDED");
        }
    }
}
