/*
 * ------------------------------------------------------------------------------
 * Script:       Interactable.cs
 * Author:       Jonas Hammer
 * Created:      [Erstellungsdatum]
 * Last Edited:  16. April 2025
 * Description:  Basisklasse für alle interaktiven Objekte im Spiel. Stellt
 *               grundlegende Funktionen für Hover-Text, Interaktion und
 *               Abstandsprüfung zur Verfügung.
 *
 * Hauptfunktionen:
 * - Anzeige eines Hover-Textes bei Maus-Hover über das Objekt
 * - Virtuelle `Interact()`-Methode für spezifische Interaktionen
 * - Gizmo-Darstellung des Interaktionsradius in der Szene
 * - Coroutine zur Distanzüberprüfung und automatischen Interaktion
 *
 * Unterstützt:
 * - Erweiterung durch Vererbung (z. B. `ItemPickup`, NPC-Dialoge, Türen)
 * - Nahtlose Integration mit TextMeshPro für UI-Anzeigen
 * - Dynamische Interaktionen basierend auf Spielerposition
 *
 * Dependencies:
 * - PlayerController.cs
 * - TextMeshProUGUI (für Hover-Text-Anzeige)
 * - NavMeshAgent (zur Bewegung/Abbruch bei Interaktion)
 *
 * Hinweise:
 * - Der Name des GameObjects für Hover-Text muss „HoverText“ lauten
 * - Die `Interact()`-Methode sollte in abgeleiteten Klassen überschrieben werden
 * - Interaktionsradius kann im Inspector angepasst werden
 * ------------------------------------------------------------------------------
 */


using UnityEngine;
using TMPro;
using System.Collections;

public class Interactable : MonoBehaviour
{
    public float radius = 3f;

    public string objectName = "Object Name";

    private TextMeshProUGUI hoverText;

    public void Start()
    {
        // Referenz auf das HoverText-UI-Element suchen
        GameObject hoverTextObject = GameObject.Find("HoverText");
        if (hoverTextObject != null)
        {
            hoverText = hoverTextObject.GetComponent<TextMeshProUGUI>();
            if (hoverText != null)
            {
                hoverText.text = "";
            }
            else
            {
                Debug.LogWarning("HoverText GameObject found, but it does not have a TextMeshProUGUI component.");
            }
        }
        else
        {
            Debug.LogWarning("No GameObject named 'HoverText' found in the scene.");
        }
    }

    void OnMouseEnter()
    {
        // Setze den Text des HoverText-UI-Elements auf den Namen des Objekts
        if (hoverText != null)
        {
            hoverText.text = objectName;
            hoverText.gameObject.SetActive(true);
        }
    }

    void OnMouseExit()
    {
        // Verberge das HoverText-UI-Element, wenn der Mauszeiger das Objekt verlässt
        if (hoverText != null)
        {
            hoverText.text = "";
            hoverText.gameObject.SetActive(false);
        }
    }

    public virtual void Interact()
    {
        // This method is meant to be overwritten
        Debug.Log("Interacting");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    // Coroutine to check the distance to the player
    public IEnumerator CheckDistanceToPlayer(PlayerController player, UnityEngine.AI.NavMeshAgent agent)
    {
        while (player.focus == this)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance <= radius)
            {
                agent.ResetPath();
                Interact();
                yield break; // Exit the coroutine once the object is destroyed
            }
            yield return null;
        }
    }
}
