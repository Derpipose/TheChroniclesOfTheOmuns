using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace PlayerApp.Models;

public class Character {
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = "Tav";
    [Required]
    public int Level { get; set; } = 1;
    public CharacterStats? Stats { get; set; }
    public int Health { get; set; } = 0;

    public CharacterRace? CharacterRace { get; set; }
    public CharacterClass? CharacterClass { get; private set; }

    public Character() {
        Stats = new CharacterStats();
    }

    public void AssignCharacterClass(CharacterClass characterClass) {
        CharacterClass = characterClass;
        CalculateHitPoints();
    }

    public DiceType? GetHitDice() {
        return CharacterClass?.HitDice;
    }

    public DiceType? GetManaDice() {
        return CharacterClass?.ManaDice;
    }

    public void CalculateHitPoints() {
        if( Stats == null || CharacterClass == null || CharacterClass.HitDice == null )
            return;

        if (CharacterClass.ClassType == "Combat") {
            Health = 2 * Stats.Constitution + CharacterClass.HitDice.Sides + GetBonus("Constitution");
        } else {
            Health = (2 * CharacterClass.HitDice.Sides) + Stats.Constitution + GetBonus("Constitution");
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
