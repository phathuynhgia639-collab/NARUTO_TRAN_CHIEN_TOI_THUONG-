using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Character - Base class cho tất cả nhân vật (Hashirama, Enemy, v.v...)
/// Quản lý: HP, Death, Components (Buff, Mana, Combat, Effect)
/// </summary>
public class Character : MonoBehaviour
{
    [Header("Character Info")]
    [SerializeField] protected float maxHP = 1000f;
    [SerializeField] protected float currentHP = 1000f;
    [SerializeField] protected bool isDead = false;
    [SerializeField] protected Slider healthBar;

    [Header("Systems")]
    protected BuffSystem buffSystem;
    protected ManaSystem manaSystem;
    protected CombatSystem combatSystem;
    protected EffectSystem effectSystem;

    [Header("Target")]
    protected Character currentTarget;

    protected virtual void Awake()
    {
        // Get components
        buffSystem = GetComponent<BuffSystem>();
        manaSystem = GetComponent<ManaSystem>();
        combatSystem = GetComponent<CombatSystem>();
        effectSystem = GetComponent<EffectSystem>();

        // Validate
        if (buffSystem == null) buffSystem = gameObject.AddComponent<BuffSystem>();
        if (manaSystem == null) manaSystem = gameObject.AddComponent<ManaSystem>();
        if (combatSystem == null) combatSystem = gameObject.AddComponent<CombatSystem>();
        if (effectSystem == null) effectSystem = gameObject.AddComponent<EffectSystem>();
    }

    protected virtual void Start()
    {
        currentHP = maxHP;
        Debug.Log($"🎮 {gameObject.name} spawn! HP: {currentHP}/{maxHP}");
    }

    protected virtual void Update()
    {
        // Check death
        if (currentHP <= 0 && !isDead)
        {
            Die();
        }
    }

    #region DAMAGE & HEAL

    /// <summary>
    /// Nhận sát thương (được gọi từ CombatSystem)
    /// </summary>
    public virtual void TakeDamage(float damage)
    {
        currentHP = Mathf.Max(currentHP - damage, 0f);
        if (healthBar != null) healthBar.value = currentHP / maxHP;

        Debug.Log($"❤️ {gameObject.name} nhận {damage} damage → {currentHP}/{maxHP}");
    }

    /// <summary>
    /// Hồi máu
    /// </summary>
    public virtual void Heal(float amount)
    {
        currentHP = Mathf.Min(currentHP + amount, maxHP);
        if (healthBar != null) healthBar.value = currentHP / maxHP;

        Debug.Log($"💚 {gameObject.name} hồi {amount} HP → {currentHP}/{maxHP}");
    }

    /// <summary>
    /// Full heal
    /// </summary>
    public void FullHeal()
    {
        currentHP = maxHP;
        if (healthBar != null) healthBar.value = 1f;
        Debug.Log($"✨ {gameObject.name} HP full → {currentHP}/{maxHP}");
    }

    #endregion

    #region DEATH

    /// <summary>
    /// Die
    /// </summary>
    public virtual void Die()
    {
        isDead = true;
        Debug.Log($"💀 {gameObject.name} đã chết!");
        
        // Disable GameObject hoặc animate death
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Check xem đã chết chưa
    /// </summary>
    public bool IsDead() => isDead;

    /// <summary>
    /// Revive
    /// </summary>
    public void Revive()
    {
        isDead = false;
        FullHeal();
        manaSystem.FullRestoreMana();
        effectSystem.ClearAllEffects();
        gameObject.SetActive(true);
        Debug.Log($"🔄 {gameObject.name} hồi sinh!");
    }

    #endregion

    #region TARGET

    /// <summary>
    /// Set target để tấn công
    /// </summary>
    public void SetTarget(Character target)
    {
        currentTarget = target;
        Debug.Log($"🎯 {gameObject.name} target: {(target != null ? target.gameObject.name : "None")}");
    }

    /// <summary>
    /// Get current target
    /// </summary>
    public Character GetTarget() => currentTarget;

    #endregion

    #region GETTERS

    public float GetMaxHP() => maxHP;
    public float GetCurrentHP() => currentHP;
    public float GetHPPercent() => currentHP / maxHP;
    public BuffSystem GetBuffSystem() => buffSystem;
    public ManaSystem GetManaSystem() => manaSystem;
    public CombatSystem GetCombatSystem() => combatSystem;
    public EffectSystem GetEffectSystem() => effectSystem;

    #endregion
}
