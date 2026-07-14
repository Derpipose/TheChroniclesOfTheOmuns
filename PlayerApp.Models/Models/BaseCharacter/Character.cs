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
    public int Id { get; private set; }
    public bool IsActive { get; private set; } = false;

    public ICollection<CharacterModifier> Modifiers { get; set; } = [];

    public Player? Player { get; private set; }


    public Character(int playerid, string name) {
        PlayerId = playerid;
        Name = name;
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
    }

    public void SetMaxHealth(int maxHealth) {
        MaxHealth = maxHealth;
    }


    public void SetActive() {
        IsActive = true;
    }

    public void SetLevel(int value) {
        if (value < 1) throw new ArgumentOutOfRangeException(nameof(value), "Level must be greater than 0.");
        Level = value;
    }

    public void SetSetting(SettingEnum setting) {
        Setting = setting;
    }

    public int GetMaxHealth() {
        var bonus = Modifiers
            .Where(m => m.ModifierType == RaceModifierType.Health)
            .Sum(m => m.Amount);
        return MaxHealth + bonus;
    }

    public int GetArmorClass(int baseArmorClass) {
        var bonus = Modifiers
            .Where(m => m.ModifierType == RaceModifierType.ArmorClass)
            .Sum(m => m.Amount);
        return baseArmorClass + bonus;
    }

    public int GetStat(RaceModifierType statType, int baseValue) {
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
