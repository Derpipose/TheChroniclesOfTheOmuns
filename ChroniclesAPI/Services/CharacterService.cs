using ChroniclesAPI.Database;
using PlayerApp.Models;
using PlayerApp.Models.Verification;

namespace ChroniclesAPI.Services;

public class CharacterService {
    private readonly AppDbContext _db;

    public CharacterService(AppDbContext db) {
        _db = db;
    }

    public bool VerifyCharacterDB(Character character, out string errorMessage) {
        if (!VerifyCharacter.IsValid(character, out errorMessage)) {
            return false;
        }
        
        if (character.CharacterRace != null && !_db.CharacterRaces.Any(r => r.Id == character.CharacterRace.Id)) {
            errorMessage = "Invalid RaceId.";
            return false;
        }        
        if (character.CharacterClass != null && !_db.CharacterClasses.Any(c => c.Id == character.CharacterClass.Id)) {
            errorMessage = "Invalid ClassId.";
            return false;
        }

        return true;
    }

    public Character BuildVerifiedCharacter(Character character) {
        Character verifiedCharacter = new Character {
            Name = character.Name,
            Level = character.Level,
            Stats = character.Stats
        };
        if (character.CharacterClass != null) {
            verifiedCharacter.AssignCharacterClass(character.CharacterClass);
        }
        if (character.CharacterRace != null) {
            verifiedCharacter.AssignCharacterRace(character.CharacterRace);
        }
        return verifiedCharacter;
    }
}