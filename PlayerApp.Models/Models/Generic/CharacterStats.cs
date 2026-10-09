using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerApp.Models.Enums;

namespace PlayerApp.Models;

public class CharacterStats {
    public const int MinStat = 10;

    [Key]
    public int Id { get; set; }
    [Required]
    public int CharacterId { get; private set; }
    [Required]
    public int Strength { get; private set; } = MinStat;
    [Required]
    public int Constitution { get; private set; } = MinStat;
    [Required]
    public int Dexterity { get; private set; } = MinStat;
    [Required]
    public int Wisdom { get; private set; } = MinStat;
    [Required]
    public int Charisma { get; private set; } = MinStat;
    [Required]
    public int Intelligence { get; private set; } = MinStat;


    // Parameterless constructor for EF Core
    protected CharacterStats() {
        // EF Core needs this to materialize the entity
    }

    public CharacterStats(int characterid) {
        CharacterId = characterid;
    }

    // Pre-play only
    public void SetStat(StatType stat, int value, Character character, IEnumerable<ICharacterModifier>? modifiers = null) {
        if (character.IsActive)
            throw new InvalidOperationException(
                "Cannot edit stats of an active character.");

        if (value < MinStat)
            throw new ArgumentException($"Stat value must be at least {MinStat}.");
        EnsureMeetsMinimum(stat, value, modifiers);

        switch (stat) {
            case StatType.Strength:     Strength = value;     break;
            case StatType.Dexterity:    Dexterity = value;    break;
            case StatType.Constitution: Constitution = value;  break;
            case StatType.Wisdom:       Wisdom = value;       break;
            case StatType.Intelligence: Intelligence = value;  break;
            case StatType.Charisma:     Charisma = value;     break;
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(stat), "Unknown stat type.");
        }
    }

    private int GetStatValue(StatType stat) => stat switch {
        StatType.Strength => Strength,
        StatType.Dexterity => Dexterity,
        StatType.Constitution => Constitution,
        StatType.Wisdom => Wisdom,
        StatType.Intelligence => Intelligence,
        StatType.Charisma => Charisma,
        _ => throw new ArgumentOutOfRangeException(
            nameof(stat), "Unknown stat type.")
    };

    private static int GetModifierBonus(StatType stat, IEnumerable<ICharacterModifier>? modifiers) {
        var modifierType = ToModifierType(stat);
        return modifiers?.Where(m => m.ModifierType == modifierType).Sum(m => m.Amount) ?? 0;
    }

    // Stored value plus modifier bonuses. Every path that sets a stat or modifier
    // validates against MinStat, so this is never below it.
    public int GetStat(StatType stat, IEnumerable<ICharacterModifier> modifiers) =>
        GetStatValue(stat) + GetModifierBonus(stat, modifiers);

    private static void EnsureMeetsMinimum(StatType stat, int baseValue, IEnumerable<ICharacterModifier>? modifiers) {
        if (baseValue + GetModifierBonus(stat, modifiers) < MinStat)
            throw new InvalidOperationException($"{stat} can't be less than {MinStat} after modifiers.");
    }

    // Call after assigning modifiers to make sure no stat ends up below MinStat
    public void EnsureAllMeetMinimum(IEnumerable<ICharacterModifier> modifiers) {
        foreach (StatType stat in Enum.GetValues<StatType>())
            EnsureMeetsMinimum(stat, GetStatValue(stat), modifiers);
    }

    public int GetStatModifier(StatType stat, IEnumerable<ICharacterModifier> modifiers) =>
        (GetStat(stat, modifiers) - MinStat) / 2;

    public static ModifierType ToModifierType(StatType stat) => stat switch {
        StatType.Strength => ModifierType.Strength,
        StatType.Dexterity => ModifierType.Dexterity,
        StatType.Constitution => ModifierType.Constitution,
        StatType.Intelligence => ModifierType.Intelligence,
        StatType.Wisdom => ModifierType.Wisdom,
        StatType.Charisma => ModifierType.Charisma,
        _ => throw new ArgumentOutOfRangeException(nameof(stat), "Unknown stat type.")
    };

    // In-play increases (level ups)
    public void IncreaseStat(StatType stat, int amount, Character character) {
        if (amount <= 0)
            throw new ArgumentException("Increase amount must be positive.");
        if (!character.IsActive)
            throw new InvalidOperationException("Cannot edit stats of an inactive character.");
        ApplyStatDelta(stat, amount);
    }

    // In-play decreases (perma debuffs)
    public void DecreaseStat(StatType stat, int amount, Character character, IEnumerable<ICharacterModifier>? modifiers = null) {
        if (amount <= 0)
            throw new ArgumentException("Decrease amount must be positive.");
        if (!character.IsActive)
            throw new InvalidOperationException("Cannot edit stats of an inactive character.");

        if (GetStatValue(stat) - amount < MinStat)
            throw new InvalidOperationException($"Stat can't be less than {MinStat}.");
        EnsureMeetsMinimum(stat, GetStatValue(stat) - amount, modifiers);
        ApplyStatDelta(stat, -amount);
    }

    private void ApplyStatDelta(StatType stat, int delta) {
        switch (stat) {
            case StatType.Strength:     Strength += delta;     break;
            case StatType.Dexterity:    Dexterity += delta;    break;
            case StatType.Constitution: Constitution += delta;  break;
            case StatType.Wisdom:       Wisdom += delta;       break;
            case StatType.Intelligence: Intelligence += delta;  break;
            case StatType.Charisma:     Charisma += delta;     break;
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(stat), "Unknown stat type.");
        }
    }
}