using ChroniclesAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticAssets;
using Microsoft.EntityFrameworkCore;
using PlayerApp.Models;
using PlayerApp.Models.Database;
using PlayerApp.Models.Enums;

namespace ChroniclesAPI.Controllers;

[ApiController]
[Route("api/scifi/[controller]")]
[Authorize]
public class ScifiStatsController(AppDbContext db, PlayerContext playerContext) : ControllerBase {

    [HttpGet("by-character/{characterId:int}")]
    public async Task<IActionResult> GetWithAdjustedStats(int characterId) {
        var character = await db.Characters
            .FirstOrDefaultAsync(c => c.Id == characterId && c.PlayerId == playerContext.PlayerId);

        if (character is null)
            return NotFound(new { message = $"Character {characterId} not found." });

        if (character.Setting != SettingEnum.Scifi)
            return BadRequest(new { message = $"Character {characterId} is not a scifi character." });

        var scifiCharacter = await db.ScifiCharacters
            .Include(sc => sc.Modifiers)
            .Include(sc => sc.Character)
            .FirstOrDefaultAsync(sc => sc.CharacterId == characterId);

        if (scifiCharacter is null)
            return NotFound(new { message = $"ScifiCharacter for {characterId} not found." });

        var characterStats = await db.CharacterStats
            .FirstOrDefaultAsync(cs => cs.CharacterId == characterId)
            ?? new CharacterStats(characterId);

        var modifiers = scifiCharacter.Modifiers.Cast<ICharacterModifier>();

        var adjusted = new AdjustedStatsDto(
            Strength: scifiCharacter.Character!.GetStat(modifiers, ModifierType.Strength, characterStats.Strength),
            Dexterity: scifiCharacter.Character.GetStat(modifiers, ModifierType.Dexterity, characterStats.Dexterity),
            Constitution: scifiCharacter.Character.GetStat(modifiers, ModifierType.Constitution, characterStats.Constitution),
            Intelligence: scifiCharacter.Character.GetStat(modifiers, ModifierType.Intelligence, characterStats.Intelligence),
            Wisdom: scifiCharacter.Character.GetStat(modifiers, ModifierType.Wisdom, characterStats.Wisdom),
            Charisma: scifiCharacter.Character.GetStat(modifiers, ModifierType.Charisma, characterStats.Charisma),
            ArmorClass: scifiCharacter.Character.GetArmorClass(modifiers),
            MaxHealth: scifiCharacter.Character.GetMaxHealth(modifiers),
            CurrentHealth: scifiCharacter.Character.CurrentHealth
        );

        return Ok(adjusted);
    }

    public record AdjustedStatsDto(int Strength, int Dexterity, int Constitution, int Intelligence, int Wisdom, int Charisma, int ArmorClass, int MaxHealth, int CurrentHealth);
}