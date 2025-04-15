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
