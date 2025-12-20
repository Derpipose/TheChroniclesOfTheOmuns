using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlayerApp.Database;
using PlayerApp.Models;

namespace PlayerApp.Services;

public class AppCharacterService
{
    private readonly ApplicationDbContext _dbContext;

    public AppCharacterService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Character>> GetAllCharactersAsync()
        => await _dbContext.Character.ToListAsync();

    public async Task<Character?> GetCharacterByIdAsync(int id)
        => await _dbContext.Character.FindAsync(id);

    public async Task<Character> CreateCharacterAsync(Character character)
    {
        _dbContext.Character.Add(character);
        await _dbContext.SaveChangesAsync();
        return character;
    }

    public async Task<Character> UpdateCharacterAsync(Character character)
    {
        _dbContext.Character.Update(character);
        await _dbContext.SaveChangesAsync();
        return character;
    }

    public async Task DeleteCharacterAsync(int id)
    {
        var character = await _dbContext.Character.FindAsync(id);
        if (character != null)
        {
            _dbContext.Character.Remove(character);
            await _dbContext.SaveChangesAsync();
        }
    }
}
