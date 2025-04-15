using UnityEngine;
using UnityEngine.UI;

public class CraftingTable : Interactable
{
    public string itemToCraft;
    public int requiredMaterials = 3;
    public int playerMaterials = 5;
    
    // Referenz zum Crafting-Canvas
    public GameObject craftingCanvas;
    public GameObject inventoryCanvas;

    public override void Interact()
    {
        base.Interact();
        OpenCraftingMenu();
    }

    void CraftItem()
    {
        if (playerMaterials >= requiredMaterials)
        {
            // Code, um das Item zu craften
            Debug.Log("Crafting " + itemToCraft + " completed!");
        }
        else
        {
            Debug.Log("Not enough materials to craft " + itemToCraft);
        }
    }

    // Methode zum Öffnen des Crafting-Menüs
    void OpenCraftingMenu()
    {
        if (craftingCanvas != null)
        {
            craftingCanvas.SetActive(true); // Aktiviert das Canvas
            inventoryCanvas.SetActive(true); // Aktiviert das Canvas
        }
        else
        {
            Debug.LogWarning("Crafting Canvas is not assigned!");
        }
    }
}
