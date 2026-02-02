using PlayerApp.Models.Enums;

namespace PlayerApp.Models;

public class DamageProfile {
    public int InitialDiceCount { get; set; }
    public DiceType DiceType { get; set; }
    public int? SecondaryDiceCount { get; set; }
    public DiceType? SecondaryDiceType { get; set; }
    public SecondaryDiceRule SecondaryRule { get; set; }
}