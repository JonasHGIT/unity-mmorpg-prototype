using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    // Referenzen zu den Canvas GameObjects
    public GameObject canvasPlayerStats;
    public GameObject canvasShop;
    public GameObject canvasSkilltree;
    public GameObject canvasInventory;
    public GameObject miniMap;
    public GameObject canvasOptions;

    void Update()
    {
        // Überprüfen, ob die Taste "C" gedrückt wird
        if (Input.GetKeyDown(KeyCode.C))
        {
            // Canvas 1 aktivieren/deaktivieren
            canvasPlayerStats.SetActive(!canvasPlayerStats.activeSelf); // Umschalten des Zustands
        }

        // Überprüfen, ob die Taste "T" gedrückt wird
        if (Input.GetKeyDown(KeyCode.T))
        {
            // Canvas 2 aktivieren/deaktivieren
            canvasShop.SetActive(!canvasShop.activeSelf); // Umschalten des Zustands
            canvasInventory.SetActive(!canvasInventory.activeSelf); // Umschalten des Zustands
        }

        // Überprüfen, ob die Taste "S" gedrückt wird
        if (Input.GetKeyDown(KeyCode.S))
        {
            // Canvas 3 aktivieren/deaktivieren
            canvasSkilltree.SetActive(!canvasSkilltree.activeSelf); // Umschalten des Zustands
        }

        // Überprüfen, ob die Taste "I" gedrückt wird
        if (Input.GetKeyDown(KeyCode.I))
        {
            // Canvas 4 aktivieren/deaktivieren
            canvasInventory.SetActive(!canvasInventory.activeSelf); // Umschalten des Zustands
        }

        // Überprüfen, ob die Taste "M" gedrückt wird
        if (Input.GetKeyDown(KeyCode.M))
        {
            // Canvas 5 aktivieren/deaktivieren
            miniMap.SetActive(!miniMap.activeSelf); // Umschalten des Zustands
        }

        // Überprüfen, ob die Taste "Esc" gedrückt wird
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Canvas 6 aktivieren/deaktivieren
            canvasOptions.SetActive(!canvasOptions.activeSelf); // Umschalten des Zustands
        }
    }
}

