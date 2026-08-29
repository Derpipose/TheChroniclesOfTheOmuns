namespace PlayerApp.Models.Dice;

public static class DiceTypeExtensions
{
    public static int Sides(this DiceType die) => (int)die;

    public static string DisplayName(this DiceType die) => $"D{die.Sides()}";
}
