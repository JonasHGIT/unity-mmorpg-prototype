/*
 * CanvasController.cs
 *
 * Author: Jonas Hammer
 * Description: Steuerung und Umschalten verschiedener UI-Canvas-Elemente per Tastendruck.
 *              Ermöglicht z.B. das Öffnen/Schließen von Inventar, Shop, Skilltree usw.
 * Last Edited: 16. April 2025
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    [Header("Canvas References")]
    public GameObject canvasPlayerStats;    // C-Taste
    public GameObject canvasShop;           // T-Taste (zusammen mit Inventory)
    public GameObject canvasSkilltree;      // S-Taste
    public GameObject canvasInventory;      // I- oder T-Taste
    public GameObject miniMap;              // M-Taste
    public GameObject canvasOptions;        // Escape-Taste

    void Update()
    {
        // Toggle PlayerStats Canvas (Taste C)
        if (Input.GetKeyDown(KeyCode.C))
        {
            canvasPlayerStats.SetActive(!canvasPlayerStats.activeSelf);
        }

        // Toggle Shop und Inventory gleichzeitig (Taste T)
        if (Input.GetKeyDown(KeyCode.T))
        {
            canvasShop.SetActive(!canvasShop.activeSelf);
            canvasInventory.SetActive(!canvasInventory.activeSelf);
        }

        // Toggle Skilltree Canvas (Taste S)
        if (Input.GetKeyDown(KeyCode.S))
        {
            canvasSkilltree.SetActive(!canvasSkilltree.activeSelf);
        }

        // Toggle Inventory unabhängig (Taste I)
        if (Input.GetKeyDown(KeyCode.I))
        {
            canvasInventory.SetActive(!canvasInventory.activeSelf);
        }

        // Toggle MiniMap (Taste M)
        if (Input.GetKeyDown(KeyCode.M))
        {
            miniMap.SetActive(!miniMap.activeSelf);
        }

        // Toggle Options (Taste ESC)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            canvasOptions.SetActive(!canvasOptions.activeSelf);
        }
    }
}
