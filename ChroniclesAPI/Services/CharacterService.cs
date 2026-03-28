using ChroniclesAPI.Database;
using PlayerApp.Models;
using PlayerApp.Models.Verification;
using Microsoft.EntityFrameworkCore;

namespace ChroniclesAPI.Services;

public class CharacterService {
    private readonly AppDbContext _db;
    private readonly PlayerApp.Models.CharacterService _modelService;


    public CharacterService(AppDbContext db) {
        _db = db;
        _modelService = new PlayerApp.Models.CharacterService();
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

    public void UpdateCharacter(Character existingCharacter, Character updatedData) {
        // Update basic info
        existingCharacter.Name = updatedData.Name;
        existingCharacter.Level = updatedData.Level;
        
        // Update stats
        if (updatedData.Stats != null) {
            existingCharacter.Stats = updatedData.Stats;
        }
        
        // Update race (if provided and valid)
        if (updatedData.CharacterRace != null) {
            var race = _db.CharacterRaces
                .Include(r => r.Modifiers)
                .ThenInclude(m => m.Modifier)
                .FirstOrDefault(r => r.Id == updatedData.CharacterRace.Id);
            if (race != null) {
                // Use Models.CharacterService to handle race update logic
                _modelService.UpdateCharacterRace(existingCharacter, race);
            }
        } else if (updatedData.CharacterRace == null && existingCharacter.CharacterRace != null) {
            _modelService.RemoveCharacterRace(existingCharacter);
        }
        
        // Update class (if provided and valid)
        if (updatedData.CharacterClass != null) {
            var characterClass = _db.CharacterClasses
                .FirstOrDefault(c => c.Id == updatedData.CharacterClass.Id);
            if (characterClass != null) {
                _modelService.UpdateCharacterClass(existingCharacter, characterClass);
            }
        } else if (updatedData.CharacterClass == null && existingCharacter.CharacterClass != null) {
            _modelService.RemoveCharacterClass(existingCharacter);
        }
    }
}