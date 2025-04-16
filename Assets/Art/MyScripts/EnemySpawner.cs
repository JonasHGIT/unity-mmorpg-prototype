/*
 * EnemySpawner.cs
 * 
 * Author: Jonas Hammer
 * Description: Spawnt Gegner in einer zufälligen Reihenfolge an vorgegebenen Spawn-Punkten. Verhindert, dass zu viele Gegner gleichzeitig existieren.
 * Last Edited: 16. April 2025
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab; // Prefab des Gegners, das gespawnt wird
    public Transform[] spawnPoints; // Orte, an denen die Gegner spawnen können
    public float spawnDelay = 5f; // Verzögerung vor dem ersten Spawn
    public float spawnInterval = 10f; // Intervall zwischen den Spawns
    public int maxEnemies = 5; // Maximale Anzahl an Gegnern, die gleichzeitig aktiv sein dürfen

    private int currentEnemyCount = 0; // Aktuelle Anzahl der aktiven Gegner

    [Header("References")]
    public LootManager lootManager; // Referenz zum LootManager
    public PlayerController playerController; // Referenz zum PlayerController

    void Start()
    {
        // Überprüfe, ob das Prefab nicht null ist und ein echtes Prefab ist
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab is not assigned in the EnemySpawner script.");
            return;
        }

        // Startet das Spawning nach der Verzögerung
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        // Warte auf die anfängliche Verzögerung
        yield return new WaitForSeconds(spawnDelay);

        while (true)
        {
            // Spawne nur neue Gegner, wenn die maximale Anzahl noch nicht erreicht ist
            if (currentEnemyCount < maxEnemies)
            {
                SpawnEnemy();
            }

            // Warte bis zum nächsten Spawn
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        // Wähle einen zufälligen Spawn-Punkt aus der Liste aus
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Spawne den Gegner am ausgewählten Punkt
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        // Stelle sicher, dass der EnemyController korrekt initialisiert wird
        EnemyController enemyController = newEnemy.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.lootManager = lootManager;
            enemyController.playerController = playerController;

            // Setze die Gesundheit des Gegners auf 100, um sicherzustellen, dass jeder neue Gegner volle Gesundheit hat
            enemyController.health = 100f;  // Volle Gesundheit initialisieren

            // Registriere das Event, um die Anzahl der Feinde zu verringern, wenn einer stirbt
            enemyController.OnEnemyDeath += HandleEnemyDeath;
        }
        else
        {
            Debug.LogError("Enemy prefab does not have an EnemyController component.");
        }

        // Inkrementiere die Anzahl der aktuellen Gegner
        currentEnemyCount++;
    }

    private void HandleEnemyDeath()
    {
        currentEnemyCount--; // Verringere die Anzahl, wenn ein Gegner stirbt
    }

    // Aufräumarbeiten: Alle Event-Handler entfernen, um Memory-Leaks zu verhindern
    void OnDestroy()
    {
        StopAllCoroutines(); // Beendet alle Coroutinen, falls das Spawner-Objekt zerstört wird
    }
}
