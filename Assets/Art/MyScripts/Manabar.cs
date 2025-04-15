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
