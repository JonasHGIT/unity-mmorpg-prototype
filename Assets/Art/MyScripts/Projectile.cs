using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage; // Basis-Schaden
    public float damageMultiplier;
    public float speed = 20f;
    private Vector3 direction;
    private float initialHeight; // Höhe des Startpunkts speichern

    public void Initialize(Vector3 _direction, float _damageMultiplier)
    {
        // Richtung und Schadensmultiplikator initialisieren
        direction = _direction;
        damageMultiplier = _damageMultiplier;
        
        // Höhe beim Start festlegen
        initialHeight = transform.position.y;
        
        // Die Y-Komponente der Richtung auf Null setzen, damit sich das Projektil nicht in der Höhe verändert
        direction.y = 0;
        direction.Normalize(); // Richtung normalisieren, um die Geschwindigkeit gleichmäßig zu halten
    }

    void Update()
    {
        // Position aktualisieren, aber die Höhe auf die Start-Höhe fixieren
        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;
        newPosition.y = initialHeight; // Höhe festlegen
        transform.position = newPosition;
    }

    void OnTriggerEnter(Collider other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
        {
            float totalDamage = damage * damageMultiplier;
            enemy.TakeDamage(totalDamage);
            Destroy(gameObject); // Projektil zerstören nach einer Kollision
        }
    }
}
