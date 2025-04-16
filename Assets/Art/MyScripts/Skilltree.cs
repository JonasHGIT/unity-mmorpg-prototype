/*
 * ------------------------------------------------------------------------------
 * Script:       Skilltree.cs
 * Author:       Jonas Hammer
 * Created:      [Erstellungsdatum]
 * Last Edited:  16. April 2025
 * Description:  Dieses Skript verwaltet die Anzeige und Aktualisierung der UI-Elemente 
 *               für das Skilltree-System des Spielers. Es zeigt die aktuellen Skillpunkte,
 *               das Level des Spielers sowie den Fortschritt in der XP-Leiste an.
 *
 * Hauptfunktionen:
 * - Anzeige der Skillpunkte und des aktuellen Levels des Spielers.
 * - Fortschrittsanzeige in der XP-Leiste, die den Fortschritt von Level 1 bis 100 darstellt.
 * - Aktualisierung der UI-Elemente bei einem Level-Up oder beim Erhalt von Skillpunkten.
 *
 * UI-Elemente:
 * - xpBar (Image): Eine XP-Leiste, die den Fortschritt des Spielers anzeigt.
 * - skillPointText (TMP_Text): Ein Textfeld zur Anzeige der aktuellen Skillpunkte.
 * - levelText (TMP_Text): Ein Textfeld zur Anzeige des aktuellen Levels des Spielers.
 *
 * Abhängigkeiten:
 * - PlayerController (Referenz auf den Spieler und dessen Level / Skillpunkte)
 * - TextMeshPro (Für die Verwendung von TMP_Text zur Darstellung der Texte)
 * - Unity UI (Für die Verwendung von Image und Text-UI-Komponenten)
 *
 * Hinweise:
 * - Die XP-Leiste wird basierend auf dem aktuellen Level des Spielers aktualisiert.
 * - Skillpunkte und Level des Spielers werden dynamisch im UI angezeigt.
 * ------------------------------------------------------------------------------
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Skilltree : MonoBehaviour
{
    [Header("UI References")]
    public Image xpBar; // XP-Bar als Image mit Filled-Attribut für den Fortschritt von Level 1 bis Level 100
    public TMP_Text skillPointText; // Textfeld zur Anzeige der aktuellen Skillpunkte
    public TMP_Text levelText; // Text zur Anzeige des aktuellen Levels

    public PlayerController playerController;

    private void Start()
    {
        UpdateUI(); // Initialisiere die UI mit den aktuellen Werten des Spielers
    }

    // Diese Methode aktualisiert die Skillpoints und die XP-Bar
    public void UpdateUI()
    {
        if (playerController != null)
        {
            Debug.Log("Updating UI...");
            skillPointText.text = "Skill Points: " + playerController.skillPoint.ToString();
            float xpProgress = playerController.playerLevel / 100f;
            xpBar.fillAmount = xpProgress;
            levelText.text = "Level: " + playerController.playerLevel.ToString();

            Debug.Log("Level: " + playerController.playerLevel);
            Debug.Log("Skill Points: " + playerController.skillPoint);
            Debug.Log("XP Progress: " + xpProgress);
        }
        else
        {
            Debug.LogError("PlayerController not found in Skilltree.");
        }
    }

    // Aufrufbar, wenn ein Level-Up stattfindet
    public void OnLevelUp()
    {
        UpdateUI(); // Aktualisiere die Anzeige bei einem Level-Up
    }
}
