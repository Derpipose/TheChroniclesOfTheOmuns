using System;
using System.Diagnostics;
using PlayerApp.Models.Enums;

namespace PlayerApp.Models;

public class Character {
    public string Name { get; private set; } = "Tav";
    public int PlayerId { get; private set; } = 0;
    public int Level { get; private set; } = 1;
    public SettingEnum Setting { get; private set; } = SettingEnum.Unregistered;
    public int BaseHealth { get; private set; } = 0;
    public int BaseArmorClass { get; private set; } = 10;

    public int CurrentHealth { get; private set; } = 0;
    public int Id { get; private set; }
    public bool IsActive { get; private set; } = false;

    public DateTime LastModified { get; private set; } = DateTime.UtcNow;

    public Player? Player { get; private set; }

    public string Notes { get; private set; } = "";


    public Character(int playerid, string name) {
        PlayerId = playerid;
        Name = name;
        UpdateLastModified();
    }

    private Character(int playerId) {
        this.PlayerId = playerId;
    }

    // Parameterless constructor for EF Core
    protected Character() {
        // EF Core needs this to materialize the entity
    }

    public void UpdateLastModified() {
        LastModified = DateTime.UtcNow;
    }

    public void SetName(string name) {
        Name = name;
        UpdateLastModified();
    }
    public void SetCurrentHealth(int currentHealth) {
        CurrentHealth = currentHealth;
        UpdateLastModified();
    }

    public void SetBaseHealth(int baseHealth) {
        BaseHealth = baseHealth;
        UpdateLastModified();
    }


    public void SetActive() {
        IsActive = true;
        UpdateLastModified();
    }

    public void SetLevel(int value) {
        if (value < 1) throw new ArgumentOutOfRangeException(nameof(value), "Level must be greater than 0.");
        Level = value;
        UpdateLastModified();
    }

    public void SetSetting(SettingEnum setting) {
        Setting = setting;
        UpdateLastModified();
    }

    public int GetMaxHealth(IEnumerable<ICharacterModifier> modifiers) {
        var bonus = modifiers
            .Where(m => m.ModifierType == ModifierType.Health)
            .Sum(m => m.Amount);
        return BaseHealth + bonus;
    }

    public int GetDamageResist(IEnumerable<ICharacterModifier> modifiers) {
        var bonus = modifiers
            .Where(m => m.ModifierType == ModifierType.DamageResist)
            .Sum(m => m.Amount);
        return bonus;
    }

    public int GetStat(IEnumerable<ICharacterModifier> modifiers, ModifierType statType, int baseValue) {
        var bonus = modifiers
            .Where(m => m.ModifierType == statType)
            .Sum(m => m.Amount);
        return baseValue + bonus;
    }

    public int GetModifierBonus(IEnumerable<ICharacterModifier> modifiers, ModifierType statType) =>
        GetStat(modifiers, statType, 0);

    public void SetNotes(string notes) {
        Notes = notes;
        UpdateLastModified();
    }


    [Conditional("DEBUG")]
    public void SimulatePersist(int id) {
        Id = id;
    }
}
