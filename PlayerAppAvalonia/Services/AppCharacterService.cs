using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlayerAppAvalonia.Database;
using PlayerApp.Models;

namespace PlayerAppAvalonia.Services;

public class AppCharacterService {
    private readonly ApplicationDbContext _dbContext;
    private readonly CharacterService _characterService;

    public AppCharacterService(ApplicationDbContext dbContext) {
        _dbContext = dbContext;
        _characterService = new CharacterService();
    }

    private IQueryable<Character> GetCharactersWithRelations(IQueryable<Character> query) 
        => query
            .Include(c => c.CharacterClass)
                .ThenInclude(cc => cc!.HitDice)
            .Include(c => c.CharacterClass)
                .ThenInclude(cc => cc!.ManaDice)
            .Include(c => c.CharacterRace)
                .ThenInclude(cr => cr!.Modifiers)
                    .ThenInclude(m => m.Modifier)
            .Include(c => c.Stats);

    public async Task<List<Character>> GetAllCharactersAsync()
        => await GetCharactersWithRelations(_dbContext.Character).ToListAsync();

    public async Task<Character?> GetCharacterByIdAsync(int id)
        => await GetCharactersWithRelations(_dbContext.Character).FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Character> CreateCharacterAsync(Character character) {
        // Reload class and race with all relationships before calculating
        if (character.CharacterClass != null) {
            var classWithDice = await _dbContext.CharacterClass
                .Include(c => c.HitDice)
                .Include(c => c.ManaDice)
                .FirstOrDefaultAsync(c => c.Id == character.CharacterClass.Id);
            if (classWithDice != null) {
                character.AssignCharacterClass(classWithDice);
            }
        }
        
        if (character.CharacterRace != null) {
            var raceWithModifiers = await _dbContext.CharacterRace
                .Include(r => r.Modifiers)
                    .ThenInclude(m => m.Modifier)
                .FirstOrDefaultAsync(r => r.Id == character.CharacterRace.Id);
            if (raceWithModifiers != null) {
                character.AssignCharacterRace(raceWithModifiers);
            }
        }
        
        // Now calculate with fully loaded objects
        _characterService.UpdateCharacterClassAndCalculateAttributes(character, character.CharacterClass!);
        _characterService.UpdateCharacterRaceAndCalculateAttributes(character, character.CharacterRace!);
        
        _dbContext.Character.Add(character);
        await _dbContext.SaveChangesAsync();
        return character;
    }

    public async Task<Character> UpdateCharacterAsync(Character character) {
        var fullCharacter = await GetCharactersWithRelations(_dbContext.Character)
            .FirstOrDefaultAsync(c => c.Id == character.Id);

        if (fullCharacter == null)
            return character;

        fullCharacter.Name = character.Name;
        fullCharacter.Level = character.Level;

        if (character.Stats != null) {
            fullCharacter.Stats.Strength = character.Stats.Strength;
            fullCharacter.Stats.Constitution = character.Stats.Constitution;
            fullCharacter.Stats.Dexterity = character.Stats.Dexterity;
            fullCharacter.Stats.Wisdom = character.Stats.Wisdom;
            fullCharacter.Stats.Charisma = character.Stats.Charisma;
            fullCharacter.Stats.Intelligence = character.Stats.Intelligence;
        }

        if (character.CharacterClass != null && character.CharacterClass.Id != fullCharacter.CharacterClass?.Id) {
            fullCharacter.AssignCharacterClass(character.CharacterClass);
        }

        if (character.CharacterRace != null && character.CharacterRace.Id != fullCharacter.CharacterRace?.Id) {
            fullCharacter.AssignCharacterRace(character.CharacterRace);
        }

        if (fullCharacter.CharacterClass != null) {
            _characterService.UpdateCharacterClassAndCalculateAttributes(fullCharacter, fullCharacter.CharacterClass);
        }
        if (fullCharacter.CharacterRace != null) {
            _characterService.UpdateCharacterRaceAndCalculateAttributes(fullCharacter, fullCharacter.CharacterRace);
        }
        
        _dbContext.Character.Update(fullCharacter);
        await _dbContext.SaveChangesAsync();
        return fullCharacter;
    }

    public async Task DeleteCharacterAsync(int id) {
        var character = await _dbContext.Character.FindAsync(id);
        if (character != null) {
            _dbContext.Character.Remove(character);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<List<CharacterClass>> GetAllClassesAsync()
        => await _dbContext.CharacterClass.ToListAsync();

    public async Task<List<CharacterRace>> GetAllRacesAsync()
        => await _dbContext.CharacterRace.ToListAsync();

    public async Task CreateClassAsync(CharacterClass characterClass) {
        _dbContext.CharacterClass.Add(characterClass);
        await _dbContext.SaveChangesAsync();
    }

    public async Task CreateRaceAsync(CharacterRace race) {
        _dbContext.CharacterRace.Add(race);
        await _dbContext.SaveChangesAsync();
    }
}
