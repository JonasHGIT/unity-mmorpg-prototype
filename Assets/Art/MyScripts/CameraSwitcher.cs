/*
 * CameraSwitcher.cs
 *
 * Author: Jonas Hammer
 * Description: Ermöglicht das Umschalten zwischen einer Hauptkamera und einer First-Person-Kamera
 *              per Tastendruck (Taste 'O').
 * Last Edited: 16. April 2025
 */

using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Camera References")]
    [SerializeField] public Camera mainCamera;             // Referenz zur Hauptkamera (Third-Person)
    [SerializeField] public Camera firstPersonCamera;      // Referenz zur First-Person-Kamera

    private Camera activeCamera;                           // Die aktuell aktive Kamera

    void Start()
    {
        // Setze initial die mainCamera als aktive Kamera
        activeCamera = mainCamera;
        SetActiveCamera(mainCamera);
    }

    void Update()
    {
        // Überprüfen, ob die Taste "O" gedrückt wurde
        if (Input.GetKeyDown(KeyCode.O))
        {
            SwitchCamera();
        }
    }

    // Wechselt zwischen mainCamera und firstPersonCamera
    void SwitchCamera()
    {
        if (activeCamera == mainCamera)
        {
            SetActiveCamera(firstPersonCamera);
        }
        else
        {
            SetActiveCamera(mainCamera);
        }
    }

    // Aktiviert die gewünschte Kamera und deaktiviert die andere
    void SetActiveCamera(Camera cameraToActivate)
    {
        mainCamera.enabled = false;
        firstPersonCamera.enabled = false;

        cameraToActivate.enabled = true;
        activeCamera = cameraToActivate;
    }
}
