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