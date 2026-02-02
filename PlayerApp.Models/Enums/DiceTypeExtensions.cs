namespace PlayerApp.Models.Enums;

public static class DiceTypeExtensions
{
    /// <summary>
    /// Gets the number of sides for a dice type.
    /// </summary>
    public static int Sides(this DiceType die) => (int)die;

    /// <summary>
    /// Gets the display name for a dice type (e.g., "D20").
    /// </summary>
    public static string DisplayName(this DiceType die) => $"D{die.Sides()}";
}
