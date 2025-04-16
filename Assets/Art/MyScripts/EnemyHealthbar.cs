/*
 * EnemyHealthbar.cs
 * 
 * Author: Jonas Hammer
 * Description: Zeigt die Lebensanzeige des Gegners an und stellt sicher, dass sie immer zur Kamera ausgerichtet ist.
 * Last Edited: 16. April 2025
 */

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealthbar : MonoBehaviour
{
    private float _maxHealth;  // Maximale Gesundheit des Gegners
    private float _currentHealth;  // Aktuelle Gesundheit des Gegners

    [SerializeField] private Image _healthBarFill;  // Die Füllanzeige der Healthbar
    [SerializeField] private TMP_Text _healthText;  // Der Text zur Anzeige von Leben in der Healthbar

    private EnemyController enemyController;  // Referenz zum EnemyController
    private Camera mainCamera;  // Referenz zur Hauptkamera

    void Start()
    {
        // Findet die EnemyController-Komponente im übergeordneten GameObject (Elternobjekt)
        enemyController = GetComponentInParent<EnemyController>();
        mainCamera = Camera.main;  // Holt die Hauptkamera

        // Initialisiert die Lebenswerte aus dem EnemyController
        _maxHealth = enemyController.health;
        _currentHealth = _maxHealth;

        // Aktualisiert die Healthbar-UI zu Beginn
        UpdateHealthBar();
    }

    void Update()
    {
        // Aktualisiert die aktuelle Gesundheit aus dem EnemyController
        _currentHealth = enemyController.health;

        // Aktualisiert die Healthbar-UI
        UpdateHealthBar();

        // Stellt sicher, dass die Healthbar immer zur Kamera zeigt
        UpdateHealthbarRotation();
    }

    // Methode zur Aktualisierung der Healthbar
    public void UpdateHealthBar()
    {
        // Berechnet den Füllstand der Healthbar basierend auf aktueller und maximaler Gesundheit
        float targetFillAmount = _currentHealth / _maxHealth;
        _healthBarFill.fillAmount = targetFillAmount;

        // Aktualisiert den Text der Healthbar, um aktuelle und maximale Gesundheit anzuzeigen
        _healthText.text = $"{(int)_currentHealth}HP / {(int)_maxHealth}HP";
    }

    // Methode zur Aktualisierung der Rotation der Healthbar, sodass sie immer zur Kamera zeigt
    private void UpdateHealthbarRotation()
    {
        // Stellt sicher, dass der Canvas immer zur Kamera zeigt
        transform.LookAt(mainCamera.transform);
        transform.Rotate(0, 180, 0);  // Dreht den Canvas um 180 Grad, da LookAt ihn sonst umdreht
    }
}
