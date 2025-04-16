/*
 * ------------------------------------------------------------------------------
 * Script:       Manabar.cs
 * Author:       Jonas Hammer
 * Created:      [Erstellungsdatum]
 * Last Edited:  16. April 2025
 * Description:  Dieses Skript verwaltet die Anzeige und Regeneration des Manabalkens 
 *               für den Spieler. Es sorgt dafür, dass die Mana-Anzeige (über eine UI-Bar)
 *               ständig aktualisiert wird und stellt sicher, dass das Mana des Spielers
 *               nicht über das Maximum hinaus geht und regeneriert wird, wenn es unter
 *               dem Maximalwert liegt.
 *
 * Hauptfunktionen:
 * - Aktualisierung des Mana-Werts des Spielers aus dem PlayerController
 * - Regeneration des Mana-Werts basierend auf einer festgelegten Geschwindigkeit
 * - Sicherstellung, dass der Mana-Wert zwischen 0 und dem Maximalwert bleibt
 * - Aktualisierung der UI, um den aktuellen Mana-Wert und das Maximum anzuzeigen
 *
 * UI-Elemente:
 * - _manaBarFill (Image): UI-Komponente, die den aktuellen Mana-Wert visuell darstellt
 * - _manaText (TMP_Text): Textkomponente, die den aktuellen Mana-Wert in der Form "XMP / YMP" anzeigt
 *
 * Abhängigkeiten:
 * - PlayerController.cs (für den Zugriff auf die aktuellen Mana-Werte des Spielers)
 *
 * Hinweise:
 * - Das Skript geht davon aus, dass die PlayerController-Komponente auf einem GameObject namens "Player_Capsule_Mesh" vorhanden ist.
 * - Die Mana-Regeneration wird kontinuierlich im Update-Loop durchgeführt und ist an die Zeit (Time.deltaTime) gebunden.
 * - Es wird sichergestellt, dass der Mana-Wert niemals über das Maximum hinausgeht oder unter 0 fällt.
 * ------------------------------------------------------------------------------
 */


using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Manabar : MonoBehaviour
{
    private float _maxMana;
    private float _currentMana;

    [SerializeField] private float _manaRegeneration;
    [SerializeField] private Image _manaBarFill;
    [SerializeField] private TMP_Text _manaText;

    private PlayerController playerController; // Reference to the PlayerController

    // Start is called before the first frame update
    void Start()
    {
        // Find the Player_Capsule_Mesh object and get the PlayerController component
        playerController = GameObject.Find("Player_Capsule_Mesh").GetComponent<PlayerController>();

        // Initialize mana values from the PlayerController
        _maxMana = playerController.maxMana;
        _currentMana = playerController.currentMana;

        // Update the mana bar UI initially
        UpdateManaBar();

    }

    // Update is called once per frame
    void Update()
    {
        // Update current mana from the PlayerController
        _currentMana = playerController.currentMana;
        
        // Optionally regenerate mana
        if (_currentMana < _maxMana)
        {
            _currentMana += playerController.manaRegeneration * Time.deltaTime;
            _currentMana = Mathf.Clamp(_currentMana, 0, _maxMana);  // Ensure mana does not exceed max or go below 0

            // Apply the regenerated mana back to the PlayerController
            playerController.currentMana = _currentMana;
        }

        // Update the mana bar UI
        UpdateManaBar();
    }

    public void UpdateManaBar()
    {
        float targetFillAmount = _currentMana / _maxMana;
        _manaBarFill.fillAmount = targetFillAmount;

        // Update the mana text
        _manaText.text = $"{(int)_currentMana}MP / {(int)_maxMana}MP";
    }
}
