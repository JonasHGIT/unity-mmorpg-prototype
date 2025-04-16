/*
 * Juweler.cs
 * 
 * Author: Jonas Hammer
 * Description: Ein Interactable-Objekt, das den Juwelier-Shop öffnet, wenn der Spieler mit ihm interagiert.
 * Last Edited: 16. April 2025
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Juweler : Interactable
{
    // Referenz auf das Juweler-Canvas, das den Shop darstellt
    public GameObject jewelerCanvas;

    // Überschreibt die Interact-Methode der Basisklasse Interactable
    public override void Interact()
    {
        base.Interact(); // Ruft die Interact-Methode der Basisklasse auf
        OpenJewelerShop(); // Öffnet den Juwelier-Shop
    }

    // Methode, die den Juwelier-Shop öffnet, indem sie das Canvas aktiviert
    void OpenJewelerShop()
    {
        // Überprüft, ob das jewelerCanvas zugewiesen wurde
        if (jewelerCanvas != null)
        {
            jewelerCanvas.SetActive(true); // Aktiviert das Juwelier-Shop-Canvas
            Debug.Log("Opening Jeweler Shop..."); // Loggt eine Nachricht in die Konsole
        }
        else
        {
            // Wenn das jewelerCanvas nicht zugewiesen wurde, loggt eine Warnung
            Debug.LogWarning("Jeweler Canvas is not assigned!");
        }
    }
}
