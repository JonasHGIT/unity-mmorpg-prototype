using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KoboldSafe : Interactable
{
    // Referenz auf das KoboldSafe-Canvas
    public GameObject koboldSafeCanvas;
    public GameObject inventoryCanvas;

    public override void Interact()
    {
        base.Interact();
        OpenKoboldSafe();
    }

    void OpenKoboldSafe()
    {
        if (koboldSafeCanvas != null)
        {
            koboldSafeCanvas.SetActive(true); // Aktiviert das Canvas
            inventoryCanvas.SetActive(true); // Aktiviert das Canvas
            Debug.Log("Opening Kobold-Safe...");
        }
        else
        {
            Debug.LogWarning("koboldSafe-Canvas is not assigned!");
        }
    }
}
