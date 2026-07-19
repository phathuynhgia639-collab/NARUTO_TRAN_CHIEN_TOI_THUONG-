using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Quản lý 4 Skill của nhân vật
/// - Skill 1: Auto trigger khi đủ mana
/// - Skill 2-3: Cooldown based
/// - Skill 4: Passive (auto trigger when condition met)
/// </summary>
public class SkillSystem : MonoBehaviour
{
    [SerializeField] private SkillData[] skills = new SkillData[4];
    
    private Character character;
    private ManaSystem manaSystem;
    private BuffSystem buffSystem;

    private void OnEnable()
    {
        character = GetComponent<Character>();
        manaSystem = GetComponent<ManaSystem>();
        buffSystem = GetComponent<BuffSystem>();

        // Initialize skills
        InitializeSkills();
    }

    private void Update()
    {
        // Update cooldowns
        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i] != null)
            {
                skills[i].UpdateCooldown(Time.deltaTime);

                // Auto trigger SK1 when have enough mana
                if (i == 0 && manaSystem.CanCastSkill(skills[i].manaCost))
                {
                    CastSkill(i);
                }
            }
        }
    }

    /// <summary>
    /// Initialize all skills (override this in character class)
    /// </summary>
    protected virtual void InitializeSkills()
    {
        // Base implementation - will be overridden by character
    }

    /// <summary>
    /// Cast skill by index (0-3)
    /// </summary>
    public bool CastSkill(int skillIndex)
    {
        if (skillIndex < 0 || skillIndex >= skills.Length)
        {
            Debug.LogWarning($"Invalid skill index: {skillIndex}");
            return false;
        }

        SkillData skill = skills[skillIndex];
        if (skill == null)
        {
            Debug.LogWarning($"Skill {skillIndex} is null");
            return false;
        }

        // Check if skill is ready
        if (!skill.IsReady())
        {
            Debug.LogWarning($"Skill {skillIndex} on cooldown");
            return false;
        }

        // Check if have enough mana
        if (!manaSystem.ConsumeMana(skill.manaCost))
        {
            Debug.LogWarning($"Not enough mana for skill {skillIndex}. Need: {skill.manaCost}, Have: {manaSystem.GetCurrentMana()}");
            return false;
        }

        // Execute skill
        ExecuteSkill(skillIndex);

        // Reset cooldown
        skill.ResetCooldown();

        Debug.Log($"Cast skill {skillIndex}: {skill.skillName}");
        return true;
    }

    /// <summary>
    /// Execute skill logic (override in character class)
    /// </summary>
    protected virtual void ExecuteSkill(int skillIndex)
    {
        SkillData skill = skills[skillIndex];
        
        // Default: deal damage
        if (character.GetTarget() != null)
        {
            float damage = CalculateDamage(skill);
            character.GetTarget().GetComponent<Character>().TakeDamage(damage);
        }

        // Apply buff
        if (skill.buffOnCast != null)
        {
            foreach (var buffName in skill.buffOnCast)
            {
                buffSystem.AddBuffByName(buffName, skill.skillDuration);
            }
        }
    }

    /// <summary>
    /// Calculate actual damage (with buffs)
    /// </summary>
    protected float CalculateDamage(SkillData skill)
    {
        float damage = skill.damageAmount;

        // Apply damage multiplier from buffs
        damage *= buffSystem.GetDamageMultiplier();

        // Apply damage type
        switch (skill.damageType.ToLower())
        {
            case "% hp":
                damage = character.GetCurrentHP() * (skill.damageAmount / 100f);
                break;
            case "% maxhp":
                damage = character.GetMaxHP() * (skill.damageAmount / 100f);
                break;
        }

        return damage;
    }

    /// <summary>
    /// Get skill by index
    /// </summary>
    public SkillData GetSkill(int index)
    {
        if (index >= 0 && index < skills.Length)
            return skills[index];
        return null;
    }

    /// <summary>
    /// Set skill (for character initialization)
    /// </summary>
    public void SetSkill(int index, SkillData skill)
    {
        if (index >= 0 && index < skills.Length)
            skills[index] = skill;
    }

    /// <summary>
    /// Manual cast skill (for player input)
    /// </summary>
    public bool TryManualCast(int skillIndex)
    {
        return CastSkill(skillIndex);
    }
}
