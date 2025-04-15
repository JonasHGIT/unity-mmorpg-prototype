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
