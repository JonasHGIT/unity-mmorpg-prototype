/*
 * ------------------------------------------------------------------------------
 * Script:       PlayerController.cs
 * Author:       Jonas Hammer
 * Created:      [Erstellungsdatum]
 * Last Edited:  16. April 2025
 * Description:  Dieses Skript verwaltet die Bewegungen, Angriffe, Gesundheits- und Mana-Werte sowie
 *               das Level-Up-System des Spielers. Es enthält auch die Interaktion mit der Welt
 *               (Bewegung zu Klickpunkten, Interaktion mit Objekten, Angriff auf Feinde) und die
 *               Darstellung von Level-Up-VFX und UI-Elementen.
 *
 * Hauptfunktionen:
 * - Steuerung der Spielerbewegung über Klick-Input
 * - Angriff auf Feinde und Berechnung von Schaden (inkl. Kritischen Treffern)
 * - Verwaltung von Gesundheit, Mana, und anderen Spielerstatistiken
 * - Level-Up System und Anwendung von Stat-Wachstum bei jedem Levelaufstieg
 * - Anzeigen von Level-Up VFX und UI-Canvas
 * - Interaktionen mit der Welt und anderen Objekten (z.B. Angreifen, Klicken, Fokussierung auf Objekte)
 *
 * Unterstützt:
 * - Spielerbewegung und -animationen mit NavMeshAgent
 * - Angriffsmechanik (Berechnung von Schaden, Kritischen Treffern und Anzeige von Schadens-Popups)
 * - Status-Updates für Gesundheit, Mana, Angriffsgeschwindigkeit und mehr
 * - Level-Up Mechanik mit visuellen und UI-Komponenten (VFX, Level-Up Canvas)
 *
 * Abhängigkeiten:
 * - ExperienceManager.cs (für das Spieler-Level und Erfahrung)
 * - PlayerStatsManager.cs (für die Anzeige der Spielerstatistiken)
 * - Skilltree.cs (für die Aktualisierung des Skilltrees beim Level-Up)
 * - SpawnsDamagePopups.cs (für die Anzeige von Schadens-Popups)
 * - Manabar.cs (für die Aktualisierung der Mana-Leiste)
 * - NavMeshAgent (für die Navigation des Spielers)
 * - Animator (für Animationen des Spielers)
 *
 * Hinweise:
 * - Das Skript benötigt eine Referenz auf den ExperienceManager und PlayerStatsManager, um korrekt zu funktionieren.
 * - Der Spieler bewegt sich durch Mausklicks auf den Boden oder Interaktionsobjekte, und es wird die Animation für das Gehen/Idle abgespielt.
 * - Level-Ups führen zu einer Erhöhung von wichtigen Stats wie Gesundheit und Schaden und zeigen visuelle Effekte und UI-Updates.
 * - Dieses Skript ist auch für das Anzeigen von Schadens-Popups beim Angreifen verantwortlich.
 * ------------------------------------------------------------------------------
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using TMPro;

public class PlayerController : MonoBehaviour
{
    const string IDLE = "Idle";
    const string WALK = "Walk";

    CustomActions input;
    public NavMeshAgent agent;
    Animator animator;
    Renderer renderer;

    [Header("Movement")]
    [SerializeField] ParticleSystem clickEffect;
    [SerializeField] LayerMask clickableLayers;
    public GameObject levelUpVFX;

    [Header("Player Stats")]
    public int playerLevel = 1; // Spielerlevel initialisieren
    public float maxHealth = 200f; // Maximaler Gesundheit
    public float currentHealth; // Aktuelle Gesundheit
    public float healthRegeneration = 5f;
    public float maxMana = 100f; // Maximaler Mana
    public float currentMana; // Aktueller Mana
    public float manaRegeneration = 5f;
    public float minDamage = 5f;
    public float maxDamage = 10f;
    public float attackSpeed = 1.5f;
    public float armorValue = 0f;
    public float critChance = 0f;
    public float critDamageBonus = 2f; // +100% Schaden bei Krits (Doppelter Schaden)

    // Wachstumfaktoren für Level-Ups
    private float healthGrowthFactor = 1.05f; // 5% Wachstum pro Level
    private float damageGrowthFactor = 1.03f; // 3% Wachstum pro Level

    public int skillPoint = 1;

    public float moveSpeed = 4f; // Die Bewegungsgeschwindigkeit des Spielers

    private ExperienceManager experienceManager; // Referenz zum ExperienceManager
    private float lookRotationSpeed = 8f;
    public Interactable focus;
    private float lastAttackTime;

    private Color originalColor;
    private bool isAttacking = false;
    public EnemyController currentTarget;
    private PlayerStatsManager playerStatsManager; // Referenz für die UI-Aktualisierung
    public GameObject levelUpCanvas;
    public TMP_Text levelUpText;
    public Skilltree skilltree; // Referenz auf das Skilltree-Skript


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        renderer = GetComponent<Renderer>();
        originalColor = renderer.material.color;
        input = new CustomActions();
        AssignInputs();

        experienceManager = FindObjectOfType<ExperienceManager>(); // ExperienceManager suchen und speichern
        playerStatsManager = FindObjectOfType<PlayerStatsManager>(); // PlayerStatsManager finden

        UpdatePlayerLevel(); // Initiales Spielerlevel setzen

        currentHealth = maxHealth; // Initialisiere die aktuelle Gesundheit mit der maximalen Gesundheit
        currentMana = maxMana; // Initialisiere das aktuelle Mana mit dem maximalen Mana

        // Initialize the NavMeshAgent speed with moveSpeed
        agent.speed = moveSpeed;
    }

    void AssignInputs()
    {
        input.Main.Move.performed += ctx => ClickToMove();
        input.Main.Move.canceled += ctx => StopAttacking(); // Stop attacking when left mouse button is released
    }

    void ClickToMove()
    {
        // Check if the click is on a UI element
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // If true, don't execute movement
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, clickableLayers))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                SetFocus(interactable);
                MoveToInteractable(interactable);
                return;
            }

            MoveToPoint(hit.point);
            RemoveFocus();
        }
    }

    void MoveToPoint(Vector3 point)
    {
        agent.destination = point;
        if (clickEffect != null)
        {
            Instantiate(clickEffect, point + new Vector3(0, 0.1f, 0), clickEffect.transform.rotation);
        }
    }

    void MoveToInteractable(Interactable interactable)
    {
        agent.destination = interactable.transform.position;
        StartCoroutine(interactable.CheckDistanceToPlayer(this, agent));
    }

    void SetFocus(Interactable newFocus)
    {
        focus = newFocus;
    }

    void RemoveFocus()
    {
        focus = null;
        StopAllCoroutines();
    }

    void OnEnable()
    {
        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();
    }

    public void UpdateManaBar()
    {
        FindObjectOfType<Manabar>().UpdateManaBar();
    }

    void Update()
    {
        FaceTarget();
        SetAnimations();

        // Update NavMeshAgent speed in case moveSpeed changes
        if (agent.speed != moveSpeed)
        {
            agent.speed = moveSpeed;
        }

        if (Mouse.current.leftButton.isPressed)
        {
            TryAttackEnemy();
        }
        else if (isAttacking)
        {
            StopAttacking();
        }
    }

    void FaceTarget()
    {
        if (agent.hasPath)
        {
            Vector3 direction = (agent.destination - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
        }
    }

    void SetAnimations()
    {
        if (agent.velocity == Vector3.zero)
        {
            animator.Play(IDLE);
        }
        else
        {
            animator.Play(WALK);
        }
    }

    void TryAttackEnemy()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, 100f))
            {
                EnemyController enemy = hit.collider.GetComponent<EnemyController>();
                if (enemy != null && IsWithinInteractionRadius(enemy))
                {
                    if (currentTarget == null || currentTarget != enemy)
                    {
                        currentTarget = enemy;
                        isAttacking = true;
                    }
                    AttackTarget(enemy);
                }
            }
        }
    }

    public void AttackTarget(EnemyController enemy)
    {
        if (Time.time >= lastAttackTime + 1f / attackSpeed)
        {
            int damage = CalculateDamage();
            bool isCrit = Random.Range(0, 100) < critChance;
            if (isCrit)
            {
                damage = (int)(damage * critDamageBonus);
            }
            enemy.TakeDamage(damage);

            // Use the new method to display damage label
            Vector3 hitPoint = enemy.transform.position; // Display at enemy position
            ShowDamagePopup(damage, hitPoint, isCrit);

            lastAttackTime = Time.time;
        }
    }

    public void ShowDamagePopup(int damage, Vector3 position, bool isCrit)
    {
        // Display damage label
        SpawnsDamagePopups.Instance.DamageDone(damage, position, isCrit);
    }


    void StopAttacking()
    {
        isAttacking = false;
        currentTarget = null;
    }

    bool IsWithinInteractionRadius(EnemyController enemy)
    {
        float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
        return distanceToEnemy <= enemy.radius;
    }

    int CalculateDamage()
    {
        return Random.Range((int)minDamage, (int)maxDamage);
    }

    public void TakeDamage(float amount)
    {
        float effectiveDamage = Mathf.Max(amount - armorValue, 0);
        currentHealth -= effectiveDamage;

        // Display damage label for damage taken by the player
        Vector3 hitPoint = transform.position; // Display at player position
        SpawnEnemyDamagePopups.Instance.DamageDone((int)effectiveDamage, hitPoint);

        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player has died");
    }

    private IEnumerator FlashRed()
    {
        renderer.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        renderer.material.color = originalColor;
    }

    public void UpdatePlayerLevel()
    {
        int previousLevel = playerLevel;
        playerLevel = experienceManager.GetCurrentLevel(); // Spielerlevel vom ExperienceManager aktualisieren

        if (playerLevel > previousLevel) // Only apply stat growth on level up
        {
            if (playerLevel >= 2)
            {
                ApplyStatGrowth();
                skillPoint++; // Skillpunkte hinzufügen
                StartCoroutine(ShowLevelUpVFX()); 
                ActivateLevelUpVFX();
                ActivateLevelUpCanvas();

                // Benachrichtige den Skilltree über das Level-Up
                if (skilltree != null)
                {
                    skilltree.OnLevelUp(); // UI im Skilltree aktualisieren
                }
            }
        }
    }

    private void ActivateLevelUpCanvas()
    {
        if (levelUpCanvas != null)
        {
            levelUpCanvas.SetActive(true);
            levelUpText.text = playerLevel.ToString();
            
            // Deaktiviere nach 8 Sekunden
            Invoke("DeactivateLevelUpCanvas", 8f);
        }
    }

    private void DeactivateLevelUpCanvas()
    {
        if (levelUpCanvas != null)
        {
            levelUpCanvas.SetActive(false); // Deaktiviere den Canvas
        }
    }

    private void ActivateLevelUpVFX()
    {
        if (levelUpVFX != null)
        {
            levelUpVFX.SetActive(true); // Aktiviere das VFX

            // Deaktiviere das VFX nach 8 Sekunden
            Invoke("DeactivateLevelUpVFX", 8f);
        }
    }

    private void DeactivateLevelUpVFX()
    {
        if (levelUpVFX != null)
        {
            levelUpVFX.SetActive(false); // Deaktiviere das VFX
        }
    }


    private void ApplyStatGrowth()
    {
        // Apply exponential growth to stats
        maxHealth *= healthGrowthFactor;
        minDamage *= damageGrowthFactor;
        maxDamage *= damageGrowthFactor;

        // Ensure current health and mana are updated with the new max values
        currentHealth = maxHealth;
        currentMana = maxMana;

        // Update the UI elements
        playerStatsManager.UpdateStatsUI();
    }

    private IEnumerator ShowLevelUpVFX()
    {
        if (levelUpVFX != null)
        {
            // Finde oder füge eine CanvasGroup-Komponente hinzu
            CanvasGroup canvasGroup = levelUpVFX.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = levelUpVFX.AddComponent<CanvasGroup>();
            }

            // Setze das Alpha auf 0 und aktiviere das VFX
            canvasGroup.alpha = 0f;
            levelUpVFX.SetActive(true);

            // Fade in
            float fadeInDuration = 1f;
            for (float t = 0; t < fadeInDuration; t += Time.deltaTime)
            {
                canvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeInDuration);
                yield return null;
            }
            canvasGroup.alpha = 1f;

            // Warte 3 Sekunden
            yield return new WaitForSeconds(3f);

            // Fade out
            float fadeOutDuration = 1f;
            for (float t = 0; t < fadeOutDuration; t += Time.deltaTime)
            {
                canvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeOutDuration);
                yield return null;
            }
            canvasGroup.alpha = 0f;

            // Deaktiviere das VFX nach dem Fade-Out
            levelUpVFX.SetActive(false);
            //StopCoroutine("ShowLevelUpVFX");
        }
    }




    // This function is called by the enemy when it dies
    public void OnEnemyDeath(int xpReward)
    {
        experienceManager.AddExperience(xpReward); // Add XP when the enemy is defeated
    }
}
