using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Quản lý Buff/Debuff trên nhân vật
/// - Buff có thời gian (VD: Bất Hoại 3 giây)
/// - Buff có hiệu ứng (tăng sát thương, giảm sát thương, khóa skill, ...)
/// - Buff cộng dồn được
/// </summary>
public class BuffSystem : MonoBehaviour
{
    [System.Serializable]
    public class Buff
    {
        public string buffName;
        public float duration; // -1 = infinite
        public float damageMultiplier = 1f; // 1.5f = 150%
        public float damageReduction = 0f; // 0.3f = 30% reduction
        public float speedMultiplier = 1f; // 0.5f = 50% slower
        public bool isImmuneToDamage = false; // Bất Hoại
        public bool isImmuneToCrowd = false; // Miễn khống chế
        public float remainingTime;

        public Buff(string name, float dur, float damMul = 1f, float damRed = 0f, 
            float spdMul = 1f, bool immDam = false, bool immCrowd = false)
        {
            buffName = name;
            duration = dur;
            damageMultiplier = damMul;
            damageReduction = damRed;
            speedMultiplier = spdMul;
            isImmuneToDamage = immDam;
            isImmuneToCrowd = immCrowd;
            remainingTime = dur;
        }
    }

    private List<Buff> activeBuffs = new List<Buff>();
    private Character character;

    private void OnEnable()
    {
        character = GetComponent<Character>();
    }

    private void Update()
    {
        // Update buff durations
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            if (activeBuffs[i].duration > 0)
            {
                activeBuffs[i].remainingTime -= Time.deltaTime;
                if (activeBuffs[i].remainingTime <= 0)
                {
                    activeBuffs.RemoveAt(i);
                }
            }
        }
    }

    /// <summary>
    /// Add a buff to character
    /// </summary>
    public void AddBuff(Buff buff)
    {
        activeBuffs.Add(buff);
        Debug.Log($"{character.name} received buff: {buff.buffName}");
    }

    /// <summary>
    /// Add buff by name (helper)
    /// </summary>
    public void AddBuffByName(string buffName, float duration)
    {
        switch (buffName.ToLower())
        {
            case "bất hoại": // Invulnerable
                AddBuff(new Buff("Bất Hoại", duration, isImmuneToDamage: true));
                break;
            case "miễn khống chế": // Immune to crowd control
                AddBuff(new Buff("Miễn Khống Chế", duration, isImmuneToCrowd: true));
                break;
            case "tăng sát thương": // Increase damage
                AddBuff(new Buff("Tăng Sát Thương", duration, damMul: 1.5f));
                break;
            case "giảm sát thương": // Reduce damage taken
                AddBuff(new Buff("Giảm Sát Thương", duration, damRed: 0.3f));
                break;
            case "chậm": // Slow
                AddBuff(new Buff("Chậm", duration, spdMul: 0.5f));
                break;
            default:
                Debug.LogWarning($"Buff '{buffName}' not found!");
                break;
        }
    }

    /// <summary>
    /// Remove all buffs of a type
    /// </summary>
    public void RemoveBuffByName(string buffName)
    {
        activeBuffs.RemoveAll(b => b.buffName.Equals(buffName));
    }

    /// <summary>
    /// Clear all buffs
    /// </summary>
    public void ClearAllBuffs()
    {
        activeBuffs.Clear();
    }

    /// <summary>
    /// Get total damage multiplier from all buffs
    /// </summary>
    public float GetDamageMultiplier()
    {
        float multiplier = 1f;
        foreach (var buff in activeBuffs)
        {
            multiplier *= buff.damageMultiplier;
        }
        return multiplier;
    }

    /// <summary>
    /// Get total damage reduction from all buffs
    /// </summary>
    public float GetDamageReduction()
    {
        float reduction = 0f;
        foreach (var buff in activeBuffs)
        {
            reduction += buff.damageReduction;
        }
        return Mathf.Clamp(reduction, 0f, 0.75f); // Max 75% reduction
    }

    /// <summary>
    /// Get speed multiplier from all buffs
    /// </summary>
    public float GetSpeedMultiplier()
    {
        float multiplier = 1f;
        foreach (var buff in activeBuffs)
        {
            multiplier *= buff.speedMultiplier;
        }
        return multiplier;
    }

    /// <summary>
    /// Check if immune to damage
    /// </summary>
    public bool IsImmuneToDamage()
    {
        foreach (var buff in activeBuffs)
        {
            if (buff.isImmuneToDamage)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Check if immune to crowd control
    /// </summary>
    public bool IsImmuneToCrowd()
    {
        foreach (var buff in activeBuffs)
        {
            if (buff.isImmuneToCrowd)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Get list of active buffs (for UI)
    /// </summary>
    public List<Buff> GetActiveBuffs() => new List<Buff>(activeBuffs);
}
