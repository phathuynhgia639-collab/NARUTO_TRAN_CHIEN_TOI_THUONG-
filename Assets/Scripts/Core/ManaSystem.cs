using UnityEngine;

/// <summary>
/// Quản lý Mana của nhân vật
/// - Pool Mana (tối đa tùy skill)
/// - Hồi Mana từ đòn thường, skill, buff
/// - % tăng hiệu quả hồi mana
/// </summary>
public class ManaSystem : MonoBehaviour
{
    [Header("Mana Stats")]
    [SerializeField] private float maxMana = 1000f;
    [SerializeField] private float currentMana;
    
    [Header("Mana Regen")]
    [SerializeField] private float baseRegenPerAttack = 20f; // 20 mana per hit
    [SerializeField] private float manaRegenMultiplier = 1f; // % tăng hiệu quả hồi mana (1 = 100%)
    
    private Character character;

    private void OnEnable()
    {
        character = GetComponent<Character>();
        currentMana = maxMana;
    }

    /// <summary>
    /// Restore mana on attack hit
    /// </summary>
    public void RestoreManaOnAttack(int hitCount = 1)
    {
        float regenAmount = baseRegenPerAttack * hitCount * manaRegenMultiplier;
        AddMana(regenAmount);
    }

    /// <summary>
    /// Consume mana for skill
    /// Returns: true if enough mana, false if not
    /// </summary>
    public bool ConsumeMana(float amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Add mana (from skill, buff, etc)
    /// </summary>
    public void AddMana(float amount)
    {
        currentMana = Mathf.Min(currentMana + amount, maxMana);
    }

    /// <summary>
    /// Set mana multiplier (from buff, equipment, etc)
    /// </summary>
    public void SetManaRegenMultiplier(float multiplier)
    {
        manaRegenMultiplier = multiplier;
    }

    /// <summary>
    /// Get current mana
    /// </summary>
    public float GetCurrentMana() => currentMana;

    /// <summary>
    /// Get max mana
    /// </summary>
    public float GetMaxMana() => maxMana;

    /// <summary>
    /// Get mana percentage (0-1)
    /// </summary>
    public float GetManaPercent() => currentMana / maxMana;

    /// <summary>
    /// Check if can cast skill (enough mana)
    /// </summary>
    public bool CanCastSkill(float cost)
    {
        return currentMana >= cost;
    }
}
