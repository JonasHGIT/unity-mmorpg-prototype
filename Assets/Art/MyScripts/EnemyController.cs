using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class EnemyController : Interactable
{
    public float lookRadius = 10f;

    public LootManager lootManager;
    public PlayerController playerController;

    [Header("Enemy Stats")]
    public int enemyLevel = 1;
    public float health = 100f;

    public float minDamage = 14f;  // Minimaler Schaden
    public float maxDamage = 20f;  // Maximaler Schaden
    public float attackSpeed = 0.5f;
    public float armorValue = 3f;
    public float xpReward = 18f; // XP reward for defeating this enemy

    [Header("Growth Factors for Level-Up")]
    private float healthGrowthFactor = 1.20f; // 20% growth per level
    private float damageGrowthFactor = 1.20f; // 20% growth per level
    private float armorGrowthFactor = 1.40f; // 40% growth per level
    private float xpRewardGrowthFactor = 1.05f; //5% growth per Level

    [Header("Other")]
    public EnemyType enemyType;
    public string enemyName = "Skelettkrieger";

    public enum EnemyType
    {
        normal,
        miniBoss,
        Boss
    }

    Transform target;
    NavMeshAgent agent;
    Renderer renderer;  // Renderer to change material color

    private float lastAttackTime;
    private Color originalColor;  // Store the original color of the enemy

    // Event, das ausgelöst wird, wenn das Gegnerlevel geändert wird
    public event Action<int> OnLevelChanged;

    public delegate void EnemyDeathDelegate();
    public event EnemyDeathDelegate OnEnemyDeath;

    void Start()
    {
        // Find the PlayerController and set the enemy level to the player level
        playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            enemyLevel = playerController.playerLevel;
        }

        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        renderer = GetComponent<Renderer>();  // Get the renderer component

        originalColor = renderer.material.color;  // Save the original color
        objectName = "Enemy: " + enemyName + ", " + enemyType.ToString() + ", Level: " + enemyLevel.ToString(); // Set the object name

        // Set the item level of all GameItems in the scene to the enemy level
        GameItem[] allGameItems = FindObjectsOfType<GameItem>();
        foreach (GameItem item in allGameItems)
        {
            item.itemLevel = enemyLevel;
        }

        // Apply initial stat growth based on the enemy level
        ApplyStatGrowth();
    }

    void Update()
    {
        float distance = Vector3.Distance(target.position, transform.position);

        if (distance <= lookRadius)
        {
            agent.SetDestination(target.position);

            if (distance <= agent.stoppingDistance)
            {
                AttackTarget();
                FaceTarget();
            }
        }
    }

    void AttackTarget()
    {
        if (Time.time >= lastAttackTime + 1f / attackSpeed)
        {
            PlayerController player = target.GetComponent<PlayerController>();
            if (player != null)
            {
                float damage = UnityEngine.Random.Range(minDamage, maxDamage);  // Calculate random damage
                player.TakeDamage(damage);

                lastAttackTime = Time.time;
            }
        }
    }

    public void TakeDamage(float amount)
    {
        float effectiveDamage = Mathf.Max(amount - armorValue, 0); // Reduce damage by armor value but not below 0
        health -= effectiveDamage;

        StartCoroutine(FlashRed());  // Flash red when taking damage

        if (health <= 0)
        {
            Die();
        }
    }

    public override void Interact()
    {
        Debug.Log("Interacting with enemy, dealing damage");
        base.Interact();

        // Hole den PlayerController
        PlayerController player = PlayerManager.instance.player.GetComponent<PlayerController>();
        if (player != null)
        {
            // Greife die AttackTarget-Methode auf, um Schaden zu berechnen und dem Feind zuzufügen
            player.AttackTarget(this);
        }
    }

    public void Die()
    {
        // Entkopple alle Event-Handler
        if (OnEnemyDeath != null)
            OnEnemyDeath.Invoke();

        playerController.OnEnemyDeath((int)xpReward);
        Vector3 dropPosition = transform.position;
        lootManager.DropLoot(dropPosition); // Pass the enemy level to the loot manager
        Destroy(gameObject);
    }


    private IEnumerator FlashRed()
    {
        renderer.material.color = Color.red;  // Change to red
        yield return new WaitForSeconds(0.1f);  // Wait for 0.1 seconds
        renderer.material.color = originalColor;  // Change back to the original color
    }

    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }

    // Methode zum Ändern des Levels des Gegners
    public void SetLevel(int newLevel)
    {
        if (enemyLevel != newLevel)
        {
            enemyLevel = newLevel;
            ApplyStatGrowth(); // Stat growth is applied when the enemy's level changes
            OnLevelChanged?.Invoke(enemyLevel); // Event auslösen, wenn das Level geändert wurde
        }
    }

    // Methode, um die Statistiken des Gegners basierend auf dem Level zu skalieren
    private void ApplyStatGrowth()
    {
        // Apply exponential growth to stats
        health *= Mathf.Pow(healthGrowthFactor, enemyLevel - 1);
        armorValue *= Mathf.Pow(armorGrowthFactor, enemyLevel - 1);
        minDamage *= Mathf.Pow(damageGrowthFactor, enemyLevel - 1);
        maxDamage *= Mathf.Pow(damageGrowthFactor, enemyLevel - 1);
        xpReward *= Mathf.Pow(xpRewardGrowthFactor, enemyLevel - 1);
    }
}
