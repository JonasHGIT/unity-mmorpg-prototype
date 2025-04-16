/*
 * ------------------------------------------------------------------------------
 * Script:       KoboldSafe.cs
 * Author:       Jonas Hammer
 * Created:      [Erstellungsdatum]
 * Last Edited:  16. April 2025
 * Description:  Dieses Skript sorgt dafür, dass der KoboldSafe-Interaktionspunkt im Spiel 
 *               korrekt funktioniert. Es ermöglicht dem Spieler, mit einem KoboldSafe-Objekt 
 *               zu interagieren, wodurch ein spezielles UI-Canvas (KoboldSafeCanvas) angezeigt wird.
 *               Zusätzlich wird das Inventar-Canvas aktiviert.
 *
 * Hauptfunktionen:
 * - Ermöglicht dem Spieler, mit dem KoboldSafe zu interagieren.
 * - Öffnet das KoboldSafe-UI und das Inventar-UI, wenn der Spieler mit dem Safe interagiert.
 * - Stellt sicher, dass die zugehörigen UI-Elemente nur aktiviert werden, wenn das Canvas zugewiesen wurde.
 *
 * UI-Elemente:
 * - koboldSafeCanvas (GameObject): Das Canvas, das beim Öffnen des KoboldSafe angezeigt wird.
 * - inventoryCanvas (GameObject): Das Canvas, das das Inventar des Spielers darstellt und bei Interaktion ebenfalls angezeigt wird.
 *
 * Abhängigkeiten:
 * - Interactable.cs (Basis-Klasse für Interaktionen)
 *
 * Hinweise:
 * - Das Skript geht davon aus, dass beide Canvas-Objekte korrekt im Inspector zugewiesen sind.
 * - Wenn das `koboldSafeCanvas` nicht zugewiesen ist, gibt es eine Warnung im Debug-Log.
 * ------------------------------------------------------------------------------
 */


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
