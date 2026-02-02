namespace PlayerApp.Models.Enums;

public enum SecondaryDiceRule {
    None,                   // No secondary dice
    AddFlat,                // Add flat number of secondary dice
    UsePrimaryResultAsCount // Roll primary, result = count of secondary dice
}