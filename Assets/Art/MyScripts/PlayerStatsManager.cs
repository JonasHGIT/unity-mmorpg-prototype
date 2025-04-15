using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStatsManager : MonoBehaviour
{
    [Header("Text Elements")]
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI healthRegenText;
    [SerializeField] TextMeshProUGUI manaText;
    [SerializeField] TextMeshProUGUI manaRegenText;
    [SerializeField] TextMeshProUGUI damageText;
    [SerializeField] TextMeshProUGUI attackSpeedText;
    [SerializeField] TextMeshProUGUI armorValueText;
    [SerializeField] TextMeshProUGUI critChanceText;
    [SerializeField] TextMeshProUGUI critDamageBonusText;
    [SerializeField] TextMeshProUGUI playerLevelText;
    [SerializeField] TextMeshProUGUI playerMoveSpeedText;

    private PlayerController playerController;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        UpdateStatsUI();
    }

    public void UpdateStatsUI()
    {
        if (playerController != null)
        {
            healthText.text = "Max Health: " + playerController.maxHealth.ToString("F1"); // Verwende maxHealth
            healthRegenText.text = "Health Regen: " + playerController.healthRegeneration.ToString("F1");
            manaText.text = "Max Mana: " + playerController.maxMana.ToString("F1"); // Verwende maxMana
            manaRegenText.text = "Mana Regen: " + playerController.manaRegeneration.ToString("F1");
            damageText.text = "Damage: " + playerController.minDamage.ToString("F1") + " - " + playerController.maxDamage.ToString("F1");
            attackSpeedText.text = "Attack Speed: " + playerController.attackSpeed.ToString("F1");
            armorValueText.text = "Armor: " + playerController.armorValue.ToString("F1");
            critChanceText.text = "Crit Chance: " + playerController.critChance.ToString("F1") + "%";
            critDamageBonusText.text = "Crit Damage: x" + playerController.critDamageBonus.ToString("F1");
            playerLevelText.text = "Level: " + playerController.playerLevel.ToString();
            playerMoveSpeedText.text = "Move Speed: " + playerController.moveSpeed.ToString();
        }
    }
}
