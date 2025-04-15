using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blacksmith : Interactable
{
    // Referenz auf das Blacksmith-Canvas
    public GameObject blacksmithCanvas;
    public GameObject blacksmithShop;

    public override void Interact()
    {
        base.Interact();
        OpenBlacksmithCanvas();
    }

    void OpenBlacksmithCanvas()
    {
        if (blacksmithCanvas != null)
        {
            blacksmithCanvas.SetActive(true); // Aktiviert das Canvas
            Debug.Log("Opening Blacksmith Canvas...");
        }
        else
        {
            Debug.LogWarning("Blacksmith Canvas is not assigned!");
        }
    }

    void OpenBlacksmithShop()
    {
        if (blacksmithShop != null)
        {
            blacksmithShop.SetActive(true); // Aktiviert das Canvas
            Debug.Log("Opening Blacksmith Shop...");
        }
        else
        {
            Debug.LogWarning("Blacksmith Shop is not assigned!");
        }
    }
}
