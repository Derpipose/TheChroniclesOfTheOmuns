using System;
using System.Diagnostics;
using PlayerApp.Models.Enums;

namespace PlayerApp.Models;

public class Character {
    public string Name { get; private set; } = "Tav";
    public int PlayerId { get; private set; } = 0;
    public int Level { get; private set; } = 1;
    public SettingEnum Setting { get; private set; } = SettingEnum.Unregistered;
    public int MaxHealth { get; private set; } = 0;
    public int BaseArmorClass { get; private set; } = 10;

    public int CurrentHealth { get; private set; } = 0;
    public int Id { get; private set; }
    public bool IsActive { get; private set; } = false;

    public DateTime LastModified { get; private set; } = DateTime.UtcNow;

    public ICollection<CharacterModifier> Modifiers { get; set; } = [];

    public Player? Player { get; private set; }


    public Character(int playerid, string name) {
        PlayerId = playerid;
        Name = name;
        LastModified = DateTime.UtcNow;
    }

    private Character(int playerId) {
        this.PlayerId = playerId;
    }

    // Parameterless constructor for EF Core
    protected Character() {
        // EF Core needs this to materialize the entity
    }
    public void SetName(string name) {
        Name = name;
        LastModified = DateTime.UtcNow;
    }
    public void SetCurrentHealth(int currentHealth) {
        CurrentHealth = currentHealth;
        LastModified = DateTime.UtcNow;
    }

    public void SetMaxHealth(int maxHealth) {
        MaxHealth = maxHealth;
        if (CurrentHealth > MaxHealth) {
            CurrentHealth = MaxHealth;
        }
        LastModified = DateTime.UtcNow;
    }

    public void SetBaseArmorClass(int baseArmorClass) {
        BaseArmorClass = baseArmorClass;
        LastModified = DateTime.UtcNow;
    }


    public void SetActive() {
        IsActive = true;
        LastModified = DateTime.UtcNow;
    }

    public void SetLevel(int value) {
        if (value < 1) throw new ArgumentOutOfRangeException(nameof(value), "Level must be greater than 0.");
        Level = value;
        LastModified = DateTime.UtcNow;
    }

    public void SetSetting(SettingEnum setting) {
        Setting = setting;
        LastModified = DateTime.UtcNow;
    }

    public int GetMaxHealth() {
        var bonus = Modifiers
            .Where(m => m.ModifierType == ModifierType.Health)
            .Sum(m => m.Amount);
        return MaxHealth + bonus;
    }

    public int GetArmorClass() {
        var bonus = Modifiers
            .Where(m => m.ModifierType == ModifierType.ArmorClass)
            .Sum(m => m.Amount);
        return BaseArmorClass + bonus;
    }

    public int GetStat(ModifierType statType, int baseValue) {
        var bonus = Modifiers
            .Where(m => m.ModifierType == statType)
            .Sum(m => m.Amount);
        return baseValue + bonus;
    }



    [Conditional("DEBUG")]
    public void SimulatePersist(int id) {
        Id = id;
    }
}
