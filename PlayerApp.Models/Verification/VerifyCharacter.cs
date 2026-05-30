using System;
using PlayerApp.Models.Enums;
using PlayerApp.Models.Legacy;

namespace PlayerApp.Models.Verification;

public class VerifyCharacter {
    public static bool IsValid(FantasyCharacter character, out string errorMessage) {
        if (string.IsNullOrWhiteSpace(character.Name)) {
            errorMessage = "FantasyCharacter name cannot be empty.";
            return false;
        }
        if (character.Level < 1) {
            errorMessage = "FantasyCharacter level must be at least 1.";
            return false;
        }
        if(character.Stats.Strength < 0 || 
           character.Stats.Constitution < 0 || 
           character.Stats.Dexterity < 0 ||
           character.Stats.Wisdom < 0 ||
           character.Stats.Charisma < 0 ||
           character.Stats.Intelligence < 0) {
            errorMessage = "FantasyCharacter stats cannot be negative.";
            return false;
        }
        
        errorMessage = "";
        return true;
    }
}