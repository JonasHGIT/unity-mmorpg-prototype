using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HotkeySkill : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI")]
    public Image image; 
    public Image coolDownImage; // Image used for cooldown display

    [SerializeField] public Skill skill;
    [SerializeField] public GameObject fireballPrefab;
    [SerializeField] private Transform projectileSpawnPoint;

    [SerializeField] private GameObject manashotPrefab;
    [SerializeField] private GameObject iceAttackPrefab;
    [SerializeField] private GameObject lightningPrefab;
    [SerializeField] private GameObject chainLightningPrefab;

    [Header("VFX")]
    [SerializeField] public GameObject electroSlashPrefab;  // Referenz zum VFX-Objekt


    [HideInInspector] public Transform parentAfterDrag;

    private PlayerController playerController; // Reference to PlayerController
    private bool isCooldown = false; // Flag to track cooldown state

    private float lastAttackTime;  // Speichert den Zeitpunkt des letzten Angriffs

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        if (coolDownImage != null)
        {
            coolDownImage.fillAmount = 0;
        }
        lastAttackTime = -Mathf.Infinity;  // Initialisieren
    }

    public void InitialiseSkill(Skill newSkill)
    {
        skill = newSkill;
        if (image != null && newSkill != null)
        {
            image.sprite = newSkill.image;
        }
    }

    public void ExecuteSkill()
    {
        // Berechne das Intervall abhängig von der Angriffsgeschwindigkeit des Spielers
        float attackInterval = 1f / playerController.attackSpeed;

        if (skill != null && !isCooldown && Time.time >= lastAttackTime + attackInterval)
        {
            if (playerController.currentMana >= skill.manaCost)
            {
                lastAttackTime = Time.time;  // Setze den letzten Angriff auf die aktuelle Zeit
                
                switch (skill.name)
                {
                    case SkillName.Fireball:
                        CastFireball();
                        break;
                    case SkillName.Teleport:
                        CastTeleport();
                        break;
                    case SkillName.Manashot:
                        CastManashot();
                        break;
                    case SkillName.IceAttack:
                        CastIceAttack();
                        break;
                    case SkillName.Lightning:
                        CastLightning();
                        break;
                    case SkillName.ChainLightning:
                        CastChainLightning();
                        break;
                    case SkillName.MeleeSlash:
                        PerformMeleeSlash();
                        break;
                    default:
                        Debug.Log("Skill " + skill.name + " wurde noch nicht implementiert.");
                        break;
                }

                StartCoroutine(CooldownRoutine());
            }
            else
            {
                Debug.Log("Not enough Mana to cast " + skill.name);
            }
        }
        else
        {
            Debug.Log("Attack is on cooldown or insufficient attack interval");
        }
    }

    private IEnumerator CooldownRoutine()
    {
        isCooldown = true; // Skill is now on cooldown
        coolDownImage.fillAmount = 1; // Start cooldown animation

        float elapsed = 0f;
        while (elapsed < skill.coolDown)
        {
            elapsed += Time.deltaTime;
            coolDownImage.fillAmount = 1 - (elapsed / skill.coolDown); // Update fill amount over time
            yield return null; // Wait for the next frame
        }

        coolDownImage.fillAmount = 0; // Reset fill amount
        isCooldown = false; // Cooldown complete
    }

    private float CalculateDamage(float baseDamage)
    {
        // Berechne Schaden basierend auf verschiedenen Faktoren
        float damage = baseDamage;

        // Beispiel für kritische Trefferchance und Bonus
        bool isCrit = Random.Range(0, 100) < playerController.critChance;
        if (isCrit)
        {
            damage *= playerController.critDamageBonus;
        }

        return damage;
    }


    void CastFireball()
    {
        Debug.Log("Casting Fireball!");

        if (fireballPrefab != null && playerController != null)
        {
            playerController.currentMana -= skill.manaCost;
            playerController.UpdateManaBar();

            GameObject fireball = Instantiate(fireballPrefab, projectileSpawnPoint.position, Quaternion.identity);

            if (fireball != null)
            {
                Debug.Log("Fireball instantiated.");
                Vector3 direction = GetMouseDirection();
                Projectile projectile = fireball.GetComponent<Projectile>();

                if (projectile != null)
                {
                    projectile.Initialize(direction, skill.damageMultiplier);

                    // Check if the fireball hits an enemy
                    RaycastHit hit;
                    if (Physics.Raycast(projectile.transform.position, direction, out hit, skill.attackRange))
                    {
                        EnemyController enemy = hit.collider.GetComponent<EnemyController>();
                        if (enemy != null)
                        {
                            float finalDamage = skill.damageMultiplier * playerController.maxDamage;
                            bool isCrit = Random.Range(0, 100) < playerController.critChance;
                            if (isCrit)
                            {
                                finalDamage *= playerController.critDamageBonus;
                            }

                            enemy.TakeDamage(finalDamage);

                            // Use the PlayerController method to show the damage popup
                            playerController.ShowDamagePopup((int)finalDamage, enemy.transform.position, isCrit);
                        }
                    }
                }
                else
                {
                    Debug.LogError("Projectile component not found on fireball prefab.");
                }
            }
            else
            {
                Debug.LogError("Fireball prefab could not be instantiated.");
            }
        }
    }

    void CastManashot()
    {
        Debug.Log("Casting Manashot!");

        if (manashotPrefab != null && playerController != null)
        {
            playerController.currentMana -= skill.manaCost;
            playerController.UpdateManaBar();

            GameObject manashot = Instantiate(manashotPrefab, projectileSpawnPoint.position, Quaternion.identity);
            Vector3 direction = GetMouseDirection();
            Projectile projectile = manashot.GetComponent<Projectile>();

            if (projectile != null)
            {
                projectile.Initialize(direction, skill.damageMultiplier);

                // Check if the manashot hits an enemy
                RaycastHit hit;
                if (Physics.Raycast(projectile.transform.position, direction, out hit, skill.attackRange))
                {
                    EnemyController enemy = hit.collider.GetComponent<EnemyController>();
                    if (enemy != null)
                    {
                        float finalDamage = skill.damageMultiplier * playerController.maxDamage;
                        bool isCrit = Random.Range(0, 100) < playerController.critChance;
                        if (isCrit)
                        {
                            finalDamage *= playerController.critDamageBonus;
                        }

                        enemy.TakeDamage(finalDamage);

                        // Use the PlayerController method to show the damage popup
                        playerController.ShowDamagePopup((int)finalDamage, enemy.transform.position, isCrit);
                    }
                }
            }
            else
            {
                Debug.LogError("Projectile component not found on Manashot prefab.");
            }
        }
        else
        {
            Debug.LogError("Manashot prefab is not assigned or PlayerController is missing.");
        }
    }

    void CastIceAttack()
    {
        Debug.Log("Casting Ice Attack!");

        if (iceAttackPrefab != null && playerController != null)
        {
            playerController.currentMana -= skill.manaCost;
            playerController.UpdateManaBar();

            GameObject iceAttack = Instantiate(iceAttackPrefab, projectileSpawnPoint.position, Quaternion.identity);

            // Logic for dealing damage and slowing enemies
            Collider[] hitColliders = Physics.OverlapSphere(iceAttack.transform.position, skill.attackRange);
            foreach (Collider hitCollider in hitColliders)
            {
                EnemyController enemy = hitCollider.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    float finalDamage = skill.damageMultiplier * playerController.maxDamage;
                    bool isCrit = Random.Range(0, 100) < playerController.critChance;
                    if (isCrit)
                    {
                        finalDamage *= playerController.critDamageBonus;
                    }

                    enemy.TakeDamage(finalDamage);

                    // Use the PlayerController method to show the damage popup
                    playerController.ShowDamagePopup((int)finalDamage, enemy.transform.position, isCrit);

                    // Slow the enemy
                    // enemy.Slow(skill.damageMultiplier); // Assuming Slow() is a method in EnemyController
                }
            }

            Destroy(iceAttack, 2f); // Destroy the ice attack after 2 seconds
        }
        else
        {
            Debug.LogError("Ice Attack prefab is not assigned or PlayerController is missing.");
        }
    }

    void CastLightning()
    {
        Debug.Log("Casting Lightning!");

        if (lightningPrefab != null && playerController != null)
        {
            playerController.currentMana -= skill.manaCost;
            playerController.UpdateManaBar();

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f))
            {
                GameObject lightning = Instantiate(lightningPrefab, hit.point, Quaternion.identity);
                EnemyController enemy = hit.collider.GetComponent<EnemyController>();

                if (enemy != null)
                {
                    float finalDamage = skill.damageMultiplier * playerController.maxDamage;
                    bool isCrit = Random.Range(0, 100) < playerController.critChance;
                    if (isCrit)
                    {
                        finalDamage *= playerController.critDamageBonus;
                    }

                    enemy.TakeDamage(finalDamage);

                    // Use the PlayerController method to show the damage popup
                    playerController.ShowDamagePopup((int)finalDamage, enemy.transform.position, isCrit);
                }

                Destroy(lightning, 0.5f); // Destroy the lightning after 0.5 seconds
            }
        }
        else
        {
            Debug.LogError("Lightning prefab is not assigned or PlayerController is missing.");
        }
    }

    void CastChainLightning()
    {
        Debug.Log("Casting Chain Lightning!");

        if (chainLightningPrefab != null && playerController != null)
        {
            playerController.currentMana -= skill.manaCost;
            playerController.UpdateManaBar();

            // Instanziiere das Blitzobjekt an der Position des Spielers
            GameObject chainLightning = Instantiate(chainLightningPrefab, projectileSpawnPoint.position, Quaternion.identity);
            Vector3 direction = GetMouseDirection(); // Richtungsberechnung für das Abschießen

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f))
            {
                // Zuerst den Gegner in der Nähe des Treffers finden
                EnemyController firstEnemy = hit.collider.GetComponent<EnemyController>();
                if (firstEnemy != null)
                {
                    // Führe hier die Logik für den Blitzschaden an den ersten Gegner aus
                    float finalDamage = CalculateDamage(skill.damageMultiplier * playerController.maxDamage);
                    bool isCrit = Random.Range(0, 100) < playerController.critChance;
                    if (isCrit)
                    {
                        finalDamage *= playerController.critDamageBonus;
                    }

                    // Schaden auf den ersten Gegner anwenden
                    firstEnemy.TakeDamage(finalDamage);
                    playerController.ShowDamagePopup((int)finalDamage, firstEnemy.transform.position, isCrit);

                    // Der Blitz überspringt zu den benachbarten Gegnern
                    EnemyController currentEnemy = firstEnemy;
                    for (int i = 0; i < 2; i++) // Der Blitz springt maximal zu 2 Gegnern
                    {
                        // Finde den nächsten benachbarten Gegner innerhalb des Umkreises
                        Collider[] nearbyEnemies = Physics.OverlapSphere(currentEnemy.transform.position, skill.attackRange);
                        EnemyController nextEnemy = null;

                        foreach (Collider nearbyCollider in nearbyEnemies)
                        {
                            EnemyController potentialEnemy = nearbyCollider.GetComponent<EnemyController>();
                            if (potentialEnemy != null && potentialEnemy != currentEnemy) // Achte darauf, dass der gleiche Gegner nicht erneut getroffen wird
                            {
                                nextEnemy = potentialEnemy;
                                break; // Wenn ein benachbarter Gegner gefunden wurde, springe zum nächsten
                            }
                        }

                        if (nextEnemy != null)
                        {
                            // Update die Position des Blitzes zu dem neuen Gegner
                            chainLightning.transform.position = nextEnemy.transform.position;

                            // Schaden auf den nächsten Gegner anwenden
                            nextEnemy.TakeDamage(finalDamage);
                            playerController.ShowDamagePopup((int)finalDamage, nextEnemy.transform.position, isCrit);

                            // Setze den aktuellen Gegner auf den nächsten Gegner, damit der Blitz weiter springt
                            currentEnemy = nextEnemy;
                        }
                        else
                        {
                            // Wenn keine weiteren Gegner gefunden werden, brich die Schleife ab
                            break;
                        }
                    }

                    // Zerstöre das Blitzobjekt nach den Sprüngen
                    Destroy(chainLightning, 0.5f); // Zerstöre das Blitzobjekt nach einer kurzen Verzögerung
                }
            }
            else
            {
                Debug.LogError("Chain Lightning konnte keinen Gegner finden.");
            }
        }
        else
        {
            Debug.LogError("Chain Lightning prefab is not assigned or PlayerController is missing.");
        }
    }



    void CastTeleport()
    {
        Debug.Log("Casting Teleport!");

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f))
            {
                player.agent.Warp(hit.point);
            }
        }
    }

    void PerformMeleeSlash()
    {
        Debug.Log("Performing Melee Slash!");

        if (playerController != null)
        {
            playerController.currentMana -= skill.manaCost;
            playerController.UpdateManaBar();

            // Erstelle eine Kollisionserkennung im Bereich des Melee-Angriffs
            Collider[] hitColliders = Physics.OverlapSphere(projectileSpawnPoint.position, skill.attackRange);
            foreach (Collider hitCollider in hitColliders)
            {
                EnemyController enemy = hitCollider.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    // Berechne die Richtung des Angriffs
                    Vector3 attackDirection = (enemy.transform.position - projectileSpawnPoint.position).normalized;
                    Quaternion attackRotation = Quaternion.LookRotation(attackDirection);

                    // Instanziere das "Electro Slash" VFX-Objekt am Ort des Angriffs
                    if (electroSlashPrefab != null)
                    {
                        // Erstelle eine Instanz des VFX mit der berechneten Rotation
                        GameObject electroSlashInstance = Instantiate(
                            electroSlashPrefab,
                            projectileSpawnPoint.position,
                            attackRotation
                        );

                        // Zerstöre das VFX nach der Dauer des Partikelsystems
                        ParticleSystem particleSystem = electroSlashInstance.GetComponent<ParticleSystem>();
                        if (particleSystem != null)
                        {
                            Destroy(electroSlashInstance, particleSystem.main.duration);
                        }
                        else
                        {
                            // Falls kein Partikelsystem gefunden wird, zerstöre es nach einer Standardzeit
                            Destroy(electroSlashInstance, 1.0f);
                        }
                    }
                    else
                    {
                        Debug.LogError("Electro slash prefab is not assigned.");
                    }

                    // Berechne den Schaden mit kritischen Treffern
                    float finalDamage = CalculateDamage(skill.damageMultiplier * playerController.maxDamage);
                    bool isCrit = Random.Range(0, 100) < playerController.critChance;
                    if (isCrit)
                    {
                        finalDamage *= playerController.critDamageBonus;
                    }

                    // Schaden an den Gegner anwenden
                    enemy.TakeDamage(finalDamage);

                    // Zeige den Schaden als Popup an
                    playerController.ShowDamagePopup((int)finalDamage, enemy.transform.position, isCrit);

                    // Breche die Schleife nach dem ersten Treffer ab, um nur einen Gegner anzugreifen
                    //break;
                }
            }
        }
    }

    Vector3 GetMouseDirection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Vector3 direction = (hit.point - projectileSpawnPoint.position).normalized;
            return direction;
        }
        else
        {
            return transform.forward;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
    }
}
