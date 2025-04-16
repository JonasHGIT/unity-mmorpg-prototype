/*
 * Healthbar.cs
 *
 * Author: Jonas Hammer
 * Description: Aktualisiert die Gesundheitsanzeige des Spielers im UI.
 * Last Edited: 16. April 2025
 */

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Healthbar : MonoBehaviour
{
    private float _maxHealth;
    private float _currentHealth;

    [SerializeField] private Image _healthBarFill;
    [SerializeField] private TMP_Text _healthText;

    private PlayerController playerController; // Reference to the PlayerController

    // Start is called before the first frame update
    void Start()
    {
        // Find the Player_Capsule_Mesh object and get the PlayerController component
        playerController = GameObject.Find("Player_Capsule_Mesh").GetComponent<PlayerController>();

        // Initialize health values from the PlayerController
        _maxHealth = playerController.maxHealth;
        _currentHealth = playerController.currentHealth;

        // Update the health bar UI initially
        UpdateHealthBar();
    }

    // Update is called once per frame
    void Update()
    {
        // Update max health in case it has changed (e.g., due to item effects)
        if (_maxHealth != playerController.maxHealth)
        {
            _maxHealth = playerController.maxHealth;
        }

        // Update current health from the PlayerController
        _currentHealth = playerController.currentHealth;

        // Optionally regenerate health
        if (_currentHealth < _maxHealth)
        {
            _currentHealth += playerController.healthRegeneration * Time.deltaTime;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth); // Ensure health does not exceed max or go below 0

            // Apply the regenerated health back to the PlayerController
            playerController.currentHealth = _currentHealth;
        }

        // Update the health bar UI
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        float targetFillAmount = _currentHealth / _maxHealth;
        _healthBarFill.fillAmount = targetFillAmount;

        // Update the health text to reflect the current and maximum health
        _healthText.text = $"{(int)_currentHealth}HP / {(int)_maxHealth}HP";
    }
}
