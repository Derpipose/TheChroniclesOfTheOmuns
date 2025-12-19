using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlayerApp.Models;

public class CharacterRace {
    [Key]
    public int Id { get; set; }

    [Required]
    public required string Name { get; set; }

    [Required]
    public required string Description { get; set; }

    // Navigation property
    public ICollection<RacialModifier> Modifiers { get; set; } = new List<RacialModifier>();
}
