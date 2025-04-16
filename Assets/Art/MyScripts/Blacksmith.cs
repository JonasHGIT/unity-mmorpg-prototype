/*
 * Blacksmith.cs
 * 
 * Author: Jonas Hammer
 * Description: Ermöglicht Interaktion mit einem Schmied im Spiel, bei der ein UI-Canvas geöffnet wird.
 * Last Edited: 16. April 2025
 * 
 * Hinweise:
 * - Setzt voraus, dass das Blacksmith-Canvas im Inspector zugewiesen wurde.
 * - Erbt von der Klasse 'Interactable'.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blacksmith : Interactable
{
    [Header("UI References")]
    [Tooltip("Das Canvas-Fenster, das beim Interagieren mit dem Schmied geöffnet wird.")]
    public GameObject blacksmithCanvas;

    [Tooltip("Ein separates Shop-UI, welches nach dem ersten Interagieren geöffnet werden kann.")]
    public GameObject blacksmithShop;

    /// <summary>
    /// Wird aufgerufen, wenn der Spieler mit dem Schmied interagiert.
    /// </summary>
    public override void Interact()
    {
        base.Interact();
        OpenBlacksmithCanvas();
    }

    /// <summary>
    /// Öffnet das Canvas für den Schmied.
    /// </summary>
    private void OpenBlacksmithCanvas()
    {
        if (blacksmithCanvas != null)
        {
            blacksmithCanvas.SetActive(true);
            Debug.Log("Opening Blacksmith Canvas...");
        }
        else
        {
            Debug.LogWarning("Blacksmith Canvas is not assigned!");
        }
    }

    /// <summary>
    /// Öffnet ein zusätzliches Shop-Fenster.
    /// </summary>
    private void OpenBlacksmithShop()
    {
        if (blacksmithShop != null)
        {
            blacksmithShop.SetActive(true);
            Debug.Log("Opening Blacksmith Shop...");
        }
        else
        {
            Debug.LogWarning("Blacksmith Shop is not assigned!");
        }
    }
}
