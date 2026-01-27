using System;
using System.ComponentModel.DataAnnotations;
using PlayerApp.Models.Enums;

namespace PlayerApp.Models;

public class CharacterClass {
    [Key]
    public int Id { get; set; }
    [Required]
    public required string Name { get; set; }
    [Required]
    public required ClassTypeEnum ClassType { get; set; }
    public bool IsVeteranLocked { get; set; } = false;
    [Required]
    public required string Description { get; set; }
    [Required]
    public required DiceType HitDice { get; set; }
    [Required]
    public required DiceType ManaDice { get; set; }
}
