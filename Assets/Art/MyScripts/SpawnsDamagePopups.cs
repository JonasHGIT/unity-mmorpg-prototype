/*
 * ------------------------------------------------------------------------------
 * Script:       SpawnsDamagePopups.cs
 * Author:       Jonas Hammer
 * Created:      [Erstellungsdatum]
 * Last Edited:  16. April 2025
 * Description:  Dieses Skript verwaltet das Erzeugen und Verwalten von Schadens-Popups, die im Spiel erscheinen, wenn der Spieler Schaden erleidet.
 *               Es nutzt ein Objekt-Pool-Design, um Popup-Objekte effizient zu erstellen und wiederzuverwenden, um die Leistung zu optimieren.
 *
 * Hauptfunktionen:
 * - Instanziierung von Schadens-Popup-Objekten (DamageLabel) bei Schaden.
 * - Verwaltung des Objektspeichers mittels eines ObjectPools für die Popup-Labels, um unnötige Instanziierungen zu vermeiden.
 * - Anzeige der Schadenswerte auf der Benutzeroberfläche in Form von Popups an einer bestimmten Position auf dem Bildschirm.
 * - Möglichkeit zur Unterscheidung zwischen normalen und kritischen Treffern durch die Darstellung von hervorgehobenen Popups.
 *
 * Abhängigkeiten:
 * - `DamageLabel`: Ein Prefab, das das Schadens-Popup darstellt, das die Schadenswerte anzeigt.
 * - `ObjectPool`: Ein Pool-Objekt, das die Instanziierung und Wiederverwendung von `DamageLabel`-Objekten verwaltet.
 * 
 * UI-Elemente:
 * - `DamageLabel`: Ein Pop-up-UI-Element, das den Schadenswert anzeigt.
 *
 * Ereignis-Handling:
 * - Es wird auf Szenenwechsel reagiert, um sicherzustellen, dass die Kamera immer korrekt zugewiesen wird.
 *
 * Wichtige Hinweise:
 * - Das System verwendet die `ObjectPool`-Klasse, um die Instanziierung von Objekten zu optimieren, indem es bereits existierende Objekte erneut verwendet, anstatt neue zu erstellen und zu zerstören.
 * - Die Anzeige der Schadens-Popups wird über die Methode `DamageDone` ausgelöst, die den Schadenswert sowie eine Position auf dem Bildschirm übergibt.
 * - Der `DamageLabel` wird nach der Anzeige wieder in den Pool zurückgegeben, um die Effizienz zu maximieren.
 * ------------------------------------------------------------------------------
 */


using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public class SpawnsDamagePopups : MonoBehaviour
{
    public static SpawnsDamagePopups Instance { get; private set; }
    
    private ObjectPool<DamageLabel> _damageLabelPopupPool;
    
    [Header("Damage Label Popup")]
    [SerializeField] private DamageLabel damageLabelPrefab;

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
            Destroy(gameObject);
        
        _damageLabelPopupPool = new ObjectPool<DamageLabel>(
            () =>
            {
                DamageLabel damageLabel = Instantiate(damageLabelPrefab, transform);
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
    
    public void DamageDone(int damage, Vector3 position, bool isCrit)
    {
        // Berechne die Mitte des Bildschirmbereichs
        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

        bool direction = screenCenter.x < Screen.width * 0.5f;

        // Spawn Damage Popup in der Mitte des Bildschirmbereichs
        SpawnDamagePopup(damage, screenCenter, direction, isCrit);
    }

    private void SpawnDamagePopup(int damage, Vector2 screenPosition, bool direction, bool isCrit)
    {
        RectTransform canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, _mainCamera, out Vector2 localPoint);

        DamageLabel damageLabel = _damageLabelPopupPool.Get();
        damageLabel.transform.position = canvasRect.TransformPoint(localPoint); // Position setzen
        damageLabel.ShowDamageLabel(damage, canvasRect.TransformPoint(localPoint), direction, isCrit); // Methode anpassen
    }



    public void ReturnDamageLabelToPool(DamageLabel damageLabel)
    {
        _damageLabelPopupPool.Release(damageLabel);
    }
}
