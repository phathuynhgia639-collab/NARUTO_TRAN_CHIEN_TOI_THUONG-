using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// BuffSystem v2 - Quản lý Buff/Debuff với phân biệt rõ ràng:
/// 
/// KHÁI NIỆM CỐT LÕI:
/// 1) Kháng hiệu ứng (Effect Resistance): Chặn TẤT CẢ hiệu ứng (CC + Debuff), nhưng Damage vẫn trúng
///    - VD: Bất Hoại (Invulnerable) → chặn Trói, Câm, Giảm Heal/Mana, Giảm Tốc...
///    
/// 2) Kháng hiệu quả xấu (CC Resistance): Chỉ chặn CC thuần túy (Trói, Câm, Đóng băng, Choáng, Đẩy)
///    - Debuff thông thường (Giảm Heal, Giảm Mana, Giảm Tốc) VẪN dính
///    - VD: Hashirama Tiên Pháp có 100% CC Resistance → miễn Trói/Câm, nhưng Giảm Heal từ Lục Đạo vẫn dính
///
/// BUFF TYPES:
/// - CC (Crowd Control): Trói, Câm, Đóng Băng, Choáng, Đẩy Lùi
/// - Debuff: Giảm Sát Thương, Giảm Heal, Giảm Mana, Giảm Tốc...
/// - Buff: Tăng Sát Thương, Miễn Hiệu Ứng, Miễn CC...
/// </summary>
public class BuffSystem : MonoBehaviour
{
    /// <summary>
    /// Enum để phân loại buff
    /// </summary>
    public enum BuffType
    {
        /// Crowd Control (CC) - thuần túy khống chế
        CrowdControl,      // Trói, Câm, Đóng Băng, Choáng, Đẩy Lùi
        
        /// Debuff thông thường - không phải CC
        Debuff,            // Giảm Heal, Giảm Mana, Giảm Tốc, Giảm Sát Thương...
        
        /// Buff dương tính
        Buff,              // Tăng Sát Thương, Miễn Hiệu Ứng, Miễn CC...
        
        /// Hiệu ứng đặc biệt
        Special            // Bất Hoại, Tiên Pháp, v.v...
    }

    [System.Serializable]
    public class Buff
    {
        public string buffName;
        public BuffType buffType = BuffType.Buff;
        public float duration; // -1 = infinite
        public float damageMultiplier = 1f; // 1.5f = 150%
        public float damageReduction = 0f; // 0.3f = 30% reduction
        public float speedMultiplier = 1f; // 0.5f = 50% slower
        public float healReduction = 0f; // 0.5f = 50% heal reduction
        public float manaReduction = 0f; // 0.5f = 50% mana reduction
        
        /// Kháng hiệu ứng: chặn TẤT CẢ hiệu ứng (CC + Debuff), damage vẫn trúng
        public bool isEffectResistance = false;
        
        /// Kháng hiệu quả xấu (CC): chỉ chặn CC, debuff thường vẫn dính
        public bool isCCResistance = false;
        
        /// Miễn Bất Hoại
        public bool isImmuneToDamage = false;
        
        public float remainingTime;

        public Buff(string name, BuffType type = BuffType.Buff, float dur = -1f)
        {
            buffName = name;
            buffType = type;
            duration = dur;
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
                    Debug.Log($"🕐 {character.name} mất buff: {activeBuffs[i].buffName}");
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
        Debug.Log($"✨ {character.name} nhận buff: {buff.buffName} ({buff.buffType})");
    }

    /// <summary>
    /// Add buff by name (helper)
    /// </summary>
    public void AddBuffByName(string buffName, float duration)
    {
        switch (buffName.ToLower())
        {
            // === SPECIAL ===
            case "bất hoại": // Immune to all damage
                {
                    var buff = new Buff("Bất Hoại", BuffType.Special, duration);
                    buff.isImmuneToDamage = true;
                    buff.isEffectResistance = true; // Chặn tất cả hiệu ứng
                    AddBuff(buff);
                }
                break;

            // === CC RESISTANCE ===
            case "miễn cc": // Immune to crowd control only (not debuff)
                {
                    var buff = new Buff("Miễn CC", BuffType.Buff, duration);
                    buff.isCCResistance = true;
                    AddBuff(buff);
                }
                break;

            case "miễn hiệu ứng": // Immune to ALL effects (CC + Debuff), but damage still hits
                {
                    var buff = new Buff("Miễn Hiệu Ứng", BuffType.Special, duration);
                    buff.isEffectResistance = true;
                    AddBuff(buff);
                }
                break;

            // === CC (Crowd Control) ===
            case "trói chân": // Root/Stun
                {
                    var buff = new Buff("Trói Chân", BuffType.CrowdControl, duration);
                    buff.speedMultiplier = 0f; // Khóa hoàn toàn
                    AddBuff(buff);
                }
                break;

            case "câm lặng": // Silence
                {
                    var buff = new Buff("Câm Lặng", BuffType.CrowdControl, duration);
                    // Khóa skill (handle trong SkillSystem)
                    AddBuff(buff);
                }
                break;

            case "đóng băng": // Freeze
                {
                    var buff = new Buff("Đóng Băng", BuffType.CrowdControl, duration);
                    buff.speedMultiplier = 0f;
                    AddBuff(buff);
                }
                break;

            case "choáng": // Stun
                {
                    var buff = new Buff("Choáng", BuffType.CrowdControl, duration);
                    buff.speedMultiplier = 0f;
                    AddBuff(buff);
                }
                break;

            case "đẩy lùi": // Knockback (bị push lùi)
                {
                    var buff = new Buff("Đẩy Lùi", BuffType.CrowdControl, duration);
                    AddBuff(buff);
                }
                break;

            // === DEBUFF ===
            case "giảm heal": // Reduce healing received
                {
                    var buff = new Buff("Giảm Heal", BuffType.Debuff, duration);
                    buff.healReduction = 0.5f; // 50% reduction
                    AddBuff(buff);
                }
                break;

            case "giảm mana": // Reduce mana received
                {
                    var buff = new Buff("Giảm Mana", BuffType.Debuff, duration);
                    buff.manaReduction = 0.5f; // 50% reduction
                    AddBuff(buff);
                }
                break;

            case "chậm": // Slow
                {
                    var buff = new Buff("Chậm", BuffType.Debuff, duration);
                    buff.speedMultiplier = 0.5f; // 50% slower
                    AddBuff(buff);
                }
                break;

            case "giảm sát thương": // Reduce damage output
                {
                    var buff = new Buff("Giảm Sát Thương", BuffType.Debuff, duration);
                    buff.damageMultiplier = 0.7f; // 70% damage
                    AddBuff(buff);
                }
                break;

            case "giảm giáp": // Reduce armor (increase damage taken)
                {
                    var buff = new Buff("Giảm Giáp", BuffType.Debuff, duration);
                    buff.damageReduction = -0.3f; // Nhận 30% sát thương thêm
                    AddBuff(buff);
                }
                break;

            // === BUFF ===
            case "tăng sát thương": // Increase damage
                {
                    var buff = new Buff("Tăng Sát Thương", BuffType.Buff, duration);
                    buff.damageMultiplier = 1.5f; // 150% damage
                    AddBuff(buff);
                }
                break;

            case "tăng tốc": // Haste
                {
                    var buff = new Buff("Tăng Tốc", BuffType.Buff, duration);
                    buff.speedMultiplier = 1.5f; // 150% speed
                    AddBuff(buff);
                }
                break;

            case "tăng giáp": // Armor up
                {
                    var buff = new Buff("Tăng Giáp", BuffType.Buff, duration);
                    buff.damageReduction = 0.2f; // 20% damage reduction
                    AddBuff(buff);
                }
                break;

            default:
                Debug.LogWarning($"⚠️ Buff '{buffName}' không tìm thấy!");
                break;
        }
    }

    /// <summary>
    /// Check xem hiệu ứng có bị chặn không
    /// </summary>
    public bool IsEffectBlocked(BuffType effectType)
    {
        // Nếu có Kháng Hiệu Ứng (Effect Resistance) → chặn TẤT CẢ
        if (HasEffectResistance())
        {
            return true;
        }

        // Nếu là CC và có Kháng CC → chặn
        if (effectType == BuffType.CrowdControl && HasCCResistance())
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Check xem có Kháng Hiệu Ứng không
    /// </summary>
    public bool HasEffectResistance()
    {
        foreach (var buff in activeBuffs)
        {
            if (buff.isEffectResistance)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Check xem có Kháng CC không
    /// </summary>
    public bool HasCCResistance()
    {
        foreach (var buff in activeBuffs)
        {
            if (buff.isCCResistance)
                return true;
        }
        return false;
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
    /// Get heal reduction from all buffs (Debuff)
    /// </summary>
    public float GetHealReduction()
    {
        float reduction = 0f;
        foreach (var buff in activeBuffs)
        {
            reduction += buff.healReduction;
        }
        return Mathf.Clamp(reduction, 0f, 1f);
    }

    /// <summary>
    /// Get mana reduction from all buffs (Debuff)
    /// </summary>
    public float GetManaReduction()
    {
        float reduction = 0f;
        foreach (var buff in activeBuffs)
        {
            reduction += buff.manaReduction;
        }
        return Mathf.Clamp(reduction, 0f, 1f);
    }

    /// <summary>
    /// Check if immune to damage (Bất Hoại)
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
    /// Get list of active buffs (for UI)
    /// </summary>
    public List<Buff> GetActiveBuffs() => new List<Buff>(activeBuffs);

    /// <summary>
    /// Get buff count
    /// </summary>
    public int GetBuffCount() => activeBuffs.Count;
}
