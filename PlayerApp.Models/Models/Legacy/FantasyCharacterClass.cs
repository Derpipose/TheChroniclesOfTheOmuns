using System;
using System.ComponentModel.DataAnnotations;
using PlayerApp.Models.Enums;
using PlayerApp.Models.Dice;

namespace PlayerApp.Models.Legacy;

public class FantasyCharacterClass {
    [Key]
    public int Id { get; set; }
    [Required]
    public required string Name { get; set; }
    [Required]
    public required FantasyClassTypeEnum FantasyClassType { get; set; }
    public bool IsVeteranLocked { get; set; } = false;
    [Required]
    public required string Description { get; set; }
    public DiceType HitDice { get; set; }
    public DiceType ManaDice { get; set; }
}
