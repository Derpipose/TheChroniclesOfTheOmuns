using System.ComponentModel.DataAnnotations;

namespace PlayerApp.Models;

public class Spell {
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string SpellBook { get; set; } = string.Empty;
    public int? BookLevel { get; set; }
    public string SpellBranch { get; set; } = string.Empty;
    [Required]
    public string Description { get; set; } = string.Empty;
    [Required]
    public int? ManaCostId { get; set; }
    [Required]
    public required ManaCost ManaCost { get; set; }
    public string? Range { get; set; }
    public int? HitDiceId { get; set; }
    public DiceType? HitDie { get; set; }
}
