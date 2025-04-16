/*
 * ------------------------------------------------------------------------------
 * Script:       QuestGiver.cs
 * Author:       Jonas Hammer
 * Created:      [Erstellungsdatum]
 * Last Edited:  16. April 2025
 * Description:  Dieses Skript ermöglicht es einem NPC, dem Spieler Quests zu vergeben.
 *               Der Spieler kann mit dem NPC interagieren, um eine Liste von Quests 
 *               anzuzeigen, aus denen er wählen kann. Jede Quest wird durch einen Button 
 *               repräsentiert, und eine detaillierte Beschreibung der ausgewählten Quest 
 *               wird angezeigt.
 *
 * Hauptfunktionen:
 * - Zeigt Quests an, die der NPC dem Spieler anbieten kann.
 * - Dynamische Erstellung von UI-Buttons für jede Quest.
 * - Anzeige der Quest-Beschreibungen und Bereitstellung der Möglichkeit, die Quests zu schließen.
 * - Automatische Positionierung der Buttons mit festen Abständen und manuellen Positionen.
 *
 * UI-Elemente:
 * - questGiverCanvas (GameObject): Das Canvas, das die Quest-Auswahl und Beschreibung enthält.
 * - questDescriptionText (TMP_Text): Das Textfeld zur Anzeige der Questbeschreibung.
 * - buttonPrefab (GameObject): Ein Prefab, das als Vorlage für die Quest-Buttons dient.
 * - buttonContainer (Transform): Der Container, in dem die Quest-Buttons erscheinen.
 * - closeButtonPrefab (GameObject): Der Button, der das Quest-Giver-Canvas schließt.
 *
 * Abhängigkeiten:
 * - Interactable.cs (Basis-Klasse für Interaktionen)
 * - TextMeshPro (Für die Verwendung von TMP_Text in der UI)
 * - Unity UI (Für die Erstellung und Handhabung von Buttons)
 *
 * Hinweise:
 * - Stellen Sie sicher, dass alle UI-Elemente korrekt im Inspector zugewiesen sind.
 * - Es gibt feste Positionen für die ersten 4 Buttons. Weitere Buttons werden automatisch positioniert.
 * ------------------------------------------------------------------------------
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Für die Verwendung von TextMeshPro
using UnityEngine.UI; // Für die Verwendung von UI-Buttons

public class QuestGiver : Interactable
{
    // Liste von Quests, die dieser NPC geben kann
    public List<Quest> quests = new List<Quest>();

    // Referenz auf das QuestGiver-Canvas
    public GameObject questGiverCanvas;

    // Referenzen zu den TextMeshPro-Feldern für die Quest-Beschreibung
    public TMP_Text questDescriptionText;  // Textfeld für die Quest-Beschreibung

    // Referenz auf das Button-Template und den Layout-Container, wo die Buttons erscheinen sollen
    public GameObject buttonPrefab;  // Button-Template für jeden Quest
    public Transform buttonContainer;  // Der Container, in dem die Buttons platziert werden

    // Referenz auf das Schließen-Button-Template
    public GameObject closeButtonPrefab; // Button-Prefab für den Schließen-Button

    // Manuell festgelegte Positionen für die ersten vier Buttons
    public Vector3[] manualButtonPositions = new Vector3[4]; // Array für manuelle Positionen der ersten 4 Buttons
    public float buttonSpacing = 100f;  // Fester Abstand zwischen den weiteren Buttons

    public override void Interact()
    {
        base.Interact();
        ShowQuestOptions();
    }

    // Methode zum Anzeigen der Quests
    void ShowQuestOptions()
    {
        if (questGiverCanvas != null)
        {
            questGiverCanvas.SetActive(true); // Aktiviert das Canvas

            if (quests != null && quests.Count > 0)
            {
                // Vorhandene Quest-Buttons löschen, andere Child-Elemente ignorieren
                foreach (Transform child in buttonContainer)
                {
                    if (child.GetComponent<Button>() != null) // Überprüft, ob das Kind eine Button-Komponente hat
                    {
                        Destroy(child.gameObject);
                    }
                }

                // Erzeuge Buttons für die Quests
                for (int i = 0; i < quests.Count; i++)
                {
                    GameObject questButton = Instantiate(buttonPrefab, buttonContainer);
                    TMP_Text buttonText = questButton.GetComponentInChildren<TMP_Text>();
                    buttonText.text = quests[i].questName;

                    // Setze die Position des Buttons je nach Questindex
                    RectTransform buttonRect = questButton.GetComponent<RectTransform>();

                    if (i < 4) // Manuelle Position für die ersten vier Buttons
                    {
                        buttonRect.localPosition = manualButtonPositions[i];
                    }
                    else // Automatische Positionierung für die weiteren Buttons
                    {
                        float newY = manualButtonPositions[3].y - buttonSpacing * (i - 3); // Abstand nach den ersten 4
                        buttonRect.localPosition = new Vector3(manualButtonPositions[3].x, newY, manualButtonPositions[3].z);
                    }

                    // Lokale Kopie von 'i' erstellen, um den richtigen Index in den Listener zu bringen
                    int questIndex = i;
                    questButton.GetComponent<Button>().onClick.AddListener(() => ShowQuestDescription(quests[questIndex]));
                }

                // Schließen-Button am Ende der Liste hinzufügen
                AddCloseButton(quests.Count);
            }
            else
            {
                Debug.Log("No quests available.");
            }
        }
        else
        {
            Debug.LogWarning("Quest Giver Canvas is not assigned!");
        }
    }

    // Methode zum Hinzufügen des Schließen-Buttons
    void AddCloseButton(int questCount)
    {
        if (closeButtonPrefab != null)
        {
            GameObject closeButton = Instantiate(closeButtonPrefab, buttonContainer);
            RectTransform buttonRect = closeButton.GetComponent<RectTransform>();

            if (questCount < 4)
            {
                // Positioniere den Schließen-Button unter dem letzten manuell platzierten Quest-Button
                buttonRect.localPosition = manualButtonPositions[questCount];
            }
            else
            {
                // Positioniere den Schließen-Button unter dem letzten automatisch platzierten Quest-Button
                float newY = manualButtonPositions[3].y - buttonSpacing * (questCount - 3);
                buttonRect.localPosition = new Vector3(manualButtonPositions[3].x, newY, manualButtonPositions[3].z);
            }

            // Füge den Event Listener hinzu, der das Canvas schließt
            closeButton.GetComponent<Button>().onClick.AddListener(() => questGiverCanvas.SetActive(false));
        }
        else
        {
            Debug.LogWarning("Close Button Prefab is not assigned!");
        }
    }

    // Methode zum Anzeigen der Quest-Beschreibung
    void ShowQuestDescription(Quest quest)
    {
        if (questDescriptionText != null)
        {
            questDescriptionText.text = quest.questDescription;
        }
        else
        {
            Debug.LogWarning("Quest description text is not assigned!");
        }
    }
}

// Quest-Klasse erweitert, um mehrere Quests zu unterstützen
[System.Serializable]
public class Quest
{
    public string questName;
    public string questDescription;
}
