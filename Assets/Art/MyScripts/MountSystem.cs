using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MountSystem : MonoBehaviour
{
    public PlayerController playerController; // Referenz auf das PlayerController-Skript
    public GameObject mount;                  // Das Reittier-Objekt
    public Transform mountPoint;              // Der Punkt, an dem der Spieler auf dem Reittier sitzt
    public float mountSpeedBonus = 5f;        // Der Bonus für die Geschwindigkeit, wenn man auf dem Reittier ist
    public float mountTime = 5f;              // Zeit, die benötigt wird, um auf das Reittier zu steigen

    private bool isMounted = false;           // Status, ob der Spieler auf dem Reittier ist
    private bool isMounting = false;          // Status, ob der Spieler gerade aufsteigt
    private float originalSpeed;              // Die ursprüngliche Geschwindigkeit des Spielers
    private Transform originalParent;         // Der ursprüngliche Eltern-Transform des Spielers
    private float mountHeight;                // Die Höhe des Mounts

    void Start()
    {
        originalSpeed = playerController.moveSpeed; // Speichere die ursprüngliche Geschwindigkeit
        originalParent = playerController.transform.parent; // Speichere das originale Parent-Objekt des Spielers

        // Bestimme die Höhe des Mounts basierend auf dem Collider des Mounts
        Collider mountCollider = mount.GetComponent<Collider>();
        if (mountCollider != null)
        {
            mountHeight = mountCollider.bounds.size.y; // Setze die Höhe auf die Y-Größe des Colliders
        }
        else
        {
            Debug.LogWarning("Kein Collider auf dem Mount gefunden! Höhe wird auf 0 gesetzt.");
            mountHeight = 0f;
        }
    }

    void Update()
    {
        // Überprüfe, ob "R" gedrückt wurde
        if (Keyboard.current.rKey.wasPressedThisFrame && !isMounting)
        {
            if (isMounted)
            {
                Dismount(); // Wenn der Spieler schon auf dem Reittier ist, absteigen
            }
            else
            {
                StartCoroutine(MountCoroutine()); // Starte den Aufsteigevorgang
            }
        }
    }

    // Coroutine, um das Aufsteigen zu simulieren
    private IEnumerator MountCoroutine()
    {
        isMounting = true;
        Debug.Log("Aufsteigen beginnt..."); // Debug Nachricht

        yield return new WaitForSeconds(mountTime); // Warte die Mount-Zeit

        Mount();
        isMounting = false;
    }

    // Funktion zum Aufsteigen auf das Reittier
    private void Mount()
    {
        isMounted = true;
        playerController.moveSpeed += mountSpeedBonus; // Erhöhe die Bewegungsgeschwindigkeit
        mount.SetActive(true); // Zeige das Reittier an (falls es nicht sichtbar ist)

        // Setze das Mount unter den Spieler und schiebe den Spieler nach oben
        mount.transform.SetParent(playerController.transform); // Das Mount wird ein Kind des Spielers
        mount.transform.localPosition = new Vector3(0f, -mountHeight / 2f, 0f); // Verschiebe das Mount um die halbe Höhe nach unten
        mount.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);


        // Verschiebe den Spieler um die Höhe des Mounts nach oben
        Vector3 playerPosition = playerController.transform.position;
        playerController.transform.position = new Vector3(playerPosition.x, playerPosition.y + mountHeight, playerPosition.z);

        Debug.Log("Auf das Reittier aufgestiegen!");
    }

    // Funktion zum Absteigen vom Reittier
    private void Dismount()
    {
        isMounted = false;
        playerController.moveSpeed = originalSpeed; // Setze die Bewegungsgeschwindigkeit zurück

        // Entferne das Mount vom Spieler und setze das Parent zurück
        mount.transform.SetParent(null); // Mount nicht mehr Kind des Spielers
        playerController.transform.SetParent(originalParent);

        // Setze den Spieler wieder auf seine ursprüngliche Position (falls gewünscht)
        Vector3 playerPosition = playerController.transform.position;
        playerController.transform.position = new Vector3(playerPosition.x, playerPosition.y - mountHeight, playerPosition.z);

        mount.SetActive(false); // Verstecke das Reittier (falls erforderlich)
        Debug.Log("Vom Reittier abgestiegen!");
    }
}
