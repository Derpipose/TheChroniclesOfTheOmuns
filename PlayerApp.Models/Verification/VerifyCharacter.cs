using System;
using PlayerApp.Models.Enums;
using PlayerApp.Models;

namespace PlayerApp.Models.Verification;

public class VerifyCharacter {
    public static bool IsValid(Character character, out string errorMessage) {
        if (string.IsNullOrWhiteSpace(character.Name)) {
            errorMessage = "Character name cannot be empty.";
            return false;
        }
        if (character.Level < 1) {
            errorMessage = "Character level must be at least 1.";
            return false;
        }
        if(character.Stats.Strength < 0 || 
           character.Stats.Constitution < 0 || 
           character.Stats.Dexterity < 0 ||
           character.Stats.Wisdom < 0 ||
           character.Stats.Charisma < 0 ||
           character.Stats.Intelligence < 0) {
            errorMessage = "Character stats cannot be negative.";
            return false;
        }
        
        errorMessage = "";
        return true;
    }
}