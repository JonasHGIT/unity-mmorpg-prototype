/*
 * ------------------------------------------------------------------------------
 * Script:       PlayerManager.cs
 * Author:       Jonas Hammer
 * Created:      [Erstellungsdatum]
 * Last Edited:  16. April 2025
 * Description:  Dieses Skript verwaltet den Zugriff auf den Spieler in einer Singleton-Struktur.
 *               Es ermöglicht anderen Skripten den Zugriff auf das Spielerobjekt über die statische Instanz.
 *
 * Hauptfunktionen:
 * - Singleton-Designmuster zur Verwaltung einer einzigen Instanz des PlayerManagers
 * - Ermöglicht anderen Komponenten den Zugriff auf das Spielerobjekt
 *
 * Unterstützt:
 * - Stellt eine zentrale Steuerung für den Spieler zur Verfügung, um dessen Referenz global zu verwenden
 * - Kann in größeren Systemen verwendet werden, die auf den Spieler zugreifen müssen (z.B. Health, Inventory)
 *
 * Dependencies:
 * - Keine externen Abhängigkeiten erforderlich
 *
 * Hinweise:
 * - Dieses Skript sollte auf einem GameObject in der Szene vorhanden sein, um den PlayerManager
 *   korrekt initialisieren zu können
 * ------------------------------------------------------------------------------
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Singleton

    public static PlayerManager instance;

    void Awake()
    {
        instance = this;
    }
    #endregion

    public GameObject player;

}
