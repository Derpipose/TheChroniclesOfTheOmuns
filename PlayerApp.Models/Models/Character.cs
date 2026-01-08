using System;
using System.ComponentModel.DataAnnotations;
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
    }

    public void RemoveCharacterClass() {
        CharacterClass = null;
    }

    public void AssignCharacterRace(CharacterRace characterRace) {
        CharacterRace = characterRace;
    }

    public void RemoveCharacterRace() {
        CharacterRace = null;
    }

    public DiceType? GetHitDice() {
        return CharacterClass?.HitDice;
    }

    public DiceType? GetManaDice() {
        return CharacterClass?.ManaDice;
    }
}

