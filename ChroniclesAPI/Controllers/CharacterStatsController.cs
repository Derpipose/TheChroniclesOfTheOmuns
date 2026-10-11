using ChroniclesAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayerApp.Models;
using PlayerApp.Models.Database;
using PlayerApp.Models.Enums;

namespace ChroniclesAPI.Controllers;

[ApiController]
[Route("api/character")]
[Authorize]
public class CharacterStatsController(AppDbContext db, PlayerContext playerContext) : ControllerBase {

    [HttpGet("{characterId:int}/stats")]
    public async Task<IActionResult> GetStats(int characterId) {
        var character = await db.Characters
            .FirstOrDefaultAsync(c => c.Id == characterId && c.PlayerId == playerContext.PlayerId);

        if (character is null)
            return NotFound(new { message = $"Character {characterId} was not found." });

        var stats = await db.CharacterStats
            .FirstOrDefaultAsync(cs => cs.CharacterId == characterId);

        if (stats is null)
            return NotFound(new { message = $"Stats for character {characterId} were not found." });

        return Ok(new CharacterStatsResponseDto(
            stats.Id,
            stats.CharacterId,
            stats.Strength,
            stats.Constitution,
            stats.Dexterity,
            stats.Wisdom,
            stats.Charisma,
            stats.Intelligence));
    }

    [HttpPut("{characterId:int}/stats")]
    public async Task<IActionResult> BulkUpdateStats(int characterId, [FromBody] BulkUpdateStatsDto request) {
        var character = await db.Characters
            .FirstOrDefaultAsync(c => c.Id == characterId && c.PlayerId == playerContext.PlayerId);

        if (character is null)
            return NotFound(new { message = $"Character {characterId} was not found." });

        if (character.IsActive)
            return BadRequest(new { message = "Cannot bulk update stats of an active character." });

        var stats = await db.CharacterStats
            .FirstOrDefaultAsync(cs => cs.CharacterId == characterId);

        if (stats is null)
            return NotFound(new { message = $"Stats for character {characterId} were not found." });

        var modifiers = await LoadModifiers(characterId);
        try {
            stats.SetStat(StatType.Strength, request.Strength, character, modifiers);
            stats.SetStat(StatType.Constitution, request.Constitution, character, modifiers);
            stats.SetStat(StatType.Dexterity, request.Dexterity, character, modifiers);
            stats.SetStat(StatType.Wisdom, request.Wisdom, character, modifiers);
            stats.SetStat(StatType.Charisma, request.Charisma, character, modifiers);
            stats.SetStat(StatType.Intelligence, request.Intelligence, character, modifiers);
        } catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) {
            return BadRequest(new { message = ex.Message });
        }

        await db.SaveChangesAsync();
        return Ok(new CharacterStatsResponseDto(
            stats.Id,
            stats.CharacterId,
            stats.Strength,
            stats.Constitution,
            stats.Dexterity,
            stats.Wisdom,
            stats.Charisma,
            stats.Intelligence));
    }

    [HttpPatch("{characterId:int}/stats")]
    public async Task<IActionResult> UpdateSingleStat(int characterId, [FromBody] UpdateSingleStatDto request) {
        var character = await db.Characters
            .FirstOrDefaultAsync(c => c.Id == characterId && c.PlayerId == playerContext.PlayerId);

        if (character is null)
            return NotFound(new { message = $"Character {characterId} was not found." });

        if (!character.IsActive)
            return BadRequest(new { message = "Cannot use single stat update on an inactive character." });

        var stats = await db.CharacterStats
            .FirstOrDefaultAsync(cs => cs.CharacterId == characterId);

        if (stats is null)
            return NotFound(new { message = $"Stats for character {characterId} were not found." });

        try {
            if (request.Value > 0)
                stats.IncreaseStat(request.Stat, request.Value, character);
            else if (request.Value < 0)
                stats.DecreaseStat(request.Stat, Math.Abs(request.Value), character, await LoadModifiers(characterId));
            else
                return BadRequest(new { message = "Value cannot be zero." });
        } catch (InvalidOperationException ex) {
            return BadRequest(new { message = ex.Message });
        }

        await db.SaveChangesAsync();
        return Ok(new CharacterStatsResponseDto(
            stats.Id,
            stats.CharacterId,
            stats.Strength,
            stats.Constitution,
            stats.Dexterity,
            stats.Wisdom,
            stats.Charisma,
            stats.Intelligence));
    }

    private async Task<List<ICharacterModifier>> LoadModifiers(int characterId) =>
        (await db.ScifiCharacterModifiers
            .Where(m => db.ScifiCharacters.Any(sc => sc.Id == m.ScifiCharacterId && sc.CharacterId == characterId))
            .ToListAsync())
        .Cast<ICharacterModifier>()
        .ToList();

    public record BulkUpdateStatsDto(int Strength, int Constitution, int Dexterity, int Wisdom, int Charisma, int Intelligence);
    public record UpdateSingleStatDto(StatType Stat, int Value);
    public record CharacterStatsResponseDto(int Id, int CharacterId, int Strength, int Constitution, int Dexterity, int Wisdom, int Charisma, int Intelligence);
}