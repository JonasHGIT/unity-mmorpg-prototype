using System.Collections.Generic;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    [System.Serializable]
    public struct LootItem
    {
        public GameItem item;
        public float dropChance; // Wahrscheinlichkeit in Prozent
    }

    public List<LootItem> lootTable;

    [Header("Rarity VFX")]
    public GameObject commonVFX;
    public GameObject uncommonVFX;
    public GameObject rareVFX;
    public GameObject epicVFX;
    public GameObject legendaryVFX;

    [Header("Rarity Background Prefabs")]
    public GameObject commonBackgroundPrefab;
    public GameObject uncommonBackgroundPrefab;
    public GameObject rareBackgroundPrefab;
    public GameObject epicBackgroundPrefab;
    public GameObject legendaryBackgroundPrefab;

    public GameItem GetRandomLoot()
    {
        float totalChance = 0f;
        foreach (var lootItem in lootTable)
        {
            totalChance += lootItem.dropChance;
        }

        float randomValue = Random.Range(0f, totalChance);
        float cumulativeProbability = 0f;

        foreach (var lootItem in lootTable)
        {
            cumulativeProbability += lootItem.dropChance;
            if (randomValue <= cumulativeProbability)
            {
                // Weisen Sie eine Rarity zu, bevor Sie das Item zurückgeben
                lootItem.item.rarity = AssignRandomRarity();
                return lootItem.item;
            }
        }

        return null; 
    }

    private Rarity AssignRandomRarity()
    {
        float randomValue = Random.Range(0f, 100f);

        if (randomValue < 50f)
            return Rarity.Common;
        else if (randomValue < 80f)
            return Rarity.Uncommon;
        else if (randomValue < 95f)
            return Rarity.Rare;
        else if (randomValue < 99f)
            return Rarity.Epic;
        else
            return Rarity.Legendary;
    }

    public void DropLoot(Vector3 position)
    {
        // Bestimme, ob ein Item gedroppt wird, basierend auf einer 50% Chance
        if (UnityEngine.Random.value > 0.5f)  // 50% Chance, da Random.value einen Wert zwischen 0 und 1 zurückgibt
        {
            GameItem itemToDrop = GetRandomLoot();
            if (itemToDrop != null)
            {
                GameObject itemObject;
                if (itemToDrop.model3D != null)
                {
                    itemObject = Instantiate(itemToDrop.model3D, position + new Vector3(0, 0.2f, 0), Quaternion.identity);
                    itemObject.name = itemToDrop.name;
                }
                else
                {
                    // Neues GameObject für das 2D-Sprite erstellen
                    itemObject = new GameObject(itemToDrop.name);
                    itemObject.transform.position = position + new Vector3(0, 0.2f, 0);

                    // SpriteRenderer-Komponente hinzufügen und Sprite zuweisen
                    SpriteRenderer renderer = itemObject.AddComponent<SpriteRenderer>();
                    renderer.sprite = itemToDrop.image;

                    // Manuelle Größe für das Sprite festlegen
                    Vector2 desiredSize = new Vector2(1.0f, 1.0f); // Beispielgröße in Unity-Einheiten

                    // Berechnung der Skalierung basierend auf der gewünschten Größe
                    float widthScale = desiredSize.x / renderer.bounds.size.x;
                    float heightScale = desiredSize.y / renderer.bounds.size.y;
                    itemObject.transform.localScale = new Vector3(widthScale, heightScale, 1f);
                }

                Collider itemCollider = itemObject.GetComponent<Collider>();
                if (itemCollider == null)
                {
                    itemCollider = itemObject.AddComponent<BoxCollider>();
                }

                BoxCollider boxCollider = itemCollider as BoxCollider;
                if (boxCollider != null)
                {
                    boxCollider.size *= 1.5f;
                }

                Rigidbody rb = itemObject.AddComponent<Rigidbody>();
                rb.mass = 0.1f;

                // VFX zuweisen basierend auf der Seltenheit des Items
                GameObject vfxPrefab = GetVFXPrefabByRarity(itemToDrop.rarity);
                if (vfxPrefab != null)
                {
                    GameObject vfxObject = Instantiate(vfxPrefab, itemObject.transform.position, Quaternion.identity);
                    vfxObject.transform.SetParent(itemObject.transform); // VFX als Kind des Items setzen
                }

                ItemPickup itemPickup = itemObject.AddComponent<ItemPickup>();
                itemPickup.item = itemToDrop;
                itemPickup.inventoryManager = FindObjectOfType<InventoryManager>();
            }
        }
        else
        {
            Debug.Log("Kein Item gedroppt.");
        }
    }


    private GameObject GetVFXPrefabByRarity(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:
                return commonVFX;
            case Rarity.Uncommon:
                return uncommonVFX;
            case Rarity.Rare:
                return rareVFX;
            case Rarity.Epic:
                return epicVFX;
            case Rarity.Legendary:
                return legendaryVFX;
            default:
                return null;
        }
    }

    public GameObject GetBackgroundPrefabByRarity(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:
                return commonBackgroundPrefab;
            case Rarity.Uncommon:
                return uncommonBackgroundPrefab;
            case Rarity.Rare:
                return rareBackgroundPrefab;
            case Rarity.Epic:
                return epicBackgroundPrefab;
            case Rarity.Legendary:
                return legendaryBackgroundPrefab;
            default:
                return null;
        }
    }

}
