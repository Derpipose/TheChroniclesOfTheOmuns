using System.ComponentModel.DataAnnotations;
using PlayerApp.Models.Enums;

namespace PlayerApp.Models;

public class CharacterRace {
    [Key]
    public int Id { get; set; }

    [Required]
    public required string Name { get; set; }

    [Required]
    public required string Description { get; set; }

    [Required]
    public required string RaceType { get; set; }

    public ICollection<RacialModifier> Modifiers { get; set; } = new List<RacialModifier>();

    public ICollection<RaceStatBonus> RaceStatBonuses { get; set; } = new List<RaceStatBonus>();

    public RacialModifier? GetModifier(ModifierType type) {
        return Modifiers.FirstOrDefault(m => m.Modifier.Type == type);
    }

    public void AddModifier(ModifierType type, int value) {
        var modifier = new Modifier { Type = type, Value = value };
        Modifiers.Add(new RacialModifier { Modifier = modifier, Race = this });
    }
}
