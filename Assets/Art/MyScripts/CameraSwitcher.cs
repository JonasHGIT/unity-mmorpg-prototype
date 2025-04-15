using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] public Camera mainCamera;              // Referenz zur Hauptkamera
    [SerializeField] public Camera firstPersonCamera;       // Referenz zur First-Person-Kamera

    private Camera activeCamera;           // Die aktuell aktive Kamera

    void Start()
    {
        // Standardmäßig setzen wir die mainCamera als aktiv
        activeCamera = mainCamera;
        SetActiveCamera(mainCamera);
    }

    void Update()
    {
        // Wenn die Taste "O" gedrückt wird, Kamera wechseln
        if (Input.GetKeyDown(KeyCode.O))
        {
            SwitchCamera();
        }
    }

    void SwitchCamera()
    {
        // Wechsel zwischen mainCamera und firstPersonCamera
        if (activeCamera == mainCamera)
        {
            SetActiveCamera(firstPersonCamera);
        }
        else
        {
            SetActiveCamera(mainCamera);
        }
    }

    void SetActiveCamera(Camera cameraToActivate)
    {
        // Beide Kameras deaktivieren
        mainCamera.enabled = false;
        firstPersonCamera.enabled = false;

        // Gewählte Kamera aktivieren
        cameraToActivate.enabled = true;
        activeCamera = cameraToActivate;
    }
}
