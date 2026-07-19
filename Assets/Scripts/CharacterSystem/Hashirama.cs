using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Hashirama - Rank SX Character
/// Hệ thống phức tạp với 4 skill + Passive + Tiên Pháp
/// 
/// SIMPLIFIED VERSION CHO BẠNHIỂU RÕ:
/// SK1: Mộc Đốn (2 dạng: Thường & Tiên Pháp)
/// SK2: Vạn Thụ Giới (2 dạng + Thế Thân Gỗ)
/// SK3: Ngũ Trọng Minh Thần Môn (1 use only)
/// SK4: Nhẫn Giả Thánh Nhân (Passive + Tiên Pháp auto trigger)
/// </summary>
public class Hashirama : Character
{
    [Header("Hashirama Special")]
    [SerializeField] private bool isImmortaMode = false; // Tiên Pháp state
    [SerializeField] private float immortalModeHPThreshold = 0.45f; // Kích hoạt khi HP < 45%
    [SerializeField] private float hpEvolutionStack = 0f; // Cộng dồn +HP từ SK4
    [SerializeField] private float damageReductionStack = 0f; // Cộng dồn +Miễn Thương
    [SerializeField] private int sk3UseCount = 0; // SK3 chỉ dùng 1 lần

    // Triệu hồi (Summon)
    private List<GameObject> summonedAllies = new List<GameObject>();

    protected override void Start()
    {
        base.Start();
        Debug.Log("🌳 Hashirama Tiên Nhân khởi tạo!");
    }

    protected override void Update()
    {
        base.Update();

        // SK4: Passive - Check Tiên Pháp trigger
        CheckImmortaModeActivation();
        
        // SK4: Passive - Auto HP regen in Tiên Pháp mode
        if (isImmortaMode)
        {
            RegenerateHPInImmortaMode();
        }
    }

    #region SK4: NHẪN GIẢ THÁNH NHÂN (Passive)

    /// <summary>
    /// SK4 Passive: Giới hạn sát thương tối đa 15% HP gốc
    /// </summary>
    public override void TakeDamage(float damage)
    {
        // Giới hạn sát thương tối đa
        float maxDamagePerHit = maxHP * 0.15f;
        float cappedDamage = Mathf.Min(damage, maxDamagePerHit);

        base.TakeDamage(cappedDamage);
    }

    /// <summary>
    /// SK4: Tiến hóa HP - Mỗi lần dùng skill, +8% HP gốc & +2% Miễn Thương
    /// </summary>
    private void EvolveHPOnSkillCast()
    {
        float hpGain = maxHP * 0.08f;
        float damageReductionGain = 0.02f;

        maxHP += hpGain;
        hpEvolutionStack += hpGain;
        
        damageReductionStack = Mathf.Min(damageReductionStack + damageReductionGain, 0.75f);

        Debug.Log($"🌳 Hashirama evolved! HP: +{hpGain}, Miễn Thương: +{damageReductionGain * 100}%");
    }

    /// <summary>
    /// SK4: Check Tiên Pháp activation (HP < 45%)
    /// </summary>
    private void CheckImmortaModeActivation()
    {
        float hpPercent = currentHP / maxHP;

        if (hpPercent < immortalModeHPThreshold && !isImmortaMode)
        {
            ActivateImmortaMode();
        }
    }

    /// <summary>
    /// SK4: Activate Tiên Pháp mode
    /// </summary>
    private void ActivateImmortaMode()
    {
        isImmortaMode = true;
        Debug.Log("🔥 HASHIRAMA TIÊN PHÁP KÍCH HOẠT!");

        // Bất Hoại 5 giây
        buffSystem.AddBuffByName("Bất Hoại", 5f);
        
        // Hồi 20% HP gốc
        float healAmount = maxHP * 0.2f;
        currentHP = Mathf.Min(currentHP + healAmount, maxHP);
        if (healthBar != null) healthBar.value = currentHP;

        // Update all skills to Tiên Pháp mode
        UpdateSkillsToImmortaMode();
    }

    /// <summary>
    /// SK4: Auto HP regen in Tiên Pháp (6% HP Max / giây)
    /// </summary>
    private void RegenerateHPInImmortaMode()
    {
        float regenPerSecond = maxHP * 0.06f;
        float regenThisFrame = regenPerSecond * Time.deltaTime;
        
        currentHP = Mathf.Min(currentHP + regenThisFrame, maxHP);
        if (healthBar != null) healthBar.value = currentHP;
    }

    /// <summary>
    /// SK4: Update skills to Tiên Pháp versions
    /// </summary>
    private void UpdateSkillsToImmortaMode()
    {
        // Khi ở Tiên Pháp, SK1-SK2-SK3 chuyển sang dạng Tiên Pháp
        // (Implement trong từng skill caster)
    }

    #endregion

    #region SK1: MỘC ĐỘN

    /// <summary>
    /// SK1 - Mộc Long Thuật (Thường): 500 Mana, 10% HP damage
    /// 80% tỉ lệ Trói chân 5 giây
    /// </summary>
    public bool CastMocLongThuat()
    {
        if (!manaSystem.ConsumeMana(500f))
        {
            Debug.Log("Không đủ mana cho Mộc Long Thuật!");
            return false;
        }

        // Bất Hoại trong skill
        buffSystem.AddBuffByName("Bất Hoại", 1f);

        if (currentTarget != null && !currentTarget.IsDead())
        {
            // Sát thương 10% HP gốc
            float damage = maxHP * 0.1f;
            currentTarget.TakeDamage(damage);

            // Đẩy lùi mục tiêu
            PushBackTarget(currentTarget, 2f);

            // 80% tỉ lệ Trói chân
            if (Random.value < 0.8f)
            {
                // Trói chân: khóa attack, giảm 40% sát thương
                BuffSystem targetBuff = currentTarget.GetComponent<BuffSystem>();
                targetBuff.AddBuffByName("Trói Chân", 5f); // Custom buff
                Debug.Log($"🌳 {currentTarget.name} bị trói chân!");
            }

            EvolveHPOnSkillCast(); // SK4: +HP
        }

        Debug.Log("🌳 Mộc Long Thuật!");
        return true;
    }

    /// <summary>
    /// SK1 - Mộc Nhân Thuật (Thường): 700 Mana, Triệu hồi Mộc Nhân
    /// Mộc Nhân có 150% HP gốc, +30% Miễn Thương cho Hashirama khi còn sống
    /// </summary>
    public bool CastMocNhanThuat()
    {
        if (!manaSystem.ConsumeMana(700f))
        {
            Debug.Log("Không đủ mana cho Mộc Nhân Thuật!");
            return false;
        }

        // Bất Hoại trong skill
        buffSystem.AddBuffByName("Bất Hoại", 1f);

        // Triệu hồi Mộc Nhân
        GameObject mocNhan = SpawnMocNhan();
        summonedAllies.Add(mocNhan);

        EvolveHPOnSkillCast(); // SK4: +HP
        Debug.Log("🌳 Mộc Nhân Thuật - Mộc Nhân xuất hiện!");
        return true;
    }

    /// <summary>
    /// SK1 - Tiên Pháp: Thiên Thủ Quan Âm (1000 Mana, 30 giây)
    /// Phật Tổ nghìn tay thay thế, gây sát thương 1% HP Max mỗi cú, 25% HP Max ở 5 giây cuối
    /// </summary>
    public bool CastThienThuQuanAm()
    {
        if (!isImmortaMode)
        {
            Debug.Log("Chỉ dùng được ở trạng thái Tiên Pháp!");
            return false;
        }

        if (!manaSystem.ConsumeMana(1000f))
        {
            Debug.Log("Không đủ mana cho Thiên Thủ Quan Âm!");
            return false;
        }

        buffSystem.AddBuffByName("Bất Hoại", 30f); // Miễn tử 30 giây
        
        // Khóa SK2, SK3, đánh thường
        // TODO: Implement trong SkillSystem

        // 25 giây: 20 cú đấm ngẫu nhiên (1% HP Max mỗi cú)
        // 5 giây cuối: 25% HP Max tấn công toàn khu vực
        StartCoroutine(ThienThuQuanAmRoutine());

        EvolveHPOnSkillCast(); // SK4: +HP
        Debug.Log("🌳 TIÊN PHÁP: THIÊN THỦ QUAN ÂM!");
        return true;
    }

    private System.Collections.IEnumerator ThienThuQuanAmRoutine()
    {
        float attackDuration = 25f;
        float finalAttackDuration = 5f;

        // 25 giây: 20 cú đấm ngẫu nhiên
        for (int i = 0; i < 20; i++)
        {
            if (currentTarget != null && !currentTarget.IsDead())
            {
                float damage = maxHP * 0.01f;
                currentTarget.TakeDamage(damage);
            }
            yield return new WaitForSeconds(attackDuration / 20f);
        }

        // 5 giây cuối: Tấn công toàn khu vực
        if (currentTarget != null && !currentTarget.IsDead())
        {
            float finalDamage = maxHP * 0.25f;
            currentTarget.TakeDamage(finalDamage);
        }
    }

    #endregion

    #region SK2: VẠN THỰ GIỚI

    /// <summary>
    /// SK2 - Vạn Thụ Giới (Thường): Rừng cây bao bọc, 2% HP damage/giây, 6 giây
    /// Giảm 50% hồi phục Mana/HP của địch
    /// </summary>
    public bool CastVanThuGioi()
    {
        if (!manaSystem.ConsumeMana(1000f))
        {
            Debug.Log("Không đủ mana cho Vạn Thụ Giới!");
            return false;
        }

        buffSystem.AddBuffByName("Bất Hoại", 1f);

        if (currentTarget != null && !currentTarget.IsDead())
        {
            // Debuff địch: 2% HP damage/giây trong 6 giây
            StartCoroutine(VanThuGioiDamageRoutine(currentTarget, 6f));
        }

        EvolveHPOnSkillCast(); // SK4: +HP
        Debug.Log("🌳 Vạn Thụ Giới!");
        return true;
    }

    private System.Collections.IEnumerator VanThuGioiDamageRoutine(Character target, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration && target != null && !target.IsDead())
        {
            float damage = maxHP * 0.02f;
            target.TakeDamage(damage);
            
            elapsed += 1f;
            yield return new WaitForSeconds(1f);
        }
    }

    #endregion

    #region SK3: NGŨ TRỌNG MINH THẦN MÔN

    /// <summary>
    /// SK3 - Chỉ dùng 1 lần ở đầu trận
    /// Triệu hồi 5 cổng gỗ vĩnh viễn, +12% Miễn Thương/cổng, chặn 80% sát thương diện rộng
    /// </summary>
    public bool CastNguTrongMinhThanMon()
    {
        if (sk3UseCount > 0)
        {
            Debug.Log("SK3 chỉ dùng được 1 lần!");
            return false;
        }

        if (!manaSystem.ConsumeMana(1000f))
        {
            Debug.Log("Không đủ mana cho Ngũ Trọng Minh Thần Môn!");
            return false;
        }

        buffSystem.AddBuffByName("Bất Hoại", 1f);

        // Triệu hồi 5 cổng
        for (int i = 0; i < 5; i++)
        {
            GameObject gate = SpawnGate(i);
            summonedAllies.Add(gate);
        }

        // +12% Miễn Thương cho Hashirama
        damageReductionStack = Mathf.Min(damageReductionStack + 0.12f * 5, 0.75f);

        sk3UseCount++;
        EvolveHPOnSkillCast(); // SK4: +HP
        Debug.Log("🌳 Ngũ Trọng Minh Thần Môn - 5 cổng triệu hồi!");
        return true;
    }

    #endregion

    #region HELPER FUNCTIONS

    /// <summary>
    /// Đẩy lùi mục tiêu
    /// </summary>
    private void PushBackTarget(Character target, float distance)
    {
        Vector3 pushDirection = (target.transform.position - transform.position).normalized;
        target.transform.position += pushDirection * distance;
    }

    /// <summary>
    /// Triệu hồi Mộc Nhân
    /// </summary>
    private GameObject SpawnMocNhan()
    {
        GameObject mocNhan = new GameObject("Mộc Nhân");
        mocNhan.transform.position = transform.position + Vector3.right * 2f;

        // Add capsule (visual)
        CapsuleCollider capsule = mocNhan.AddComponent<CapsuleCollider>();
        
        // Add character component
        Character mocNhanChar = mocNhan.AddComponent<Character>();
        mocNhanChar.GetType().GetField("maxHP", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(mocNhanChar, maxHP * 1.5f);

        return mocNhan;
    }

    /// <summary>
    /// Triệu hồi Cổng Gỗ
    /// </summary>
    private GameObject SpawnGate(int index)
    {
        GameObject gate = new GameObject($"Cổng Gỗ {index}");
        gate.transform.position = transform.position + (Vector3.right * (index - 2) * 2f);

        // Add visual (cube)
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visual.transform.SetParent(gate.transform);
        visual.transform.localScale = new Vector3(0.5f, 2f, 0.5f);

        return gate;
    }

    #endregion

    #region GETTERS

    public bool IsInImmortaMode() => isImmortaMode;
    public float GetHPEvolutionStack() => hpEvolutionStack;
    public float GetDamageReductionStack() => damageReductionStack;

    #endregion
}
