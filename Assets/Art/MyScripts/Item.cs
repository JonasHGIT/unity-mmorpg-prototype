using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable object/GameItem")]
public class GameItem : ScriptableObject
{
    [Header("3D Model")]
    public GameObject model3D = null;

    [Header("Only gameplay")]
    new public string name = "New Item";
    public Sprite image = null;
    public bool isEquipable = false;

    public GameItemType type;
    public ActionType actionType;

    [Header("Rarity")]
    public Rarity rarity;

    [Header("Only UI")]
    public bool stackable = true;

    [Header("Base Values")]
    public int itemLevel = 1;
    public int inventoryItemLevel;
    public float health;
    public float mana;
    public float minDamage;
    public float maxDamage;
    public float attackSpeed;
    public float armorValue;
    public float critChance;
    public float critDamageBonus;
    public float moveSpeed;
    public int sellValue; // Verkaufswert als int

    [Header("Enchantments")]
    public List<Enchantment> enchantments = new List<Enchantment>();

    void OnEnable()
    {
        // Wenn der Typ Potion ist, setze die Stapelbarkeit auf true und die Rarity auf Common
        if (type == GameItemType.Potion)
        {
            stackable = true;
            rarity = Rarity.Common;
        }
    }

    public void ApplyBaseValues()
    {
        // Setze alle Werte auf 0, bevor Basiswerte und Verzauberungen angewendet werden
        ResetValues();

        // Erzwinge Common-Rarity und Stapelbarkeit für Potions
        if (type == GameItemType.Potion)
        {
            rarity = Rarity.Common;
            stackable = true;
        }

        // Wende Basiswerte basierend auf dem Gegenstandstyp an
        SetBaseValues();

        // Wende Verzauberungen nur an, wenn der Gegenstand nicht vom Typ Potion ist
        if (type != GameItemType.Potion)
        {
            ApplyEnchantmentsBasedOnRarity();
        }
    }

    private void ResetValues()
    {
        //itemLevel = 1;
        //inventoryItemLevel = 0;
        health = 0f;
        mana = 0f;
        minDamage = 0f;
        maxDamage = 0f;
        attackSpeed = 0f;
        armorValue = 0f;
        critChance = 0;
        critDamageBonus = 0f;
        moveSpeed = 0f;
    }

    private void SetBaseValues()
    {
        switch (type)
        {
            case GameItemType.SingleHandWeapon:
                minDamage = 8f;
                maxDamage = 11f;
                attackSpeed = 0.2f;
                break;
            case GameItemType.DoubleHandWeapon:
                minDamage = 15f;
                maxDamage = 25f;
                attackSpeed = 0.1f;
                break;
            case GameItemType.Amulette:
                health = 15f;
                critChance = 5;
                break;
            case GameItemType.Cape:
                armorValue = 2f;
                health = 12f;
                break;
            case GameItemType.Belt:
                armorValue = 4f;
                mana = 2f;
                break;
            case GameItemType.Ring:
                critChance = 1;
                critDamageBonus = 0.1f;
                break;
            case GameItemType.Gloves:
                attackSpeed = 0.1f;
                critChance = 1;
                break;
            case GameItemType.Hat:
                health = 11f;
                mana = 2f;
                break;
            case GameItemType.Shoulders:
                armorValue = 3f;
                health = 12f;
                break;
            case GameItemType.Torso:
                armorValue = 3f;
                health = 14f;
                break;
            case GameItemType.Trousers:
                armorValue = 5f;
                health = 8f;
                break;
            case GameItemType.Shoes:
                moveSpeed = 0.1f;
                health = 15f;
                break;
            case GameItemType.OffHand:
                minDamage = 5f;
                maxDamage = 7f;
                break;
            case GameItemType.Potion:
                health = 100f; // Health Potion
                break;
            case GameItemType.Gem:
                critChance = 0.8f;
                critDamageBonus = 0.2f;
                break;
            default:
                Debug.LogWarning("Unknown GameItemType: " + type);
                break;
        }
        // Scale base values based on inventory item level
        ScaleBaseValues();
    }

    private void ScaleBaseValues()
    {
        float scalingFactor = 1 + ((inventoryItemLevel - 1) * 0.05f); // 5% increase per item level starting from level 2

        health *= scalingFactor;
        mana *= scalingFactor;
        minDamage *= scalingFactor;
        maxDamage *= scalingFactor;
        attackSpeed *= scalingFactor;
        armorValue *= scalingFactor;
        critChance = Mathf.Min(100, Mathf.RoundToInt(critChance * scalingFactor)); // Clamp crit chance to 100%
        critDamageBonus *= scalingFactor;
        moveSpeed *= scalingFactor;
    }


    private void ApplyEnchantmentsBasedOnRarity()
    {
        int enchantmentCount = GetEnchantmentCountForRarity(rarity);
        enchantments.Clear();

        for (int i = 0; i < enchantmentCount; i++)
        {
            Enchantment enchantment = CreateEnchantment();
            enchantments.Add(enchantment);
        }

        foreach (var enchantment in enchantments)
        {
            ApplyEnchantment(enchantment);
            Debug.Log($"Applied Enchantment: {enchantment.name}");
        }
    }

    private void ApplyEnchantment(Enchantment enchantment)
    {
        health += enchantment.healthBonus;
        mana += enchantment.manaBonus;
        minDamage += enchantment.minDamageBonus;
        maxDamage += enchantment.maxDamageBonus;
        attackSpeed += enchantment.attackSpeedBonus;
        armorValue += enchantment.armorValueBonus;
        critChance += enchantment.critChanceBonus;
        critDamageBonus += enchantment.critDamageBonusBonus;
        moveSpeed += enchantment.moveSpeedBonus;

        // Scale enchantments based on inventory item level
        ScaleEnchantments();
    }

    private void ScaleEnchantments()
    {
        float scalingFactor = 1 + ((inventoryItemLevel - 1) * 0.03f); // 3% increase per item level starting from level 2

        for (int i = 0; i < enchantments.Count; i++)
        {
            enchantments[i].healthBonus *= scalingFactor;
            enchantments[i].manaBonus *= scalingFactor;
            enchantments[i].minDamageBonus *= scalingFactor;
            enchantments[i].maxDamageBonus *= scalingFactor;
            enchantments[i].attackSpeedBonus *= scalingFactor;
            enchantments[i].armorValueBonus *= scalingFactor;
            enchantments[i].critChanceBonus = Mathf.Min(100, Mathf.RoundToInt(enchantments[i].critChanceBonus * scalingFactor)); // Clamp crit chance to 100%
            enchantments[i].critDamageBonusBonus *= scalingFactor;
            enchantments[i].moveSpeedBonus *= scalingFactor;
        }
    }


    private int GetEnchantmentCountForRarity(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:
                return 0;
            case Rarity.Uncommon:
                return 1;
            case Rarity.Rare:
                return 2;
            case Rarity.Epic:
                return 3;
            case Rarity.Legendary:
                return 4;
            default:
                return 0;
        }
    }

    private Enchantment CreateEnchantment()
    {
        List<Enchantment> possibleEnchantments = new List<Enchantment>
        {
            new Enchantment("Health Boost", Random.Range(4f, 8f), 0f, 0f, 0f, 0f, 0f, 0, 0f, 0f),
            new Enchantment("Mana Boost", 0f, Random.Range(2f, 4f), 0f, 0f, 0f, 0f, 0, 0f, 0f),
            new Enchantment("Min Damage Boost", 0f, 0f, Random.Range(1f, 5f), 0f, 0f, 0f, 0, 0f, 0f),
            new Enchantment("Max Damage Boost", 0f, 0f, 0f, Random.Range(1f, 5f), 0f, 0f, 0, 0f, 0f),
            new Enchantment("Attack Speed Boost", 0f, 0f, 0f, 0f, Random.Range(0.05f, 0.2f), 0f, 0, 0f, 0f),
            new Enchantment("Armor Boost", 0f, 0f, 0f, 0f, 0f, Random.Range(1f, 5f), 0, 0f, 0f),
            new Enchantment("Crit Chance Boost", 0f, 0f, 0f, 0f, 0f, 0f, Random.Range(0.2f, 1.4f), 0f, 0f),
            new Enchantment("Crit Damage Boost", 0f, 0f, 0f, 0f, 0f, 0f, 0, Random.Range(0.05f, 0.2f), 0f),
            new Enchantment("Move Speed Boost", 0f, 0f, 0f, 0f, 0f, 0f, 0, 0f, Random.Range(0.05f, 0.2f))
        };

        return possibleEnchantments[Random.Range(0, possibleEnchantments.Count)];
    }

    public int CalculateSellValue()
    {
        float baseValue = Random.Range(5f, 15f); // Basiswert zwischen 5 und 15
        float rarityMultiplier = GetRarityMultiplier(rarity);
        float levelMultiplier = 1 + (itemLevel * 0.5f); // 50% Erhöhung pro Item-Level

        // Berechne den Verkaufswert und speichere ihn als int
        sellValue = Mathf.FloorToInt(baseValue * rarityMultiplier * levelMultiplier);
        return sellValue; // Gib den Verkaufswert zurück
    }

    private float GetRarityMultiplier(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:
                return 3f;
            case Rarity.Uncommon:
                return 10f;
            case Rarity.Rare:
                return 20f;
            case Rarity.Epic:
                return 30f;
            case Rarity.Legendary:
                return 50f;
            default:
                return 3f;
        }
    }

    public GameItem Clone()
    {
        GameItem clone = ScriptableObject.CreateInstance<GameItem>();
        clone.itemLevel = this.itemLevel;
        clone.inventoryItemLevel = this.inventoryItemLevel;
        clone.name = this.name;
        clone.model3D = this.model3D;
        clone.image = this.image;
        clone.isEquipable = this.isEquipable;
        clone.type = this.type;
        clone.actionType = this.actionType;
        clone.rarity = this.rarity;
        clone.stackable = this.stackable;
        clone.health = this.health;
        clone.mana = this.mana;
        clone.minDamage = this.minDamage;
        clone.maxDamage = this.maxDamage;
        clone.attackSpeed = this.attackSpeed;
        clone.armorValue = this.armorValue;
        clone.critChance = this.critChance;
        clone.critDamageBonus = this.critDamageBonus;
        clone.moveSpeed = this.moveSpeed;
        clone.enchantments = new List<Enchantment>();
        foreach (var enchantment in this.enchantments)
        {
            clone.enchantments.Add(enchantment.Clone());
        }
        return clone;
    }
}

[System.Serializable]
public class Enchantment
{
    public string name;
    public float healthBonus;
    public float manaBonus;
    public float minDamageBonus;
    public float maxDamageBonus;
    public float attackSpeedBonus;
    public float armorValueBonus;
    public float critChanceBonus;
    public float critDamageBonusBonus;
    public float moveSpeedBonus;

    public Enchantment(string name, float healthBonus, float manaBonus, float minDamageBonus, float maxDamageBonus, float attackSpeedBonus, float armorValueBonus, float critChanceBonus, float critDamageBonusBonus, float moveSpeedBonus)
    {
        this.name = name;
        this.healthBonus = healthBonus;
        this.manaBonus = manaBonus;
        this.minDamageBonus = minDamageBonus;
        this.maxDamageBonus = maxDamageBonus;
        this.attackSpeedBonus = attackSpeedBonus;
        this.armorValueBonus = armorValueBonus;
        this.critChanceBonus = critChanceBonus;
        this.critDamageBonusBonus = critDamageBonusBonus;
        this.moveSpeedBonus = moveSpeedBonus;
    }

    public Enchantment Clone()
    {
        return new Enchantment(name, healthBonus, manaBonus, minDamageBonus, maxDamageBonus, attackSpeedBonus, armorValueBonus, critChanceBonus, critDamageBonusBonus, moveSpeedBonus);
    }
}

public enum GameItemType
{
    SingleHandWeapon,
    DoubleHandWeapon,
    Amulette,
    Cape,
    Belt,
    Ring,
    Gloves,
    Hat,
    Shoulders,
    Torso,
    Trousers,
    Shoes,
    OffHand,
    Potion,
    Gem
}

public enum ActionType
{
    Use,
    Equip,
    Insert
}

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}
