using System;
using System.ComponentModel.DataAnnotations;
using PlayerApp.Models.Dice;

namespace PlayerApp.Models;

public class FantasyCharacter {
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = "Tav";
    [Required]
    public int Level { get; set; } = 1;
    public CharacterStats Stats { get; set; }
    public int Health { get; set; } = 0;
    public int Mana { get; set; } = 0;

    public FantasyCharacterRace? FantasyCharacterRace { get; private set; }
    public FantasyCharacterClass? FantasyCharacterClass { get; private set; }

    public List<CharacterStatBonus> CharacterStatBonuses { get; set; } = new List<CharacterStatBonus>();

    public FantasyCharacter() {
        Stats = new CharacterStats();
    }

    public void AssignFantasyCharacterClass(FantasyCharacterClass FantasyCharacterClass) {
        this.FantasyCharacterClass = FantasyCharacterClass;
    }

    public void RemoveFantasyCharacterClass() {
        FantasyCharacterClass = null;
    }

    public void AssignFantasyCharacterRace(FantasyCharacterRace FantasyCharacterRace) {
        this.FantasyCharacterRace = FantasyCharacterRace;
    }

    public void RemoveFantasyCharacterRace() {
        FantasyCharacterRace = null;
    }

    public DiceType? GetHitDice() {
        return FantasyCharacterClass?.HitDice;
    }

    public DiceType? GetManaDice() {
        return FantasyCharacterClass?.ManaDice;
    }
}

