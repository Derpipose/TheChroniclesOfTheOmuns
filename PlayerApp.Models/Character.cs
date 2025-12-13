using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace PlayerApp.Models;

public class Character {

    [Required]
    public string Name { get; set; } = "Tav";
    [Required]
    public int Level { get; set; } = 1;
    public CharacterStats? Stats { get; set; } = new CharacterStats();

    public CharacterRace? CharacterRace { get; set; }

}
