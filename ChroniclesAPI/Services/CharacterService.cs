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

    public bool VerifyCharacterDB(FantasyCharacter character, out string errorMessage) {
        if (!VerifyCharacter.IsValid(character, out errorMessage)) {
            return false;
        }
        
        if (character.FantasyCharacterRace != null && !_db.FantasyCharacterRaces.Any(r => r.Id == character.FantasyCharacterRace.Id)) {
            errorMessage = "Invalid RaceId.";
            return false;
        }        
        if (character.FantasyCharacterClass != null && !_db.FantasyCharacterClasses.Any(c => c.Id == character.FantasyCharacterClass.Id)) {
            errorMessage = "Invalid ClassId.";
            return false;
        }

        return true;
    }

    public FantasyCharacter BuildVerifiedCharacter(FantasyCharacter character) {
        FantasyCharacter verifiedCharacter = new FantasyCharacter {
            Name = character.Name,
            Level = character.Level,
            Stats = character.Stats
        };
        if (character.FantasyCharacterClass != null) {
            verifiedCharacter.AssignFantasyCharacterClass(character.FantasyCharacterClass);
        }
        if (character.FantasyCharacterRace != null) {
            verifiedCharacter.AssignFantasyCharacterRace(character.FantasyCharacterRace);
        }
        return verifiedCharacter;
    }

    public void UpdateCharacter(FantasyCharacter existingCharacter, FantasyCharacter updatedData) {
        // Update basic info
        existingCharacter.Name = updatedData.Name;
        existingCharacter.Level = updatedData.Level;
        
        // Update stats
        if (updatedData.Stats != null) {
            existingCharacter.Stats = updatedData.Stats;
        }
        
        // Update race (if provided and valid)
        if (updatedData.FantasyCharacterRace != null) {
            var race = _db.FantasyCharacterRaces
                .Include(r => r.Modifiers)
                .ThenInclude(m => m.Modifier)
                .FirstOrDefault(r => r.Id == updatedData.FantasyCharacterRace.Id);
            if (race != null) {
                // Use Models.CharacterService to handle race update logic
                _modelService.UpdateFantasyCharacterRace(existingCharacter, race);
            }
        } else if (updatedData.FantasyCharacterRace == null && existingCharacter.FantasyCharacterRace != null) {
            _modelService.RemoveFantasyCharacterRace(existingCharacter);
        }
        
        // Update class (if provided and valid)
        if (updatedData.FantasyCharacterClass != null) {
            var FantasyCharacterClass = _db.FantasyCharacterClasses
                .FirstOrDefault(c => c.Id == updatedData.FantasyCharacterClass.Id);
            if (FantasyCharacterClass != null) {
                _modelService.UpdateFantasyCharacterClass(existingCharacter, FantasyCharacterClass);
            }
        } else if (updatedData.FantasyCharacterClass == null && existingCharacter.FantasyCharacterClass != null) {
            _modelService.RemoveFantasyCharacterClass(existingCharacter);
        }
    }
}