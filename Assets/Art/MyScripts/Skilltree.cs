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
