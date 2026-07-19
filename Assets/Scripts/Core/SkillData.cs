using UnityEngine;

/// <summary>
/// Template for skill data
/// Bạn sẽ điền thông tin skill Hashirama vào đây
/// </summary>
[System.Serializable]
public class SkillData
{
    [Header("Basic Info")]
    public string skillName;
    public int skillSlot; // 1, 2, 3, 4
    public string description;

    [Header("Cost & Cooldown")]
    public float manaCost;
    public float cooldown; // 0 = no cooldown (auto trigger)
    public float currentCooldown;

    [Header("Damage")]
    public float damageAmount;
    public string damageType; // "flat", "% HP", "% MaxHP"

    [Header("Buff on Cast")]
    public string[] buffOnCast; // VD: ["Bất Hoại 3s", "Tăng Sát Thương 5s"]

    [Header("Trigger")]
    public string triggerType; // "manual", "auto_mana", "cooldown_ready", "passive"
    public float triggerCondition; // VD: 0.5f = trigger when HP < 50%

    [Header("Duration")]
    public float skillDuration; // Duration of effect
    public bool isInstant; // true = instant, false = over time

    public SkillData()
    {
        skillName = "New Skill";
        skillSlot = 1;
        manaCost = 1000f;
        cooldown = 0f;
        currentCooldown = 0f;
        damageAmount = 100f;
        damageType = "flat";
        triggerType = "manual";
        isInstant = true;
    }

    /// <summary>
    /// Check if skill is ready to cast
    /// </summary>
    public bool IsReady()
    {
        return currentCooldown <= 0f;
    }

    /// <summary>
    /// Update cooldown
    /// </summary>
    public void UpdateCooldown(float deltaTime)
    {
        if (currentCooldown > 0f)
            currentCooldown -= deltaTime;
    }

    /// <summary>
    /// Reset cooldown after cast
    /// </summary>
    public void ResetCooldown()
    {
        currentCooldown = cooldown;
    }
}
