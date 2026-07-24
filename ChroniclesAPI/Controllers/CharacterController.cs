using ChroniclesAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayerApp.Models;
using PlayerApp.Models.Database;
using PlayerApp.Models.Enums;

namespace ChroniclesAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CharacterController(AppDbContext db, PlayerContext playerContext) : ControllerBase {

    [HttpGet("{id:int}/details")]
    public async Task<ActionResult<CharacterDetailsDto>> GetCharacterDetails(int id) {
        var character = await db.Characters
            .Where(c => c.Id == id && c.PlayerId == playerContext.PlayerId)
            .Select(c => new CharacterDetailsDto(c.Id, c.Name, c.Level, c.Setting, c.MaxHealth, c.IsActive))
            .FirstOrDefaultAsync();

        if (character is null)
            return NotFound(new { message = $"Character {id} was not found." });

        return Ok(character);
    }

    [HttpGet("details")]
    public async Task<ActionResult<IEnumerable<CharacterDetailsDto>>> GetAllCharacterDetails() {
        var characters = await db.Characters
            .Where(c => c.PlayerId == playerContext.PlayerId)
            .OrderBy(c => c.Id)
            .Select(c => new CharacterDetailsDto(c.Id, c.Name, c.Level, c.Setting, c.MaxHealth, c.IsActive))
            .ToListAsync();

        return Ok(characters);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CharacterResponseDto>>> GetPlayerCharacters() {
        var characters = await db.Characters
            .Where(c => c.PlayerId == playerContext.PlayerId)
            .OrderBy(c => c.Id)
            .ToListAsync();

        var characterIds = characters.Select(c => c.Id).ToList();
        var statsByCharacterId = await db.CharacterStats
            .Where(cs => characterIds.Contains(cs.CharacterId))
            .ToDictionaryAsync(cs => cs.CharacterId);

        return Ok(characters.Select(c => MapToResponse(c, statsByCharacterId.TryGetValue(c.Id, out var stats) ? stats : null)));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CharacterResponseDto>> GetCharacter(int id) {
        var character = await db.Characters
            .FirstOrDefaultAsync(c => c.Id == id && c.PlayerId == playerContext.PlayerId);

        if (character is null)
            return NotFound(new { message = $"Character {id} was not found." });

        var stats = await db.CharacterStats.FirstOrDefaultAsync(cs => cs.CharacterId == character.Id);
        return Ok(MapToResponse(character, stats));
    }

    [HttpPost]
    public async Task<ActionResult<CharacterResponseDto>> CreateCharacter([FromBody] CreateCharacterDto request) {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { message = "Character name is required." });

        var character = new Character(playerContext.PlayerId, request.Name);
        character.SetLevel(request.Level);

        db.Characters.Add(character);
        await db.SaveChangesAsync();

        var stats = new CharacterStats(character.Id);
        db.CharacterStats.Add(stats);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCharacter), new { id = character.Id }, MapToResponse(character, stats));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CharacterResponseDto>> UpdateCharacter(int id, [FromBody] UpdateCharacterDto request) {
        var character = await db.Characters
            .FirstOrDefaultAsync(c => c.Id == id && c.PlayerId == playerContext.PlayerId);

        if (character is null)
            return NotFound(new { message = $"Character {id} was not found." });

        if (request.Name is not null) character.SetName(request.Name);
        if (request.MaxHealth is not null) character.SetMaxHealth(request.MaxHealth.Value);
        if (request.Level is not null) character.SetLevel(request.Level.Value);
        if (request.IsActive is true) character.SetActive();

        await db.SaveChangesAsync();
        var stats = await db.CharacterStats.FirstOrDefaultAsync(cs => cs.CharacterId == character.Id);
        return Ok(MapToResponse(character, stats));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCharacter(int id) {
        var character = await db.Characters
            .FirstOrDefaultAsync(c => c.Id == id && c.PlayerId == playerContext.PlayerId);

        if (character is null)
            return NotFound(new { message = $"Character {id} was not found." });

        var stats = await db.CharacterStats.FirstOrDefaultAsync(cs => cs.CharacterId == character.Id);
        if (stats is not null) db.CharacterStats.Remove(stats);

        db.Characters.Remove(character);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static CharacterResponseDto MapToResponse(Character character, CharacterStats? stats) => new(
        character.Id,
        character.Name,
        character.PlayerId,
        character.Level,
        character.Setting,
        character.MaxHealth,
        character.IsActive,
        stats is null ? null : new CharacterStatsResponseDto(
            stats.Id,
            stats.CharacterId,
            stats.Strength,
            stats.Constitution,
            stats.Dexterity,
            stats.Wisdom,
            stats.Charisma,
            stats.Intelligence));

    public record CreateCharacterDto(string Name, int Level = 1);
    public record CharacterDetailsDto(int Id, string Name, int Level, SettingEnum Setting, int MaxHealth, bool IsActive);
    public record UpdateCharacterDto(string? Name, int? MaxHealth, int? Level, bool? IsActive);
    public record CharacterResponseDto(int Id, string Name, int PlayerId, int Level, SettingEnum Setting, int MaxHealth, bool IsActive, CharacterStatsResponseDto? CharacterStats);
    public record CharacterStatsResponseDto(int Id, int CharacterId, int Strength, int Constitution, int Dexterity, int Wisdom, int Charisma, int Intelligence);
}