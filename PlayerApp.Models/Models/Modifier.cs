using System.ComponentModel.DataAnnotations;
using PlayerApp.Models.Enums;

namespace PlayerApp.Models;

public class Modifier {
    // Modifiers are used to define specific changes applied to characters, 
    // such as bonus health, mana or AC provided by a race
    [Key]
    public int Id { get; set; }

    [Required]
    public ModifierType Type { get; set; }

    [Required]
    public int Value { get; set; }

    public string? Description { get; set; }

    public ICollection<RacialModifier> RacialModifiers { get; set; } = new List<RacialModifier>();
}
