using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerApp.Models.Enums;

namespace PlayerApp.Models;

public class CharacterStats {
    [Key]
    public int Id { get; set; }
    [Required]
    public int CharacterId { get; private set; }
    [Required]
    public int Strength { get; private set; } = 10;
    [Required]
    public int Constitution { get; private set; } = 10;
    [Required]
    public int Dexterity { get; private set; } = 10;
    [Required]
    public int Wisdom { get; private set; } = 10;
    [Required]
    public int Charisma { get; private set; } = 10;
    [Required]
    public int Intelligence { get; private set; } = 10;


    public CharacterStats(int characterid) {
        CharacterId = characterid;
    }

    // Pre-play only
    public void SetStat(StatType stat, int value, Character character) {
        if (character.IsActive)
            throw new InvalidOperationException(
                "Cannot edit stats of an active character.");
        if (value <= 0)
            throw new ArgumentException("Stat value must be positive.");

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

    // In-play increases (level ups)
    public void IncreaseStat(StatType stat, int amount, Character character) {
        if (amount <= 0)
            throw new ArgumentException("Increase amount must be positive.");
        if (!character.IsActive)
            throw new InvalidOperationException("Cannot edit stats of an inactive character.");
        ApplyStatDelta(stat, amount);
    }

    // In-play decreases (perma debuffs)
    public void DecreaseStat(StatType stat, int amount, Character character) {
        if (amount <= 0)
            throw new ArgumentException("Decrease amount must be positive.");
        if (!character.IsActive)
            throw new InvalidOperationException("Cannot edit stats of an inactive character.");
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