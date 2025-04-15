using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Juweler : Interactable
{
    // Referenz auf das Juweler-Canvas
    public GameObject jewelerCanvas;

    public override void Interact()
    {
        base.Interact();
        OpenJewelerShop();
    }

    void OpenJewelerShop()
    {
        if (jewelerCanvas != null)
        {
            jewelerCanvas.SetActive(true); // Aktiviert das Canvas
            Debug.Log("Opening Jeweler Shop...");
        }
        else
        {
            Debug.LogWarning("Jeweler Canvas is not assigned!");
        }
    }
}
