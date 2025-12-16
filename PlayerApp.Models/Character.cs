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

    public CharacterRace? CharacterRace { get; set; }
    public CharacterClass? CharacterClass { get; set; }

    public Character() {
        Stats = new CharacterStats();
    }

    public DiceType? GetHitDice() {
        return CharacterClass?.HitDice;
    }

    public DiceType? GetManaDice(){
        return CharacterClass?.ManaDice;
    }
}
