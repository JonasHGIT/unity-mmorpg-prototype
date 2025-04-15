using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealthbar : MonoBehaviour
{
    private float _maxHealth;
    private float _currentHealth;

    [SerializeField] private Image _healthBarFill;
    [SerializeField] private TMP_Text _healthText;

    private EnemyController enemyController; // Referenz zum EnemyController
    private Camera mainCamera; // Referenz zur Hauptkamera

    void Start()
    {
        // Findet die EnemyController-Komponente im übergeordneten GameObject
        enemyController = GetComponentInParent<EnemyController>();
        mainCamera = Camera.main; // Holt die Hauptkamera

        // Initialisiert die Lebenswerte aus dem EnemyController
        _maxHealth = enemyController.health;
        _currentHealth = _maxHealth;

        // Aktualisiert die Healthbar-UI zu Beginn
        UpdateHealthBar();
    }

    void Update()
    {
        // Aktualisiert das aktuelle Leben aus dem EnemyController
        _currentHealth = enemyController.health;

        // Aktualisiert die Healthbar-UI
        UpdateHealthBar();

        // Stellt sicher, dass der Canvas immer zur Kamera zeigt
        UpdateHealthbarRotation();
    }

    public void UpdateHealthBar()
    {
        float targetFillAmount = _currentHealth / _maxHealth;
        _healthBarFill.fillAmount = targetFillAmount;

        // Aktualisiert den Text für die Healthbar, um aktuelles und maximales Leben anzuzeigen
        _healthText.text = $"{(int)_currentHealth}HP / {(int)_maxHealth}HP";
    }

    private void UpdateHealthbarRotation()
    {
        // Stellt sicher, dass die Healthbar zur Kamera zeigt
        transform.LookAt(mainCamera.transform);
        transform.Rotate(0, 180, 0); // Dreht den Canvas um 180 Grad, weil LookAt ihn umdreht
    }
}
