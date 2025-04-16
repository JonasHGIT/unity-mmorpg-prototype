/*
 * EnemyController.cs
 *
 * Author: Jonas Hammer
 * Description: Steuert das Verhalten eines Gegners im Spiel, inklusive Bewegung, Angriff, Schaden und Levelaufstieg.
 * Last Edited: 16. April 2025
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class EnemyController : Interactable
{
    public float lookRadius = 10f; // Radius, in dem der Gegner den Spieler sieht

    public LootManager lootManager; // Referenz zum Loot-Manager
    public PlayerController playerController; // Referenz zum PlayerController

    [Header("Enemy Stats")]
    public int enemyLevel = 1; // Level des Gegners
    public float health = 100f; // Gesundheit des Gegners

    public float minDamage = 14f; // Minimaler Schaden
    public float maxDamage = 20f; // Maximaler Schaden
    public float attackSpeed = 0.5f; // Angriffsgeschwindigkeit
    public float armorValue = 3f; // Rüstungswert des Gegners
    public float xpReward = 18f; // XP-Belohnung für das Besiegen dieses Gegners

    [Header("Growth Factors for Level-Up")]
    private float healthGrowthFactor = 1.20f; // Wachstum der Gesundheit pro Level
    private float damageGrowthFactor = 1.20f; // Wachstum des Schadens pro Level
    private float armorGrowthFactor = 1.40f; // Wachstum des Rüstungswerts pro Level
    private float xpRewardGrowthFactor = 1.05f; // Wachstum der XP-Belohnung pro Level

    [Header("Other")]
    public EnemyType enemyType; // Typ des Gegners (normal, miniBoss, Boss)
    public string enemyName = "Skelettkrieger"; // Name des Gegners

    public enum EnemyType
    {
        normal,   // Normaler Gegner
        miniBoss, // Mini-Boss
        Boss      // Boss
    }

    // Wichtige Variablen für die Navigation und Interaktion
    Transform target;          // Ziel (Spieler)
    NavMeshAgent agent;        // NavMeshAgent zur Bewegung des Gegners
    Renderer renderer;         // Renderer zur Änderung der Materialfarbe

    private float lastAttackTime; // Zeitpunkt des letzten Angriffs
    private Color originalColor; // Ursprüngliche Farbe des Gegners

    // Event, das ausgelöst wird, wenn das Gegnerlevel geändert wird
    public event Action<int> OnLevelChanged;

    // Event, das ausgelöst wird, wenn der Gegner stirbt
    public delegate void EnemyDeathDelegate();
    public event EnemyDeathDelegate OnEnemyDeath;

    void Start()
    {
        // Initialisierung: Hole den PlayerController und setze das Level des Gegners
        playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            enemyLevel = playerController.playerLevel; // Setze das Level des Gegners auf das Level des Spielers
        }

        target = PlayerManager.instance.player.transform; // Ziel ist der Spieler
        agent = GetComponent<NavMeshAgent>(); // Hole den NavMeshAgent für die Bewegung
        renderer = GetComponent<Renderer>(); // Hole den Renderer für die Farbe

        originalColor = renderer.material.color; // Speichere die ursprüngliche Farbe des Gegners
        objectName = "Enemy: " + enemyName + ", " + enemyType.ToString() + ", Level: " + enemyLevel.ToString(); // Setze den Namen des Objekts

        // Setze das Level aller GameItems im Spiel auf das Level des Gegners
        GameItem[] allGameItems = FindObjectsOfType<GameItem>();
        foreach (GameItem item in allGameItems)
        {
            item.itemLevel = enemyLevel; // Setze das Item-Level auf das Level des Gegners
        }

        ApplyStatGrowth(); // Wende die Wachstumsfaktoren an, um die Statistiken des Gegners anzupassen
    }

    void Update()
    {
        // Berechne die Distanz zwischen Gegner und Ziel (Spieler)
        float distance = Vector3.Distance(target.position, transform.position);

        // Wenn der Spieler im Sichtbereich ist, folge ihm
        if (distance <= lookRadius)
        {
            agent.SetDestination(target.position); // Setze das Ziel des Gegners auf den Spieler

            if (distance <= agent.stoppingDistance)
            {
                AttackTarget(); // Greife den Spieler an, wenn der Gegner nah genug ist
                FaceTarget();   // Drehe den Gegner, um dem Spieler entgegenzublicken
            }
        }
    }

    void AttackTarget()
    {
        // Wenn die Zeit für einen neuen Angriff gekommen ist
        if (Time.time >= lastAttackTime + 1f / attackSpeed)
        {
            PlayerController player = target.GetComponent<PlayerController>(); // Hole den PlayerController
            if (player != null)
            {
                // Berechne den Schaden zufällig zwischen minDamage und maxDamage
                float damage = UnityEngine.Random.Range(minDamage, maxDamage);
                player.TakeDamage(damage); // Füge dem Spieler Schaden zu

                lastAttackTime = Time.time; // Setze den Zeitpunkt des letzten Angriffs
            }
        }
    }

    public void TakeDamage(float amount)
    {
        // Berechne den effektiven Schaden, der den Rüstungswert berücksichtigt
        float effectiveDamage = Mathf.Max(amount - armorValue, 0);
        health -= effectiveDamage; // Ziehe den Schaden von der Gesundheit des Gegners ab

        StartCoroutine(FlashRed()); // Lasse den Gegner rot blinken, um den Schaden anzuzeigen

        if (health <= 0)
        {
            Die(); // Wenn die Gesundheit 0 erreicht, stirbt der Gegner
        }
    }

    public override void Interact()
    {
        Debug.Log("Interacting with enemy, dealing damage");
        base.Interact();

        // Hole den PlayerController und lasse ihn den Gegner angreifen
        PlayerController player = PlayerManager.instance.player.GetComponent<PlayerController>();
        if (player != null)
        {
            player.AttackTarget(this); // Spieler greift den Gegner an
        }
    }

    public void Die()
    {
        // Entkopple alle Event-Handler, wenn der Gegner stirbt
        OnEnemyDeath?.Invoke();

        playerController.OnEnemyDeath((int)xpReward); // Gebe XP an den Spieler
        Vector3 dropPosition = transform.position;
        lootManager.DropLoot(dropPosition); // Lasse Loot am Todesort des Gegners fallen
        Destroy(gameObject); // Zerstöre den Gegner
    }

    private IEnumerator FlashRed()
    {
        renderer.material.color = Color.red; // Setze die Farbe des Gegners auf Rot
        yield return new WaitForSeconds(0.1f); // Warte 0.1 Sekunden
        renderer.material.color = originalColor; // Setze die Farbe zurück
    }

    void FaceTarget()
    {
        // Drehe den Gegner, um dem Spieler entgegenzublicken
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void OnDrawGizmosSelected()
    {
        // Zeichne den Sichtkreis des Gegners im Editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }

    // Methode zum Ändern des Levels des Gegners
    public void SetLevel(int newLevel)
    {
        if (enemyLevel != newLevel)
        {
            enemyLevel = newLevel;
            ApplyStatGrowth(); // Wende das Level-Wachstum auf die Statistiken des Gegners an
            OnLevelChanged?.Invoke(enemyLevel); // Event auslösen, wenn das Level geändert wurde
        }
    }

    // Methode zur Skalierung der Statistiken des Gegners basierend auf dem Level
    private void ApplyStatGrowth()
    {
        // Wende exponentielles Wachstum auf die Statistiken des Gegners an
        health *= Mathf.Pow(healthGrowthFactor, enemyLevel - 1);
        armorValue *= Mathf.Pow(armorGrowthFactor, enemyLevel - 1);
        minDamage *= Mathf.Pow(damageGrowthFactor, enemyLevel - 1);
        maxDamage *= Mathf.Pow(damageGrowthFactor, enemyLevel - 1);
        xpReward *= Mathf.Pow(xpRewardGrowthFactor, enemyLevel - 1);
    }
}
