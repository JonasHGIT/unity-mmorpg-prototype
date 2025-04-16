/*
 * CameraController.cs
 * 
 * Author: Jonas Hammer
 * Description: Dieses Script sorgt dafür, dass die Kamera einer Ziel-Transform flüssig folgt. 
 *              Dabei wird ein Offset berücksichtigt, um z. B. nicht direkt auf der Spielfigur zu sitzen.
 * Last Edited: 16. April 2025
 *
 * Hinweise:
 * - Die Kamera folgt dem Target mit einer Glättung (smoothSpeed)
 * - Offset kann für perspektivisches Verschieben verwendet werden
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Zielobjekt (meist der Spieler)")]
    public Transform target;

    [Header("Glättung & Versatz")]
    public float smoothSpeed = 8f;       // Geschwindigkeit der Kamera-Glättung
    public Vector3 offset;               // Offset zur Zielposition

    /// <summary>
    /// Wird einmal pro Frame aufgerufen – bewegt die Kamera sanft zum Ziel.
    /// </summary>
    void Update()
    {
        if (target == null) return; // Keine Bewegung ohne Ziel

        // Berechne die gewünschte Kameraposition basierend auf dem Ziel & Offset
        Vector3 desiredPosition = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            target.position.z + offset.z
        );

        // Sanfte Bewegung zur Zielposition
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}
