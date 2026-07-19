# 🔥 NARUTO - TRẬN CHIẾN TỐI THƯỢNG

**RPG PvE/PvP Auto-Battler Game** - Unity 6 (6000.5.4f1)

---

## 📋 GAME CONCEPT

- **Trận đấu 6 phút** PvE/PvP
- **Auto-battler**: Nhân vật tự đánh, bạn ấn phím dùng skill
- **Mana System**: Hồi mana từ đòn thường, skill, buff
- **4 Skill + 1 Passive**: Mỗi skill có cost, cooldown, buff
- **Build nhân vật**: Chọn skill trước vào trận
- **Win condition**: 
  - Sau 6 phút: Team nào còn nhiều nhân vật hơn thắng
  - Nếu bằng: Team nào có HP cao hơn thắng

---

## 🏗️ STRUCTURE

```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── ManaSystem.cs          (Quản lý Mana)
│   │   ├── SkillSystem.cs         (Quản lý Skill)
│   │   ├── BuffSystem.cs          (Quản lý Buff/Debuff)
│   │   ├── CombatSystem.cs        (Tính sát thương)
│   │   └── SkillData.cs           (Template skill)
│   ├── CharacterSystem/
│   │   ├── Character.cs           (Nhân vật chính)
│   │   └── Enemy.cs               (Nhân vật địch)
│   ├── Data/
│   │   └── BuffData.cs            (Template buff)
│   └── UI/
│       ├── ManaBar.cs             (UI Mana)
│       └── SkillUI.cs             (UI Skill)
└── Resources/
    ├── Skills/                     (Skill assets)
    └── Buffs/                      (Buff assets)
```

---

## 🎮 GAMEPLAY

### **Mana System:**
- Pool: Tùy skill (1000 là chuẩn)
- Hồi: 20 mana/đòn thường + buff + trang bị
- Có % tăng hiệu quả hồi mana

### **Skill Trigger:**
- **SK1**: Auto khi đủ mana
- **SK2-SK3**: Hồi cooldown (dùng rồi phải chờ)
- **SK4**: Passive nội tại (auto trigger khi điều kiện đạt)

### **Ấn Skill:**
- Bạn ấn **1/2/3/4** để dùng skill (nếu đủ điều kiện)

---

## 📝 PHASE 1: COMPLETED ✅

- [x] ManaSystem.cs - Quản lý Mana pool & regen
- [x] BuffSystem.cs - Quản lý Buff/Debuff
- [x] SkillData.cs - Template skill
- [x] SkillSystem.cs - Quản lý 4 skill
- [x] CombatSystem.cs - Combat logic

---

## 🚀 PHASE 2: NEXT

- [ ] Sửa Character.cs để tích hợp hệ thống mới
- [ ] Sửa Enemy.cs để tích hợp hệ thống mới
- [ ] Tạo Hashirama class (inherit từ Character)
- [ ] Implement 4 Hashirama skills
- [ ] Test gameplay

---

## 👨‍💻 DEVELOPER

- **Backend/System**: @copilot
- **Skill Design**: @phathuynhgia639-collab
