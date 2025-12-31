using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlayerAppAvalonia.Database;
using PlayerApp.Models;

namespace PlayerAppAvalonia.Services;

public class AppCharacterService {
    private readonly ApplicationDbContext _dbContext;

    public AppCharacterService(ApplicationDbContext dbContext) {
        _dbContext = dbContext;
    }

    public async Task<List<Character>> GetAllCharactersAsync()
        => await _dbContext.Character
            .Include(c => c.CharacterClass)
            .Include(c => c.CharacterRace)
            .ToListAsync();

    public async Task<Character?> GetCharacterByIdAsync(int id)
        => await _dbContext.Character.FindAsync(id);

    public async Task<Character> CreateCharacterAsync(Character character) {
        _dbContext.Character.Add(character);
        await _dbContext.SaveChangesAsync();
        return character;
    }

    public async Task<Character> UpdateCharacterAsync(Character character) {
        _dbContext.Character.Update(character);
        await _dbContext.SaveChangesAsync();
        return character;
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
