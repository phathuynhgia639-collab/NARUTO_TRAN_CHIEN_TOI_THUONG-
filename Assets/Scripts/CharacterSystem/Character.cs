using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Character - Nhân vật chính (Hashirama, Player...)
/// Tích hợp: HP, Mana, Buff, Skill, Combat
/// </summary>
public class Character : MonoBehaviour
{
    [Header("HP Stats")]
    [SerializeField] protected float maxHP = 1000f;
    [SerializeField] protected float currentHP;

    [Header("Damage")]
    [SerializeField] protected float baseDamage = 100f;

    [Header("Movement")]
    [SerializeField] protected float moveSpeed = 6f;
    [SerializeField] protected float attackRange = 3f;

    [Header("UI")]
    [SerializeField] protected UnityEngine.UI.Slider healthBar;

    // Systems
    protected ManaSystem manaSystem;
    protected BuffSystem buffSystem;
    protected SkillSystem skillSystem;
    protected CombatSystem combatSystem;

    // Current target
    protected Character currentTarget;
    protected float nextAttackTime = 0f;
    protected bool isDead = false;

    protected virtual void Awake()
    {
        // Initialize systems
        gameObject.AddComponent<ManaSystem>();
        gameObject.AddComponent<BuffSystem>();
        gameObject.AddComponent<SkillSystem>();
        gameObject.AddComponent<CombatSystem>();
    }

    protected virtual void Start()
    {
        currentHP = maxHP;
        
        // Get system components
        manaSystem = GetComponent<ManaSystem>();
        buffSystem = GetComponent<BuffSystem>();
        skillSystem = GetComponent<SkillSystem>();
        combatSystem = GetComponent<CombatSystem>();

        // Setup health bar
        if (healthBar != null)
        {
            healthBar.maxValue = maxHP;
            healthBar.value = currentHP;
        }

        Debug.Log($"{gameObject.name} đã khởi tạo! HP: {currentHP}, Mana: {manaSystem.GetMaxMana()}");
    }

    protected virtual void Update()
    {
        if (isDead) return;

        // Update target
        FindClosestTarget();

        // Combat
        if (currentTarget != null && !currentTarget.IsDead())
        {
            float distance = Vector3.Distance(transform.position, currentTarget.transform.position);

            if (distance <= attackRange)
            {
                // Try attack
                combatSystem.TryAttack(currentTarget);
            }
            else
            {
                // Move towards target
                MoveTowards(currentTarget.transform.position);
            }
        }

        // Check if dead
        if (currentHP <= 0f)
        {
            Die();
        }
    }

    /// <summary>
    /// Find closest enemy target
    /// </summary>
    protected virtual void FindClosestTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            currentTarget = null;
            return;
        }

        Character closest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;
            Character enemyChar = enemy.GetComponent<Character>();
            if (enemyChar == null || enemyChar.IsDead()) continue;

            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = enemyChar;
            }
        }

        currentTarget = closest;
    }

    /// <summary>
    /// Move towards target position
    /// </summary>
    protected virtual void MoveTowards(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        
        // Apply speed buff
        float speedMultiplier = buffSystem.GetSpeedMultiplier();
        transform.position += direction * moveSpeed * speedMultiplier * Time.deltaTime;
    }

    /// <summary>
    /// Take damage (with buff checks)
    /// </summary>
    public virtual void TakeDamage(float damage)
    {
        // Check if immune to damage
        if (buffSystem.IsImmuneToDamage())
        {
            Debug.Log($"{gameObject.name} miễn dịch sát thương!");
            return;
        }

        // Apply damage reduction from buffs
        float reduction = buffSystem.GetDamageReduction();
        float actualDamage = damage * (1f - reduction);

        // Cap damage (like Hashirama's SK4: max 15% HP)
        float maxDamagePerHit = maxHP * 0.15f;
        actualDamage = Mathf.Min(actualDamage, maxDamagePerHit);

        currentHP -= actualDamage;

        // Update health bar
        if (healthBar != null)
        {
            healthBar.value = currentHP;
        }

        Debug.Log($"{gameObject.name} nhận {actualDamage} sát thương. HP còn lại: {currentHP}");
    }

    /// <summary>
    /// Character dies
    /// </summary>
    public virtual void Die()
    {
        isDead = true;
        Debug.Log($"{gameObject.name} đã bị tiêu diệt!");
        
        if (healthBar != null)
        {
            Destroy(healthBar.gameObject);
        }
        
        Destroy(gameObject);
    }

    /// <summary>
    /// Getters
    /// </summary>
    public float GetCurrentHP() => currentHP;
    public float GetMaxHP() => maxHP;
    public float GetHPPercent() => currentHP / maxHP;
    public bool IsDead() => isDead;
    public Character GetTarget() => currentTarget;
    public float GetBaseDamage() => baseDamage;

    /// <summary>
    /// Setters
    /// </summary>
    public void SetBaseDamage(float newDamage) => baseDamage = newDamage;
    public void SetMoveSpeed(float newSpeed) => moveSpeed = newSpeed;
}
