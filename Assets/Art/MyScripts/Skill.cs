/*
 * ------------------------------------------------------------------------------
 * Script:       Skill.cs
 * Author:       Jonas Hammer
 * Created:      [Erstellungsdatum]
 * Last Edited:  16. April 2025
 * Description:  Dieses Skript definiert ein `Skill`-Objekt, das die Eigenschaften und Parameter eines Spielzaubers oder Angriffs im Spiel speichert.
 *               Es nutzt ScriptableObjects, um Fähigkeiten zu erstellen, die vom Spieler genutzt werden können.
 *
 * Hauptfunktionen:
 * - Speicherung der grundlegenden Attribute einer Fähigkeit (z.B. Name, Mana-Kosten, Schadenstyp, Reichweite).
 * - Ermöglicht die Verwendung von verschiedenen Schadensarten wie physisch, Feuer, Eis, Blitz und Gift.
 * - Unterstützung von mehreren Zaubern wie Feuerball, Teleportation und Kettenblitzen.
 * - Integration von Bildreferenzen für die Darstellung der Fähigkeiten im UI.
 *
 * Abhängigkeiten:
 * - `SkillName`: Enum, das verschiedene Fähigkeitsnamen definiert.
 * - `DamageType`: Enum, das verschiedene Schadensarten für Fähigkeiten definiert.
 * 
 * UI-Elemente:
 * - image (Sprite): Ein Bild, das die Fähigkeit visuell darstellt, z.B. ein Icon.
 *
 * Wichtige Hinweise:
 * - Diese Fähigkeiten werden als ScriptableObjects erstellt und können so einfach im Unity-Editor erstellt und verwaltet werden.
 * - Mana-Kosten und Cooldown sind für das Gameplay relevant und steuern die Spielmechanik.
 * ------------------------------------------------------------------------------
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Scriptable object/Skill")]
public class Skill : ScriptableObject
{
    [Header("Only gameplay")]
    public SkillName name; // oder doch lieber einfach als string?
    public float manaCost;
    public DamageType damageType;
    public float damageMultiplier;
    public float attackRange;
    public float coolDown;

    [Header("Both")]
    public Sprite image;

}

public enum SkillName
{
    Fireball,
    Teleport,
    Manashot, // Magicshot/Arkanshot
    IceAttack,
    Lightning,
    ChainLightning,
    MeleeSlash
}

public enum DamageType
{
    PhysicDamage,
    FireDamage,
    IceDamage,
    LightningDamage,
    PoisonDamage
}