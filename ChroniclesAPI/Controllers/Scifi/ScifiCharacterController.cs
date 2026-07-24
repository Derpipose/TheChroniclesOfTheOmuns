using ChroniclesAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayerApp.Models;
using PlayerApp.Models.Database;
using PlayerApp.Models.Enums;

namespace ChroniclesAPI.Controllers;

[ApiController]
[Route("api/scifi/[controller]")]
[Authorize]
public class ScifiCharacterController(AppDbContext db, PlayerContext playerContext) : ControllerBase {

    [HttpGet]
    public async Task<IActionResult> GetAll() {
        var characters = await db.ScifiCharacters
            .Include(sc => sc.Character)
            .Where(sc => sc.Character!.PlayerId == playerContext.PlayerId)
            .Select(sc => new ScifiCharacterResponseDto(
                sc.Id,
                sc.CharacterId,
                sc.ScifiRaceId,
                sc.ScifiSubraceId,
                sc.ScifiClassId,
                sc.ScifiBackgroundId))
            .ToListAsync();

        return Ok(characters);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) {
        var scifiCharacter = await db.ScifiCharacters
            .Include(sc => sc.Character)
            .Where(sc => sc.Id == id && sc.Character!.PlayerId == playerContext.PlayerId)
            .Select(sc => new ScifiCharacterResponseDto(
                sc.Id,
                sc.CharacterId,
                sc.ScifiRaceId,
                sc.ScifiSubraceId,
                sc.ScifiClassId,
                sc.ScifiBackgroundId))
            .FirstOrDefaultAsync();

        if (scifiCharacter is null)
            return NotFound(new { message = $"ScifiCharacter {id} was not found." });

        return Ok(scifiCharacter);
    }

    [HttpGet("by-character/{characterId:int}")]
    public async Task<IActionResult> GetByCharacterId(int characterId) {
        var scifiCharacter = await db.ScifiCharacters
            .Include(sc => sc.Character)
            .Where(sc => sc.CharacterId == characterId && sc.Character!.PlayerId == playerContext.PlayerId)
            .Select(sc => new ScifiCharacterResponseDto(
                sc.Id,
                sc.CharacterId,
                sc.ScifiRaceId,
                sc.ScifiSubraceId,
                sc.ScifiClassId,
                sc.ScifiBackgroundId))
            .FirstOrDefaultAsync();

        if (scifiCharacter is null)
            return NotFound(new { message = $"ScifiCharacter for Character {characterId} was not found." });

        return Ok(scifiCharacter);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateScifiCharacterDto request) {
        var character = await db.Characters
            .FirstOrDefaultAsync(c => c.Id == request.CharacterId && c.PlayerId == playerContext.PlayerId);

        if (character is null)
            return NotFound(new { message = "Character not found." });

        if (character.IsActive)
            return BadRequest(new { message = "Cannot create a ScifiCharacter for an active character." });

        if (character.Setting == SettingEnum.Scifi) {
            return BadRequest(new { message = "Cannot create a ScifiCharacter for a character that is already in the Scifi setting." });
        }

        // Validate optional race exists and is scifi
        if (request.ScifiRaceId is not null) {
            // TODO: When multiple settings exist, ensure this race actually belongs to the Scifi setting
            var raceExists = await db.ScifiRaces.AnyAsync(r => r.Id == request.ScifiRaceId);
            if (!raceExists)
                return BadRequest(new { message = "ScifiRace not found." });
        }

        // Validate optional subrace exists
        if (request.ScifiSubraceId is not null) {
            var subraceExists = await db.ScifiSubraces.AnyAsync(sr => sr.Id == request.ScifiSubraceId);
            if (!subraceExists)
                return BadRequest(new { message = "ScifiSubrace not found." });
        }

        // Validate optional class exists and is scifi
        if (request.ScifiClassId is not null) {
            // TODO: When multiple settings exist, ensure this class actually belongs to the Scifi setting
            var classExists = await db.ScifiClasses.AnyAsync(c => c.Id == request.ScifiClassId);
            if (!classExists)
                return BadRequest(new { message = "ScifiClass not found." });
        }

        // Validate optional background exists and is scifi
        if (request.ScifiBackgroundId is not null) {
            // TODO: When multiple settings exist, ensure this background actually belongs to the Scifi setting
            var backgroundExists = await db.ScifiBackgrounds.AnyAsync(b => b.Id == request.ScifiBackgroundId);
            if (!backgroundExists)
                return BadRequest(new { message = "ScifiBackground not found." });
        }

        // Upsert: if scifi character already exists for this character, update it instead
        var existing = await db.ScifiCharacters
            .FirstOrDefaultAsync(sc => sc.CharacterId == request.CharacterId);

        if (existing is not null) {
            // Subrace requires a race to be set (either already persisted, or provided in this same request)
            if (request.ScifiSubraceId is not null && request.ScifiRaceId is null && existing.ScifiRaceId == 0)
                return BadRequest(new { message = "ScifiRace must be set before assigning a ScifiSubrace." });

            if (request.ScifiRaceId is not null) {
                var race = await db.ScifiRaces.FirstAsync(r => r.Id == request.ScifiRaceId);
                existing.SetScifiRace(race.Id);
                existing.ClearScifiSubrace();
            }
            if (request.ScifiSubraceId is not null) {
                var subrace = await db.ScifiSubraces.FirstAsync(sr => sr.Id == request.ScifiSubraceId);
                existing.SetScifiSubrace(subrace.Id);
            }
            if (request.ScifiClassId is not null) {
                var scifiClass = await db.ScifiClasses.FirstAsync(c => c.Id == request.ScifiClassId);
                existing.SetScifiClass(scifiClass.Id);
            }
            if (request.ScifiBackgroundId is not null) {
                var background = await db.ScifiBackgrounds.FirstAsync(b => b.Id == request.ScifiBackgroundId);
                existing.SetScifiBackground(background.Id);
            }

            character.SetSetting(SettingEnum.Scifi);
            character.SetMaxHealth(0);
            await db.SaveChangesAsync();

            return Ok(new ScifiCharacterResponseDto(
                existing.Id,
                existing.CharacterId,
                existing.ScifiRaceId,
                existing.ScifiSubraceId,
                existing.ScifiClassId,
                existing.ScifiBackgroundId));
        }

        // No existing scifi character, create new
        if (request.ScifiSubraceId is not null && request.ScifiRaceId is null)
            return BadRequest(new { message = "ScifiRace must be set before assigning a ScifiSubrace." });

        character.SetSetting(SettingEnum.Scifi);
        character.SetMaxHealth(0);

        var scifiCharacter = new ScifiCharacter(character.Id);

        if (request.ScifiRaceId is not null) {
            var race = await db.ScifiRaces.FirstAsync(r => r.Id == request.ScifiRaceId);
            scifiCharacter.SetScifiRace(race.Id);
        }

        if (request.ScifiSubraceId is not null) {
            var subrace = await db.ScifiSubraces.FirstAsync(sr => sr.Id == request.ScifiSubraceId);
            scifiCharacter.SetScifiSubrace(subrace.Id);
        }

        if (request.ScifiClassId is not null) {
            var scifiClass = await db.ScifiClasses.FirstAsync(c => c.Id == request.ScifiClassId);
            scifiCharacter.SetScifiClass(scifiClass.Id);
        }

        if (request.ScifiBackgroundId is not null) {
            var background = await db.ScifiBackgrounds.FirstAsync(b => b.Id == request.ScifiBackgroundId);
            scifiCharacter.SetScifiBackground(background.Id);
        }

        db.ScifiCharacters.Add(scifiCharacter);
        await db.SaveChangesAsync();

        return Ok(new ScifiCharacterResponseDto(
            scifiCharacter.Id,
            scifiCharacter.CharacterId,
            scifiCharacter.ScifiRaceId,
            scifiCharacter.ScifiSubraceId,
            scifiCharacter.ScifiClassId,
            scifiCharacter.ScifiBackgroundId));
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateScifiCharacterDto request) {
        var scifiCharacter = await db.ScifiCharacters
            .Include(sc => sc.Character)
            .FirstOrDefaultAsync(sc => sc.Id == id && sc.Character!.PlayerId == playerContext.PlayerId);

        if (scifiCharacter is null)
            return NotFound(new { message = $"ScifiCharacter {id} was not found." });

        if (scifiCharacter.Character!.IsActive)
            return BadRequest(new { message = "Cannot update ScifiCharacter assignments for an active character." });

        // Race: 0 = clear, valid ID = set, null = no change
        if (request.ScifiRaceId == 0) {
            scifiCharacter.ClearScifiRace();
            scifiCharacter.ClearScifiSubrace();
        } else if (request.ScifiRaceId is not null) {
            // TODO: When multiple settings exist, ensure this race actually belongs to the Scifi setting
            var race = await db.ScifiRaces.FirstOrDefaultAsync(r => r.Id == request.ScifiRaceId);
            if (race is null)
                return BadRequest(new { message = "ScifiRace not found." });
            scifiCharacter.SetScifiRace(race.Id);
            scifiCharacter.ClearScifiSubrace();
        }

        // Subrace: 0 = clear, valid ID = set, null = no change
        if (request.ScifiSubraceId == 0 ||  request.ScifiRaceId == 0 ) {
            scifiCharacter.ClearScifiSubrace();
        } else if (request.ScifiSubraceId is not null) {
            var subrace = await db.ScifiSubraces.FirstOrDefaultAsync(sr => sr.Id == request.ScifiSubraceId);
            if (subrace is null)
                return BadRequest(new { message = "ScifiSubrace not found." });

            // Subrace requires a race to already be set (either persisted, or set earlier in this same request)
            var raceId = request.ScifiRaceId is not null && request.ScifiRaceId != 0
                ? request.ScifiRaceId
                : scifiCharacter.ScifiRaceId;

            if (raceId is null or 0)
                return BadRequest(new { message = "ScifiRace must be set before assigning a ScifiSubrace." });

            scifiCharacter.SetScifiSubrace(subrace.Id);
        }

        // Class: 0 = clear, valid ID = set, null = no change
        if (request.ScifiClassId == 0) {
            scifiCharacter.ClearScifiClass();
        } else if (request.ScifiClassId is not null) {
            // TODO: When multiple settings exist, ensure this class actually belongs to the Scifi setting
            var scifiClass = await db.ScifiClasses.FirstOrDefaultAsync(c => c.Id == request.ScifiClassId);
            if (scifiClass is null)
                return BadRequest(new { message = "ScifiClass not found." });
            scifiCharacter.SetScifiClass(scifiClass.Id);
        }

        // Background: 0 = clear, valid ID = set, null = no change
        if (request.ScifiBackgroundId == 0) {
            scifiCharacter.ClearScifiBackground();
        } else if (request.ScifiBackgroundId is not null) {
            // TODO: When multiple settings exist, ensure this background actually belongs to the Scifi setting
            var background = await db.ScifiBackgrounds.FirstOrDefaultAsync(b => b.Id == request.ScifiBackgroundId);
            if (background is null)
                return BadRequest(new { message = "ScifiBackground not found." });
            scifiCharacter.SetScifiBackground(background.Id);
        }

        await db.SaveChangesAsync();

        return Ok(new ScifiCharacterResponseDto(
            scifiCharacter.Id,
            scifiCharacter.CharacterId,
            scifiCharacter.ScifiRaceId,
            scifiCharacter.ScifiSubraceId,
            scifiCharacter.ScifiClassId,
            scifiCharacter.ScifiBackgroundId));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) {
        var scifiCharacter = await db.ScifiCharacters
            .Include(sc => sc.Character)
            .FirstOrDefaultAsync(sc => sc.Id == id && sc.Character!.PlayerId == playerContext.PlayerId);

        if (scifiCharacter is null)
            return NotFound(new { message = $"ScifiCharacter {id} was not found." });

        db.ScifiCharacters.Remove(scifiCharacter);
        await db.SaveChangesAsync();
        return NoContent();
    }

    public record CreateScifiCharacterDto(int CharacterId, int? ScifiRaceId, int? ScifiSubraceId, int? ScifiClassId, int? ScifiBackgroundId);
    public record UpdateScifiCharacterDto(int? ScifiRaceId, int? ScifiSubraceId, int? ScifiClassId, int? ScifiBackgroundId);
    public record ScifiCharacterResponseDto(int Id, int CharacterId, int? ScifiRaceId, int? ScifiSubraceId, int? ScifiClassId, int? ScifiBackgroundId);
}
