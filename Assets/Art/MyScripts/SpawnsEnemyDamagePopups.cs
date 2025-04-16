/*
 * ------------------------------------------------------------------------------
 * Script:       SpawnEnemyDamagePopups.cs
 * Author:       Jonas Hammer
 * Created:      [Erstellungsdatum]
 * Last Edited:  16. April 2025
 * Description:  Dieses Skript verwaltet das Erstellen und Verwalten von Schadens-Text-Popups, die angezeigt werden, wenn ein Feind Schaden erleidet.
 *               Es verwendet ein ObjectPool, um die Erstellung und das Recycling der Schadens-Popups zu optimieren und spawnt die Popups an der Weltposition des Feindes.
 * 
 * Hauptfunktionen:
 * - Verwendet ein ObjectPool für die effiziente Erstellung und Verwaltung von Schadens-Popups.
 * - Wandelt eine Weltposition in eine Bildschirmposition um, um das Schadens-Popup an der richtigen Stelle anzuzeigen.
 * - Zeigt das Schadens-Popup für Feinde an, wenn dieser Schaden erleidet, und passt die Anzeige für die Richtung an.
 * 
 * UI-Elemente:
 * - `damageLabelPrefab`: Das Prefab für das Schadens-Text-Popup.
 * 
 * Abhängigkeiten:
 * - `EnemyDamageLabel`: Das Label-Objekt, das das Schadens-Popup darstellt und verwaltet.
 * - `ObjectPool`: Eine Pooling-Mechanismus zur Wiederverwendung von `EnemyDamageLabel`-Instanzen.
 * - `SceneManager`: Ein Unity-Komponentenmanager, der auf das Laden von Szenen reagiert, um die Kamera neu zu initialisieren.
 * 
 * Ereignis-Handling:
 * - Das Pop-up für den Feindschaden wird über die Methode `DamageDone()` ausgelöst, wenn der Feind Schaden erleidet.
 * - Das Popup wird an der Bildschirmposition basierend auf der Weltkoordinate des Feindes angezeigt.
 * - Wenn das Schadens-Popup nicht mehr benötigt wird, wird es zurück in den Pool gestellt.
 *
 * Wichtige Hinweise:
 * - Die Methode `SpawnDamagePopup()` berechnet die Position des Popups auf dem Bildschirm und zeigt es in der Nähe der Weltposition des Feindes an.
 * - Die Richtung des Popups wird angepasst, je nachdem, ob der Feind auf der linken oder rechten Seite des Bildschirms steht.
 * - Das Object Pooling hilft dabei, die Performance zu verbessern, indem instanziierte Objekte wiederverwendet werden.
 * ------------------------------------------------------------------------------
 */


using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public class SpawnEnemyDamagePopups : MonoBehaviour
{
    public static SpawnEnemyDamagePopups Instance { get; private set; }

    private ObjectPool<EnemyDamageLabel> _damageLabelPopupPool;

    [Header("Damage Label Popup")]
    [SerializeField] private EnemyDamageLabel damageLabelPrefab;

    [Header("Display Setup")]
    [Range(0.8f, 1.5f), SerializeField] public float displayLength = 1f;
    private Camera _mainCamera;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _damageLabelPopupPool = new ObjectPool<EnemyDamageLabel>(
            () =>
            {
                EnemyDamageLabel damageLabel = Instantiate(damageLabelPrefab, transform);
                damageLabel.Initialize(displayLength, this);
                return damageLabel;
            },
            damageLabel => damageLabel.gameObject.SetActive(true),
            damageLabel => damageLabel.gameObject.SetActive(false)
        );

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _mainCamera = Camera.main;
    }

    public void DamageDone(int damage, Vector3 position)
    {
        // Convert world position to screen position
        Vector2 screenPosition = _mainCamera.WorldToScreenPoint(position);

        bool direction = screenPosition.x < Screen.width * 0.5f;

        // Spawn Damage Popup in der Weltposition
        SpawnDamagePopup(damage, screenPosition, direction);
    }

    private void SpawnDamagePopup(int damage, Vector2 screenPosition, bool direction)
    {
        RectTransform canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, _mainCamera, out Vector2 localPoint);

        EnemyDamageLabel damageLabel = _damageLabelPopupPool.Get();
        damageLabel.transform.position = canvasRect.TransformPoint(localPoint); // Set position
        damageLabel.ShowEnemyDamageLabel(damage, canvasRect.TransformPoint(localPoint), direction); // Methode anpassen
    }



    public void ReturnDamageLabelToPool(EnemyDamageLabel damageLabel)
    {
        _damageLabelPopupPool.Release(damageLabel);
    }
}
