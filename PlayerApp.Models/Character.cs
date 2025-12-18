using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using PlayerApp.Models.Enums;

namespace PlayerApp.Models;

public class Character {
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = "Tav";
    [Required]
    public int Level { get; set; } = 1;
    public CharacterStats Stats { get; set; }
    public int Health { get; set; } = 0;
    public int Mana { get; set; } = 0;

    public CharacterRace? CharacterRace { get; private set; }
    public CharacterClass? CharacterClass { get; private set; }

    public Character() {
        Stats = new CharacterStats();
    }

    public void AssignCharacterClass(CharacterClass characterClass) {
        CharacterClass = characterClass;
        CalculateHitPoints();
        CalculateManaPoints();
    }

    public void AssignCharacterRace(CharacterRace characterRace) {
        CharacterRace = characterRace;
        CalculateManaPoints();
        CalculateHitPoints();
    }

    public DiceType? GetHitDice() {
        return CharacterClass?.HitDice;
    }

    public DiceType? GetManaDice() {
        return CharacterClass?.ManaDice;
    }

    public int GetRaceModifierValue(ModifierType type) {
        if (CharacterRace?.Modifiers == null) return 0;
        var modifier = CharacterRace.Modifiers
            .FirstOrDefault(m => m.Modifier.Type == type);
        return modifier?.Modifier.Value ?? 0;
    }

    public void CalculateHitPoints() {
        if (Stats.Constitution == 0 || CharacterClass == null || CharacterClass.HitDice == null)
            return;

        {
            if (CharacterClass.ClassType == "Combat") {
                Health = 2 * Stats.Constitution + CharacterClass.HitDice.Sides + GetBonus("Constitution");
            } else {
                Health = (2 * CharacterClass.HitDice.Sides) + Stats.Constitution + GetBonus("Constitution");
            }
        }
    }

    public void CalculateManaPoints() {
        if (Stats.Intelligence == 0 || Stats.Wisdom == 0 || CharacterClass == null ||
            CharacterClass.ManaDice == null || CharacterRace == null)
            return;

        int addBonus = GetRaceModifierValue(ModifierType.ManaBonus);
        int multValue = GetRaceModifierValue(ModifierType.ManaMultiplier);
        bool isMultiplicative = multValue > 0;

        if (CharacterClass.ClassType == "Magic") {
            if (!isMultiplicative) {
                // Additive bonus
                if (Stats.Intelligence <= Stats.Wisdom) {
                    Mana = Stats.Intelligence + Stats.Wisdom + GetBonus("Wisdom") + addBonus + CharacterClass.ManaDice.Sides;
                } else {
                    Mana = Stats.Intelligence + Stats.Wisdom + GetBonus("Intelligence") + addBonus + CharacterClass.ManaDice.Sides;
                }
            } else {
                // Multiplicative bonus
                if (Stats.Intelligence <= Stats.Wisdom) {
                    Mana = (Stats.Wisdom * multValue) + Stats.Intelligence + CharacterClass.ManaDice.Sides + GetBonus("Wisdom");
                } else {
                    Mana = (Stats.Intelligence * multValue) + Stats.Wisdom + CharacterClass.ManaDice.Sides + GetBonus("Intelligence");
                }
            }
        } else {
            // Non-Magic class
            if (isMultiplicative) {
                // Multiplicative bonus
                if (Stats.Intelligence <= Stats.Wisdom) {
                    Mana = (Stats.Wisdom * multValue) + (2 * CharacterClass.ManaDice.Sides) + GetBonus("Wisdom");
                } else {
                    Mana = (Stats.Intelligence * multValue) + (2 * CharacterClass.ManaDice.Sides) + GetBonus("Intelligence");
                }
            } else {
                // Additive bonus
                if (Stats.Intelligence <= Stats.Wisdom) {
                    Mana = Stats.Wisdom + (2 * CharacterClass.ManaDice.Sides) + GetBonus("Wisdom") + addBonus;
                } else {
                    Mana = Stats.Intelligence + (2 * CharacterClass.ManaDice.Sides) + GetBonus("Intelligence") + addBonus;
                }
            }
        }
    }

    public int GetBonus(string statName) {
        if (Stats == null)
            return 0;

        return statName.ToLower() switch {
            "strength" => (Stats.Strength - 10) / 2,
            "constitution" => (Stats.Constitution - 10) / 2,
            "dexterity" => (Stats.Dexterity - 10) / 2,
            "wisdom" => (Stats.Wisdom - 10) / 2,
            "charisma" => (Stats.Charisma - 10) / 2,
            "intelligence" => (Stats.Intelligence - 10) / 2,
            _ => 0
        };
    }
}

