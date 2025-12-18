using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayerApp.Models;

public class RacialModifier {
    [Key]
    public int Id { get; set; }

    [Required]
    public int RaceId { get; set; }

    [Required]
    public int ModifierId { get; set; }

    // Navigation properties
    [ForeignKey(nameof(RaceId))]
    public CharacterRace Race { get; set; }

    [ForeignKey(nameof(ModifierId))]
    public Modifier Modifier { get; set; }
}
