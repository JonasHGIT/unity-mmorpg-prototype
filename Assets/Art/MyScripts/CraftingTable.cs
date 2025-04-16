/*
 * CraftingTable.cs
 *
 * Author: Jonas Hammer
 * Description: Script für ein interaktives Crafting-Objekt im Spiel. Öffnet das Crafting-Menü,
 *              prüft Materialien und ermöglicht das Craften eines Items.
 * Last Edited: 16. April 2025
 *
 Ü Erbt von Interactable
 */

using UnityEngine;
using UnityEngine.UI;

public class CraftingTable : Interactable
{
    [Header("Crafting Settings")]
    public string itemToCraft;                // Name des Items, das gecraftet werden soll
    public int requiredMaterials = 3;         // Benötigte Materialien zum Craften
    public int playerMaterials = 5;           // Verfügbare Materialien des Spielers (Platzhalterwert)

    [Header("UI References")]
    public GameObject craftingCanvas;         // Referenz zum Crafting-Canvas
    public GameObject inventoryCanvas;        // Referenz zum Inventar-Canvas

    // Wird ausgelöst, wenn der Spieler mit dem Crafting-Tisch interagiert
    public override void Interact()
    {
        base.Interact();
        OpenCraftingMenu();
    }

    // Führt den Crafting-Vorgang durch
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

    // Öffnet das Crafting-Menü (und Inventar)
    void OpenCraftingMenu()
    {
        if (craftingCanvas != null)
        {
            craftingCanvas.SetActive(true);
            inventoryCanvas.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Crafting Canvas is not assigned!");
        }
    }
}
