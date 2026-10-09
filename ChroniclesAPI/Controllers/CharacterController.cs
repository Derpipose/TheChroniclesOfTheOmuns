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
            .Select(c => new CharacterDetailsDto(c.Id, c.Name, c.Level, c.Setting, c.CurrentHealth, c.IsActive))
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
            .Select(c => new CharacterDetailsDto(c.Id, c.Name, c.Level, c.Setting, c.CurrentHealth, c.IsActive))
            .ToListAsync();

        return Ok(characters);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CharacterResponseDto>>> GetPlayerCharacters() {
        var characters = await db.Characters
            .Where(c => c.PlayerId == playerContext.PlayerId)
            .OrderByDescending(c => c.LastModified)
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
        character.SetNotes(request.Notes);

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
        if (request.Level is not null) character.SetLevel(request.Level.Value);
        if (request.IsActive is true) character.SetActive();

        await db.SaveChangesAsync();
        var stats = await db.CharacterStats.FirstOrDefaultAsync(cs => cs.CharacterId == character.Id);
        return Ok(MapToResponse(character, stats));
    }

    [HttpPatch("{id:int}/notes")]
    public async Task<IActionResult> UpdateCharacterNotes(int id, [FromBody] string notes) {
        var character = await db.Characters
            .FirstOrDefaultAsync(c => c.Id == id && c.PlayerId == playerContext.PlayerId);

        if (character is null)
            return NotFound(new { message = $"Character {id} was not found." });

        character.SetNotes(notes);
        await db.SaveChangesAsync();

        return NoContent();
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
        character.CurrentHealth,
        character.IsActive,
        character.Notes,
        stats is null ? null : new CharacterStatsResponseDto(
            stats.Id,
            stats.CharacterId,
            stats.Strength,
            stats.Constitution,
            stats.Dexterity,
            stats.Wisdom,
            stats.Charisma,
            stats.Intelligence));

    public record CreateCharacterDto(string Name, int Level = 1, string Notes = "");
    public record CharacterDetailsDto(int Id, string Name, int Level, SettingEnum Setting, int CurrentHealth, bool IsActive);
    public record UpdateCharacterDto(string? Name, int? Level, bool? IsActive);
    public record CharacterResponseDto(int Id, string Name, int PlayerId, int Level, SettingEnum Setting, int CurrentHealth, bool IsActive, string Notes, CharacterStatsResponseDto? CharacterStats);
    public record CharacterStatsResponseDto(int Id, int CharacterId, int Strength, int Constitution, int Dexterity, int Wisdom, int Charisma, int Intelligence);
}


// Eventually we will need a "getSettingsForCharacter" endpoint that will look at the different setting tables and 
// return a list of settings that the character has a sub in. We will use that to correctly call the right
// endpoints for character creation and updates. For right now, we don't need to worry about that since we only have
// one setting, but in the future we will need to make sure that we are not creating a character in a setting that
// the character already has a sub in. For example, if a character has a sub in the Scifi setting, we should not 
// allow them to create a new ScifiCharacter for that character. That could cause issues. Each will have it's 
// own check to ensure that doesn't happen, but just in case. 