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
public class ScifiCharacterController(AppDbContext db, PlayerContext playerContext, ScifiTalentService scifiTalentService) : ControllerBase {

    [HttpGet]
    public async Task<IActionResult> GetAll() {
        var characters = await db.ScifiCharacters
            .Include(sc => sc.Character)
            .Where(sc => sc.Character!.PlayerId == playerContext.PlayerId)
            .Select(sc => new ScifiCharacterSummaryDto(
                sc.Id,
                sc.CharacterId,
                sc.ScifiRaceId,
                sc.ScifiSubraceId,
                sc.ScifiClassId,
                sc.ScifiBackgroundId
                ))
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
                sc.ScifiBackgroundId,
                sc.Modifiers
                    .Where(m => m.ModifierType != ModifierType.ArmorClass && m.ModifierType != ModifierType.Health)
                    .Select(m => new StatModifierResponseDto(m.ModifierType, m.Amount, m.Source))
                    .ToList(),
                sc.Talents
                    .Where(t => t.SourceType == TalentSourceEnum.Background)
                    .Select(t => (int?)t.ScifiTalentId)
                    .FirstOrDefault()
            )).FirstOrDefaultAsync();

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
                sc.ScifiBackgroundId,
                sc.Modifiers
                    .Where(m => m.ModifierType != ModifierType.ArmorClass && m.ModifierType != ModifierType.Health)
                    .Select(m => new StatModifierResponseDto(m.ModifierType, m.Amount, m.Source))
                    .ToList(),
                sc.Talents
                    .Where(t => t.SourceType == TalentSourceEnum.Background)
                    .Select(t => (int?)t.ScifiTalentId)
                    .FirstOrDefault()
                )).FirstOrDefaultAsync();

        if (scifiCharacter is null)
            return NotFound(new { message = $"ScifiCharacter for Character {characterId} was not found." });

        return Ok(scifiCharacter);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateScifiCharacterDto request) {
        var character = await db.Characters
            .FirstOrDefaultAsync(c => c.Id == request.CharacterId && c.PlayerId == playerContext.PlayerId);

        // Validation checks

        if (character is null)
            return NotFound(new { message = "Character not found." });

        if (character.IsActive)
            return BadRequest(new { message = "Cannot create a ScifiCharacter for an active character." });


        var existingScifiCharacter = await db.ScifiCharacters
            .FirstOrDefaultAsync(sc => sc.CharacterId == request.CharacterId);

        if (existingScifiCharacter is not null)
            return BadRequest(new { message = "ScifiCharacter already exists for this character." });


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

        // No existing scifi character, create new
        if (request.ScifiSubraceId is not null && request.ScifiRaceId is null)
            return BadRequest(new { message = "ScifiRace must be set before assigning a ScifiSubrace." });

        character.SetSetting(SettingEnum.Scifi);
        character.SetBaseHealth(0);

        var scifiCharacter = new ScifiCharacter(character.Id);
        CharacterStats stats = await db.CharacterStats.FirstOrDefaultAsync(cs => cs.CharacterId == character.Id)
                                ?? new CharacterStats(character.Id);

        if (request.ScifiRaceId is not null) {
            var race = await db.ScifiRaces
                .Include(r => r.RaceStats)
                .FirstAsync(r => r.Id == request.ScifiRaceId);

            ScifiSubrace? subrace = null;
            if (request.ScifiSubraceId is not null) {
                subrace = await db.ScifiSubraces
                    .Include(sr => sr.RaceStats)
                    .FirstAsync(sr => sr.Id == request.ScifiSubraceId);
            }

            var picks = (request.StatPicks ?? []).Select(sp => (sp.Stat, sp.Amount)).ToList();

            try {
                scifiCharacter.SetScifiRace(race, picks, subrace, stats);
            } catch (InvalidOperationException ex) {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Declare these before the class/background blocks so they're usable later
        ScifiClass? scifiClass = null;
        ScifiBackground? background = null;

        if (request.ScifiClassId is not null) {
            scifiClass = await db.ScifiClasses.FirstAsync(c => c.Id == request.ScifiClassId);
            scifiCharacter.SetScifiClass(scifiClass.Id);
        }

        if (request.ScifiBackgroundId is not null) {
            background = await db.ScifiBackgrounds.FirstAsync(b => b.Id == request.ScifiBackgroundId);
            scifiCharacter.SetScifiBackground(background.Id);
        }

        // Validate the pick before anything is saved
        ScifiTalent? pickedTalent = null;
        if (request.BackgroundTalentId is not null) {
            if (scifiClass is null || background is null)
                return BadRequest(new { message = "Cannot pick a background talent without both a class and background set." });

            var pool = await scifiTalentService.GetTalentPoolAsync(scifiClass, background);
            pickedTalent = pool.Pickable.FirstOrDefault(t => t.Id == request.BackgroundTalentId);
            if (pickedTalent is null)
                return BadRequest(new { message = "Background talent pick is not valid for the current class and background." });
        }

        db.ScifiCharacters.Add(scifiCharacter);

        if (!db.CharacterStats.Local.Contains(stats))
            db.CharacterStats.Add(stats);

        scifiCharacter.UpdateHealth(stats, scifiClass);
        await db.SaveChangesAsync();

        if (request.ScifiClassId is not null && request.ScifiBackgroundId is not null) {
            await scifiTalentService.SyncTalentsAsync(scifiCharacter);
            if (pickedTalent is not null)
                scifiCharacter.AddTalent(pickedTalent, TalentSourceEnum.Background);
            await db.SaveChangesAsync();
        }
        


        return Ok(new ScifiCharacterResponseDto(
            scifiCharacter.Id,
            scifiCharacter.CharacterId,
            scifiCharacter.ScifiRaceId,
            scifiCharacter.ScifiSubraceId,
            scifiCharacter.ScifiClassId,
            scifiCharacter.ScifiBackgroundId,
            scifiCharacter.Modifiers
                .Where(m => m.ModifierType != ModifierType.ArmorClass && m.ModifierType != ModifierType.Health)
                .Select(m => new StatModifierResponseDto(m.ModifierType, m.Amount, m.Source))
                .ToList(),
            scifiCharacter.Talents
                .FirstOrDefault(t => t.SourceType == TalentSourceEnum.Background)
                ?.ScifiTalentId
            ));
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateScifiCharacterDto request) {
        var scifiCharacter = await db.ScifiCharacters
            .Include(sc => sc.Character)
            .Include(sc => sc.ScifiRace)
            .Include(sc => sc.ScifiSubrace)
            .Include(sc => sc.ScifiClass)
            .Include(sc => sc.ScifiBackground)
            .Include(sc => sc.Talents)
            .Include(sc => sc.Modifiers)
            .FirstOrDefaultAsync(sc => sc.Id == id && sc.Character!.PlayerId == playerContext.PlayerId);

        if (scifiCharacter is null)
            return NotFound(new { message = $"ScifiCharacter {id} was not found." });

        if (scifiCharacter.Character!.IsActive)
            return BadRequest(new { message = "Cannot update ScifiCharacter assignments for an active character." });

        CharacterStats stats = await db.CharacterStats.FirstOrDefaultAsync(cs => cs.CharacterId == scifiCharacter.CharacterId)
            ?? new CharacterStats(scifiCharacter.CharacterId);

        // Race: 0 = clear, valid ID = set/change, null = no change (unless subrace alone is changing)
        if (request.ScifiRaceId == 0) {
            scifiCharacter.ClearScifiRace();
        } else if (request.ScifiRaceId is not null || request.ScifiSubraceId is not null) {

            ScifiRace? race;
            if (request.ScifiRaceId is not null) {
                race = await db.ScifiRaces
                    .Include(r => r.RaceStats)
                    .FirstOrDefaultAsync(r => r.Id == request.ScifiRaceId);
                if (race is null)
                    return BadRequest(new { message = "ScifiRace not found." });
            } else {
                race = await db.ScifiRaces
                    .Include(r => r.RaceStats)
                    .FirstOrDefaultAsync(r => r.Id == scifiCharacter.ScifiRaceId);
                if (race is null)
                    return BadRequest(new { message = "ScifiRace not found." });
            }

            ScifiSubrace? subrace = null;
            if (request.ScifiSubraceId is not null) {
                subrace = await db.ScifiSubraces
                    .Include(sr => sr.RaceStats)
                    .FirstOrDefaultAsync(sr => sr.Id == request.ScifiSubraceId);
                if (subrace is null)
                    return BadRequest(new { message = "ScifiSubrace not found." });
            }

            var picks = (request.StatPicks ?? []).Select(sp => (sp.Stat, sp.Amount)).ToList();

            try {
                scifiCharacter.SetScifiRace(race, picks, subrace, stats);
            } catch (InvalidOperationException ex) {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Class: 0 = clear, valid ID = set, null = no change
        // CHANGED: capture whether class is being cleared this request, since
        // ScifiClassId == 0 needs different handling than "not provided" below.
        var scifiClassCleared = request.ScifiClassId == 0;
        ScifiClass? scifiClass = null;

        if (request.ScifiClassId == 0) {
            scifiCharacter.ClearScifiClass();
        } else if (request.ScifiClassId is not null) {
            // TODO: When multiple settings exist, ensure this class actually belongs to the Scifi setting
            scifiClass = await db.ScifiClasses.FirstOrDefaultAsync(c => c.Id == request.ScifiClassId);
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
        
        if(request.ScifiClassId is not null || request.ScifiBackgroundId is not null) {
            db.ChangeTracker.DetectChanges(); // Ensure EF Core is aware of the changes before syncing talents
            await scifiTalentService.SyncTalentsAsync(scifiCharacter);
        }

        if(request.BackgroundTalentId is not null) {

            if(scifiCharacter.ScifiClass is null || scifiCharacter.ScifiBackground is null) {
                return BadRequest(new { message = "Cannot pick a background talent without both a class and background set." });
            }

            var talentPool = await scifiTalentService.GetTalentPoolAsync(scifiCharacter);
            var talent = talentPool.Pickable.FirstOrDefault(t => t.Id == request.BackgroundTalentId);
            if(talent is null) {
                return BadRequest(new { message = "Background talent pick is not valid for the current class and background." });
            }
            
            var currentPick = scifiCharacter.Talents.FirstOrDefault(t => t.SourceType == TalentSourceEnum.Background);

            if (currentPick?.ScifiTalentId != talent.Id) {
                // Remove the current background talent
                scifiCharacter.ClearTalentsBySource(TalentSourceEnum.Background);
                scifiCharacter.AddTalent(talent, TalentSourceEnum.Background);
            }
        }

        var classForHealth = scifiClassCleared ? null : scifiClass ?? scifiCharacter.ScifiClass;

        scifiCharacter.UpdateHealth(stats, classForHealth);

        await db.SaveChangesAsync();

        return Ok(new ScifiCharacterResponseDto(
            scifiCharacter.Id,
            scifiCharacter.CharacterId,
            scifiCharacter.ScifiRaceId,
            scifiCharacter.ScifiSubraceId,
            scifiCharacter.ScifiClassId,
            scifiCharacter.ScifiBackgroundId,
            scifiCharacter.Modifiers
                .Where(m => m.ModifierType != ModifierType.ArmorClass && m.ModifierType != ModifierType.Health)
                .Select(m => new StatModifierResponseDto(m.ModifierType, m.Amount, m.Source))
                .ToList(),
            scifiCharacter.Talents
                .FirstOrDefault(t => t.SourceType == TalentSourceEnum.Background)
                ?.ScifiTalentId
            ));
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

    public record StatPickDto(StatType Stat, int Amount);
    public record StatModifierResponseDto(ModifierType Stat, int Amount, ModifierSource Source);
    public record CreateScifiCharacterDto(int CharacterId, int? ScifiRaceId, int? ScifiSubraceId, int? ScifiClassId, int? ScifiBackgroundId, List<StatPickDto>? StatPicks = null, int? BackgroundTalentId = null);
    public record UpdateScifiCharacterDto(int? ScifiRaceId, int? ScifiSubraceId, int? ScifiClassId, int? ScifiBackgroundId, List<StatPickDto>? StatPicks = null, int? BackgroundTalentId = null);
    public record ScifiCharacterResponseDto(int Id, int CharacterId, int? ScifiRaceId, int? ScifiSubraceId, int? ScifiClassId, int? ScifiBackgroundId, List<StatModifierResponseDto> StatBonuses, int? BackgroundTalentId);
    public record ScifiCharacterSummaryDto(int Id, int CharacterId, int? ScifiRaceId, int? ScifiSubraceId, int? ScifiClassId, int? ScifiBackgroundId);
}
