namespace PlayerApp.Models.Enums;

public enum SecondaryDiceRule {
    None = 0,                   // No secondary dice
    AddFlat = 1,                // Add flat number of secondary dice
    UsePrimaryResultAsCount = 2 // Roll primary, result = count of secondary dice
}