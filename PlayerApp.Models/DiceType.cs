using System.ComponentModel.DataAnnotations;

namespace PlayerApp.Models;

public class DiceType {
    [Key]
    public int Id { get; set; }
    [Required]
    public required string Name { get; set; } // "D4", "D6", "D8", etc.
    [Required]
    public int Sides { get; set; } // 4, 6, 8, 10, 12, 20

    public static Dictionary<int, DiceType> GetStandardDice() {
        return new Dictionary<int, DiceType> {
            { 1, new DiceType { Id = 1, Name = "D4", Sides = 4 } },
            { 2, new DiceType { Id = 2, Name = "D6", Sides = 6 } },
            { 3, new DiceType { Id = 3, Name = "D8", Sides = 8 } },
            { 4, new DiceType { Id = 4, Name = "D10", Sides = 10 } },
            { 5, new DiceType { Id = 5, Name = "D12", Sides = 12 } },
            { 6, new DiceType { Id = 6, Name = "D20", Sides = 20 } }
        };
    }
}
