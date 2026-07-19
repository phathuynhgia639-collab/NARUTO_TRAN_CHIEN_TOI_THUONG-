using UnityEngine;

/// <summary>
/// Quản lý Combat logic
/// - Tính sát thương
/// - Kiểm tra buff
/// - Xử lý kết quả tấn công
/// </summary>
public class CombatSystem : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1f;
    
    private float lastAttackTime;
    private Character character;
    private ManaSystem manaSystem;
    private BuffSystem buffSystem;

    private void OnEnable()
    {
        character = GetComponent<Character>();
        manaSystem = GetComponent<ManaSystem>();
        buffSystem = GetComponent<BuffSystem>();
        lastAttackTime = -attackCooldown;
    }

    /// <summary>
    /// Attempt attack on target
    /// </summary>
    public bool TryAttack(Character target)
    {
        if (target == null || target.IsDead())
            return false;

        // Check cooldown
        if (Time.time - lastAttackTime < attackCooldown)
            return false;

        // Check distance
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance > attackRange)
            return false;

        // Check if immune to crowd control
        if (buffSystem.IsImmuneToCrowd())
        {
            // Can't attack if crowd controlled (simplified)
        }

        // Calculate damage
        float damage = CalculateDamage(target);

        // Apply damage
        target.TakeDamage(damage);

        // Restore mana
        manaSystem.RestoreManaOnAttack(1);

        lastAttackTime = Time.time;

        Debug.Log($"{character.name} attacked {target.name} for {damage} damage");
        return true;
    }

    /// <summary>
    /// Calculate actual damage with all modifiers
    /// </summary>
    private float CalculateDamage(Character target)
    {
        float damage = baseDamage;

        // Apply attacker's damage buff
        damage *= buffSystem.GetDamageMultiplier();

        // Apply target's damage reduction
        BuffSystem targetBuffSystem = target.GetComponent<BuffSystem>();
        float reduction = targetBuffSystem.GetDamageReduction();
        damage *= (1f - reduction);

        // Cap damage (like Hashirama's SK4: max 15% HP)
        float maxDamage = character.GetMaxHP() * 0.15f;
        damage = Mathf.Min(damage, maxDamage);

        return Mathf.Max(1f, damage); // Min 1 damage
    }

    /// <summary>
    /// Take damage (with buff checks)
    /// </summary>
    public float TakeDamage(float damage)
    {
        // Check if immune to damage
        if (buffSystem.IsImmuneToDamage())
        {
            Debug.Log($"{character.name} is immune to damage!");
            return 0f;
        }

        // Apply damage reduction
        float reduction = buffSystem.GetDamageReduction();
        float actualDamage = damage * (1f - reduction);

        return actualDamage;
    }

    /// <summary>
    /// Get attack range
    /// </summary>
    public float GetAttackRange() => attackRange;

    /// <summary>
    /// Get base damage
    /// </summary>
    public float GetBaseDamage() => baseDamage;

    /// <summary>
    /// Set base damage (for items, buff, etc)
    /// </summary>
    public void SetBaseDamage(float damage)
    {
        baseDamage = damage;
    }
}
