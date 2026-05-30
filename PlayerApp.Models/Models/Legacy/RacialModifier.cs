using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayerApp.Models.Legacy;

public class RacialModifier {
    // Racial modifier is for things like bonus mana or bonus health
    [Key]
    public int Id { get; set; }

    [Required]
    public int RaceId { get; set; }

    [Required]
    public int ModifierId { get; set; }

    // Navigation properties
    [ForeignKey(nameof(RaceId))]
    public required FantasyCharacterRace Race { get; set; }

    [ForeignKey(nameof(ModifierId))]
    public required Modifier Modifier { get; set; }
}
