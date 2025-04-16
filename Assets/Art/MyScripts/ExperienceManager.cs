/*
 * ExperienceManager.cs
 * 
 * Author: Jonas Hammer
 * Description: Verwaltet das Erfahrungssystem des Spielers und aktualisiert die Benutzeroberfläche entsprechend.
 * Last Edited: 16. April 2025
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExperienceManager : MonoBehaviour
{
    [Header("Experience")]
    [SerializeField] AnimationCurve experienceCurve; // Kurve, die die Erfahrungspunkte für jedes Level definiert

    int currentLevel, totalExperience;  // Der aktuelle Level und die Gesamt-Erfahrung des Spielers
    int previousLevelsExperience, nextLevelsExperience;  // Erfahrung bis zum vorherigen und nächsten Level

    [Header("Interface")]
    [SerializeField] TextMeshProUGUI levelText; // Text für den aktuellen Level des Spielers
    [SerializeField] TextMeshProUGUI experienceText; // Text für die angezeigte Erfahrung des Spielers
    [SerializeField] Image experienceFill; // Das UI-Element für den Erfahrungsbalken

    void Start()
    {
        currentLevel = 1; // Setzt den Spieler-Level zu Beginn auf 1
        UpdateLevel(); // Aktualisiert die Level-Daten und die Benutzeroberfläche
    }

    // Methode, um Erfahrung hinzuzufügen
    public void AddExperience(int amount)
    {
        totalExperience += amount; // Erhöht die Gesamt-Erfahrung
        CheckForLevelUp(); // Überprüft, ob der Spieler ein Level-Up erreicht hat
        UpdateInterface(); // Aktualisiert die Benutzeroberfläche mit den neuen Werten
    }

    // Überprüft, ob der Spieler das nächste Level erreicht hat
    void CheckForLevelUp()
    {
        // Wenn die Gesamt-Erfahrung gleich oder größer ist als die benötigte Erfahrung für das nächste Level
        if (totalExperience >= nextLevelsExperience)
        {
            currentLevel++; // Erhöht den aktuellen Level
            UpdateLevel(); // Aktualisiert die Level-Daten
        }
    }

    // Methode, um das Level zu aktualisieren
    void UpdateLevel()
    {
        // Berechnet die Erfahrung für das vorherige und das nächste Level basierend auf der Erfahrungskurve
        previousLevelsExperience = (int)experienceCurve.Evaluate(currentLevel);
        nextLevelsExperience = (int)experienceCurve.Evaluate(currentLevel + 1);

        UpdateInterface(); // Aktualisiert die Benutzeroberfläche

        // Benachrichtigt den PlayerController über das neue Level
        PlayerManager.instance.player.GetComponent<PlayerController>().UpdatePlayerLevel();
    }

    // Aktualisiert die Benutzeroberfläche mit den aktuellen Erfahrungspunkten und dem Level
    void UpdateInterface()
    {
        // Berechnet den aktuellen Fortschritt in Bezug auf die Erfahrung für das Level
        int start = totalExperience - previousLevelsExperience; // Erfahrung seit dem letzten Level
        int end = nextLevelsExperience - previousLevelsExperience; // Erfahrung bis zum nächsten Level

        // Aktualisiert den Level-Text
        levelText.text = currentLevel.ToString();
        // Aktualisiert den Erfahrungstext, der die aktuelle Erfahrung und die benötigte Erfahrung für das nächste Level zeigt
        experienceText.text = start + " exp / " + end + " exp";
        // Aktualisiert den Erfahrungsbalken, um den Fortschritt zu zeigen
        experienceFill.fillAmount = (float)start / (float)end;
    }

    // Gibt den aktuellen Level zurück
    public int GetCurrentLevel()
    {
        return currentLevel;
    }
}
